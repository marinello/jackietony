using UnityEngine;
using System.Collections;

public class Anel : MonoBehaviour {
	
	public int superBonus = 500;
	public AudioClip[] audioClips = new AudioClip[4];
	public TextMesh textoAnel;
	
	private bool destruido = false;
	private float tempo;
	
	private static readonly float tempoPersistencia = 3;
	
	public void Start () {
		collider.isTrigger = true;	
	}
	
	public void Update() {
		if (destruido && Time.time > tempo + tempoPersistencia) {
			MotorJogo.Ocultar(transform.parent.gameObject, true);
			gameObject.active = true;
		}
	}
	
	public void OnTriggerEnter(Collider colisao) {
		if (destruido || colisao == null || !tag.StartsWith("anel") || Radar.jogador == null || Radar.jogador.tag != colisao.tag) return;
		
		int pontuacaoAnel = Atributos.ultimaPontuacaoAnel;
		
		int anelAtual = int.Parse(tag.Substring(4));
		int aneisDecorridos = (anelAtual - Atributos.ultimoAnel + 5) % 5;
		int aneisDecorridosInicio = (anelAtual - Atributos.primeiroAnel + 5) % 5;
		
		if (Atributos.primeiroAnel != -1 && aneisDecorridos == 1) {
			if (aneisDecorridosInicio >= 4) {
				Atributos.primeiroAnel = -1;
				pontuacaoAnel += superBonus;
			} else {
				pontuacaoAnel *= 2;
			}
			audio.PlayOneShot(audioClips[Mathf.Clamp(aneisDecorridosInicio - 1, 0, 3)]);
		} else {
			Atributos.primeiroAnel = anelAtual;
			pontuacaoAnel = 10;
			audio.PlayOneShot(audioClips[0]);
		}
		Atributos.ultimoAnel = anelAtual;
		Atributos.ultimaPontuacaoAnel = pontuacaoAnel;
		Atributos.pontos += pontuacaoAnel;
		
		if (textoAnel != null) textoAnel.text = pontuacaoAnel.ToString();
		
		destruido = true;
		tempo = Time.time;
	}
	
	public void Reiniciar() {
		if (textoAnel != null) textoAnel.text = "";
		destruido = false;
		MotorJogo.Ocultar(transform.parent.gameObject, false);
	}
}