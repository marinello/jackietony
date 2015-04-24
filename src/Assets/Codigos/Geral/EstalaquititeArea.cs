using UnityEngine;
using System.Collections;

public class EstalaquititeArea : MonoBehaviour {

	public Transform estalaq;
	
	public void Awake() {
		if (!networkView.isMine) enabled = false;
	}
	
	public void OnTriggerEnter(Collider colisao) {
		if (MotorJogo.multiJogador && !networkView.isMine || !colisao.gameObject.tag.StartsWith("player")) return;
		
		MotorJogo.Instanciar(estalaq, transform.position, transform.rotation);
	}
}
