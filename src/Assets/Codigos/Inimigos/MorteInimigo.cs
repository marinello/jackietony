using UnityEngine;
using System.Collections;

public class MorteInimigo : MonoBehaviour {
	private float tempo;
	
	public void Awake() {
		if (!networkView.isMine) enabled = false;
	}
	
	public void Start() {
		tempo = Time.time;
	}
	
	public void Update() {
		if (Time.time - tempo > 6) {
			MotorJogo.Destruir(gameObject);
		}
	}
}
