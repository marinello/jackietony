using UnityEngine;
using System.Collections;

public class ExplosaoLuz : MonoBehaviour {
	public float tempoIluminacao = 0.1f;
	public float tempoDissipacao = 0.1f;
	
	private float tempo;
	
	public void Awake() {
		if (!networkView.isMine) enabled = false;
	}
	
	public void Start() {
		tempo = Time.time;
	}
	
	public void Update() {
		if (Time.time > tempo + tempoIluminacao) {
			float dissipacao = (Time.time - tempo - tempoIluminacao) * tempoDissipacao;
			light.color = Color.Lerp(light.color, Color.black, dissipacao);
			if (dissipacao >= 1) {
				MotorJogo.Destruir(gameObject);
			}
		}
	}
}
