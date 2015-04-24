using UnityEngine;
using System.Collections;

public class CameraJogador : MonoBehaviour {
	public Transform jogador;
	
	private static readonly float distancia = 25;
	private static readonly float altura = 7;
	private static readonly float velocidadeHorizontal = 2;
	private static readonly float velocidadeRotacao = 3;
	
	void LateUpdate() {
		if (jogador == null || MotorJogo.estado != MotorJogo.Estado.Ligado) return;
		
		float anguloDesejado = jogador.eulerAngles.y;
		float alturaDesejada = jogador.position.y + altura;
		
		float anguloAtual = transform.eulerAngles.y;
		float alturaAtual = transform.position.y;
		
		anguloAtual = Mathf.LerpAngle(anguloAtual, anguloDesejado, velocidadeRotacao * Time.deltaTime);
		alturaAtual = Mathf.Lerp(alturaAtual, alturaDesejada, velocidadeHorizontal * Time.deltaTime);
		
		Quaternion rotacaoAtual = Quaternion.Euler(0, anguloAtual, 0);
		
		Vector3 posicao = jogador.position - rotacaoAtual * Vector3.forward * distancia;
		posicao.y = alturaAtual;
		transform.position = posicao;
		transform.LookAt(Vector3.up * altura + jogador.position);
	}
}
