using UnityEngine;
using System.Collections;

public class InimigoPlanta : InimigoPadrao {
	public Transform planta;
	
	private Quaternion rotacaoInicial;
	
	public override void Patrulhar() {
		planta.animation.Stop();
		transform.LookAt(jogadorAlvo);
		rotacaoInicial = transform.rotation;
	}
	
	public override void Atacar() {
		float angulo = Quaternion.Angle(transform.rotation, rotacaoInicial);
		planta.animation.Play("Shoot");
		if(angulo <= 80.0f) {
			transform.Rotate(Vector3.right, Time.deltaTime * 100.0f);
		} else {
			transform.rotation = rotacaoInicial;
		}
	}
}
