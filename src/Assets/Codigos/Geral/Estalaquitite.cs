using UnityEngine;
using System.Collections;

public class Estalaquitite : MonoBehaviour {

	private float tempo;
	
	public int dano = 5;
	
	public void Awake() {
		if (!networkView.isMine) enabled = false;
	}
	
	public void Start() {
		tempo = Time.time;
		dano = dano * ((MotorJogo.dificuldade+2)/2);
	}

	// Update is called once per frame
	void Update () {
		if (Time.time - tempo > 12) {
			MotorJogo.Destruir(gameObject);
		}
	}
	
	public virtual void OnCollisionEnter(Collision colisao) {
		if (Radar.jogador == null || colisao.gameObject.tag != Radar.jogador.tag) return;
		
		colisao.gameObject.GetComponent<ControleJogador>().AplicarDano(dano);
	}
}
