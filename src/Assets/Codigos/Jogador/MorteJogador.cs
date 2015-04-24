using UnityEngine;
using System.Collections;

public class MorteJogador : MonoBehaviour {
	public Vector3 posicao;
	public Vector3 rotacao;
	public bool renascer;
	
	private float tempo;
	
	private static readonly float tempoAnimacao = 3;
	
	public void Awake() {
		if (!networkView.isMine) enabled = false;
	}
	
	public void Start() {
		tempo = Time.time;
	}
	
	public void Update() {
		if (Time.time - tempo > tempoAnimacao) {
			if (renascer) {
				Atributos.vidas--;
				Atributos.energia = 100;
				MotorJogo.NovoJogador(posicao, Quaternion.Euler(rotacao));
				MotorJogo.Destruir(gameObject);
			} else {
				MotorJogo.FimJogo();
			}
		}
	}
}
