using UnityEngine;
using System.Collections;

public class InimigoColado : InimigoPadrao {
	public Transform jogadorAlvoColado;
	public float intervaloDano = 1;
	
	private bool criado = false;
	private Transform posicaoAlvo;
	private float ultimoDano = 0;
	
	public override void Update() {
		if (MotorJogo.estado != MotorJogo.Estado.Ligado) return;
		
		if (jogadorAlvoColado != null && !criado) {
			posicaoAlvo = GameObject.FindWithTag("ear" + jogadorAlvoColado.tag.Substring(6) + ((Random.value > 0.5f) ? "L" : "R")).transform;
			criado = true;
		}
		
		if (posicaoAlvo == null) return;
		
		transform.position = posicaoAlvo.position; // TODO: Criar inimigoColado já na orelha, para não precisar movê-lo
		transform.rotation = Quaternion.Slerp(transform.rotation, posicaoAlvo.rotation, 0.5f);
		
		if (Time.time > ultimoDano + intervaloDano) {
			jogadorAlvoColado.GetComponent<ControleJogador>().AplicarDano(dano);
			ultimoDano = Time.time;
		}
	}
	
	public void Girar(string rotuloJogador) {
		if (jogadorAlvoColado != null && jogadorAlvoColado.tag == rotuloJogador) AplicarDano(1);
	}
}
