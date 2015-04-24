using UnityEngine;
using System.Collections;

public class AnelFinal : MonoBehaviour {
	
	public void Start () {
		collider.isTrigger = true;	
	}
	
	public void OnTriggerEnter(Collider colisao) {
		if (colisao == null || !tag.StartsWith("anel") || Radar.jogador == null || Radar.jogador.tag != colisao.tag) return;
		
		MotorJogo.PassarFase();
	}
}
