using UnityEngine;
using System.Collections;

public class InimigoBesouro : InimigoPadrao {
	
	public Transform tiro;
	public Transform arma;
	public float velocidade = 20;
	public float intervaloAtaque = 0.1f;
	
	protected float ultimoAtaque = 0;
	
	public override void Atacar() {
		transform.localScale = Vector3.one * 2;
		transform.LookAt(jogadorAlvo);
		transform.Translate(Vector3.forward * velocidade * Time.deltaTime);
		
		if (Time.time > ultimoAtaque + intervaloAtaque) {
			if (Random.value > 0.5f) MotorJogo.Instanciar(tiro, arma.transform.position, transform.rotation);
			ultimoAtaque = Time.time;
		}
	}
	
	public override void Patrulhar() {
		transform.localScale = Vector3.one * 3;
	}
}
