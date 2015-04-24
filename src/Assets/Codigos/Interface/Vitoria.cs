using UnityEngine;
using System.Collections;

public class Vitoria : MonoBehaviour {
	public Texture2D creditos;
	public float velocidade = 0.02f;
	
	private float tempo;
	
	public void Start() {
		tempo = Time.time;
		if (MotorJogo.estado == MotorJogo.Estado.Desligado) MotorJogo.estado = MotorJogo.Estado.EmPausa;
	}
	
	public void OnGUI() {
		if (creditos == null) return;
		
		float largura = Screen.width * 0.8f;
		float altura = largura * creditos.height / creditos.width;
		float passado = (Time.time - tempo) * velocidade * altura;
		
		GUI.DrawTexture(
			new Rect(
				Screen.width * 0.1f,
				Screen.height * 1.1f - passado,
				largura,
				altura
			),
			creditos
		);
		
		if (passado > altura + Screen.height * 1.2f) MotorJogo.MenuInicial();
	}
}
