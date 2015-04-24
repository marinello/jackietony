using UnityEngine;
using System.Collections;

public class RochaArea : MonoBehaviour {

	public Transform rock;
	
	public void Awake() {
		if (!networkView.isMine) enabled = false;
	}
	
	public void OnTriggerEnter(Collider colisao) {
		if (MotorJogo.multiJogador && !networkView.isMine || !colisao.gameObject.tag.StartsWith("player")) return;
		
		MotorJogo.Instanciar(rock, transform.position, transform.rotation);
	}
}
