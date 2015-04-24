using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
	public bool vida = false;
	public int bomba = -1;
	
	public void Awake() {
		if (!networkView.isMine) enabled = false;
	}
	
	public void Start() {
		if (vida == false && bomba == 0) {
			if (tag == "BBasic") bomba = 0;
			else if (tag == "BStrong") bomba = 1;
			else if (tag == "BFire") bomba = 2;
			else if (tag == "BIce") bomba = 3;
			else if (tag == "BLight") bomba = 4;
			else vida = true;
		}
	}
	
	public void OnCollisionEnter(Collision colisao) {
		if (Radar.jogador == null || colisao.gameObject.tag != Radar.jogador.tag) return;
		
		if (vida) {
			Atributos.vidas++;
			// TODO: enviar para jogador remoto
		} else {
			Atributos.bomba = bomba;
		}
		MotorJogo.Destruir(gameObject);
	}
}
