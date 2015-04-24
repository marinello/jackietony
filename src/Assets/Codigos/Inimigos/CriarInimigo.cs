using UnityEngine;
using System.Collections;

public class CriarInimigo : MonoBehaviour {
	public GameObject entidade;
	
	private GameObject inimigo;
	
	public void OnTriggerEnter(Collider colisao) {
		if (MotorJogo.multiJogador && !MotorJogo.servidor) return;
		
		if((colisao.gameObject.tag == "playerJack" || colisao.gameObject.tag == "playerTony") && inimigo == null){
			inimigo = (GameObject) MotorJogo.Instanciar(entidade, transform.position, transform.rotation);
		}
	}
	
	void  OnTriggerExit(Collider colisao) {
		if (MotorJogo.multiJogador && !MotorJogo.servidor) return;
		
		if ((colisao.gameObject.tag == "playerJack" || colisao.gameObject.tag == "playerTony") && inimigo != null) {
			MotorJogo.Destruir(inimigo);
			inimigo = null;
		}
	}
}
