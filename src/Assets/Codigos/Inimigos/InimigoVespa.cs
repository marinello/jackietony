using UnityEngine;
using System.Collections;
public class InimigoVespa : InimigoPadrao {
	
	public Transform vespaColada;
	public float velocidade = 100;
	
	public override void OnCollisionEnter(Collision colisao) {
		if (Radar.jogador == null || colisao.gameObject.tag != Radar.jogador.tag) return;
		
		if (Random.value > 0.5f) {
			Transform inimigo = (Transform) MotorJogo.Instanciar(vespaColada, transform.position, Quaternion.identity);
			MotorJogo.Destruir(gameObject);
			inimigo.GetComponent<InimigoColado>().jogadorAlvoColado = colisao.transform;
		} else {
			Explodir();
			colisao.gameObject.GetComponent<ControleJogador>().AplicarDano(dano);
		}
	}
	
	public override void Atacar() {
		transform.LookAt(jogadorAlvo);
		transform.Translate(Vector3.forward * velocidade * Time.deltaTime);
	}
	
}
