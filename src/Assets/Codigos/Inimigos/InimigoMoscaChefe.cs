using UnityEngine;
using System.Collections;

public class InimigoMoscaChefe : InimigoBesouro {
	
	private bool morreu = false;
	
	public override void Atacar() {
		animation.Play("attack");
		transform.LookAt(jogadorAlvo);
		transform.Translate(Vector3.forward * velocidade * Time.deltaTime);
	}
	
	public override void Patrulhar() {
		animation.Play("idle");
		transform.LookAt(jogadorAlvo);
		transform.Translate(Vector3.forward * velocidade * 2 * Time.deltaTime);
		
		if (Time.time > ultimoAtaque + intervaloAtaque) {
			if (Random.value > 0.5f) MotorJogo.Instanciar(tiro, arma.transform.position, transform.rotation);
			ultimoAtaque = Time.time;
		}
	}
	
	public override void OnCollisionEnter(Collision colisao) {
		if (morreu || Radar.jogador == null || colisao.gameObject.tag != Radar.jogador.tag) return;
		
		colisao.gameObject.GetComponent<ControleJogador>().AplicarDano(dano);
		colisao.gameObject.GetComponent<ControleJogador>().BateVolta();
	}
	
	public override void Congelar() {
		AplicarDano(5);
	}
	
	public override void BotarFogo() {
		AplicarDano(5);
	}
	
	public override void Explodir() {
		if (MotorJogo.multiJogador) networkView.RPC("Morreu", RPCMode.OthersBuffered);
		morreu = true;
		enabled = false;
		animation.Play("death");
		StartCoroutine(PassaFase());
	}
	
	[RPC]
	public void Morreu() {
		morreu = true;
	}
	
	private IEnumerator PassaFase() {
		yield return new WaitForSeconds(5);
		((Transform) MotorJogo.Instanciar(entidadeExplosao, transform.position, transform.rotation)).localScale = Vector3.one * 20;
		GameObject passaFase = new GameObject("PassaFase");
		passaFase.AddComponent<PassaFase>();
		MotorJogo.Destruir(gameObject);
	}
}
