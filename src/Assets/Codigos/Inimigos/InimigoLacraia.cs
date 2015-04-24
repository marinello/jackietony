using UnityEngine;
using System.Collections;

public class InimigoLacraia : InimigoPadrao {
	
	public float velocidade = 30;
	
	public override void Atacar() {
		transform.Translate(Vector3.up * Time.deltaTime * velocidade);
	}
	
}
