using UnityEngine;
using System.Collections;

public class TiroInimigo : MonoBehaviour {
	public int dano = 2;
	public int velocidade = 40;
	private Vector3 posicaoInicial;
	
	public void Awake() {
		if (!networkView.isMine) enabled = false;
	}
	
	public void Start() {
		posicaoInicial = transform.position;
	}
	
	public void Update() {
		if (MotorJogo.estado != MotorJogo.Estado.Ligado) return;
		
		transform.Translate(Vector3.forward * velocidade * Time.deltaTime);
		//transform.position += transform.forward * Time.deltaTime * velocidade;
		
		float distancia = Vector3.Distance(posicaoInicial, transform.position);
		if (distancia >= 200.0f) {
			MotorJogo.Destruir(gameObject);
		}
	}
	
	public void OnCollisionEnter(Collision colisao) {
		if (colisao.gameObject.tag == "enemy" || colisao.gameObject.tag == "wingEnemy" || colisao.gameObject.tag.StartsWith("anel")) return;
		
		if (Radar.jogador != null && colisao.gameObject.tag == Radar.jogador.tag) {
			colisao.gameObject.GetComponent<ControleJogador>().AplicarDano(dano);
		}
		MotorJogo.Destruir(gameObject);
	}
}
