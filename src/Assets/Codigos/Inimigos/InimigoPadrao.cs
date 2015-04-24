using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InimigoPadrao : MonoBehaviour {
	public Transform entidadeCongelada;
	public Transform entidadeChamas;
	public Transform entidadeExplosao;
	public int vida = 10;
	public int dano = 1;
	public float distanciaPatrulha = 80;
	public float distanciaAtaque = 30;
	protected Transform jogadorAlvo = null;
	
	public void Awake() {
		if (MotorJogo.multiJogador && !MotorJogo.servidor) enabled = false;
	}
	
	public void Start() {
		//Altera o dano e vida dos inimigos de acordo com a dificuldade do jogo
		vida = vida * ((MotorJogo.dificuldade+2)/2);
		dano = dano * ((MotorJogo.dificuldade+2)/2);
	}
	
	public virtual void Update() {
		List<Transform> alvos = MotorJogo.jogadoresAlvo;
		
		if (MotorJogo.estado != MotorJogo.Estado.Ligado || alvos == null || alvos.Count == 0) return;
		
		float menorDistancia = -1;
		
		foreach (Transform alvo in alvos) {
			float distancia = Vector3.Distance(transform.position, alvo.position);
			if (menorDistancia == -1 || distancia < menorDistancia) {
				jogadorAlvo = alvo;
				menorDistancia = distancia;
			}
		}
		
		if (jogadorAlvo != null) {
			float distancia = Vector3.Distance(transform.position, jogadorAlvo.position);
			
			if (distancia <= distanciaAtaque) {
				Atacar();
			} else if (distanciaPatrulha == -1 || distancia <= distanciaPatrulha) {
				Patrulhar();
			}
		}
	}
	
	public virtual void OnCollisionEnter(Collision colisao) {
		if (Radar.jogador == null || colisao.gameObject.tag != Radar.jogador.tag) return;
		
		Explodir();
		colisao.gameObject.GetComponent<ControleJogador>().AplicarDano(dano);
	}
	
	public virtual void Patrulhar() {
	}
	
	public virtual void Atacar() {
	}
	
	[RPC]
	public virtual void AplicarDano(int dano) {
		if (MotorJogo.multiJogador && !MotorJogo.servidor) {
			transform.networkView.RPC("AplicarDano", RPCMode.OthersBuffered, dano); // TODO: fazer dano aplicável localmente
			return;
		}
		
		vida -= dano;
		if (vida <= 0) {
			Explodir();
		}
	}
	
	[RPC]
	public virtual void Congelar() {
		if (MotorJogo.multiJogador && !networkView.isMine) {
			transform.networkView.RPC("Congelar", RPCMode.OthersBuffered);
			return;
		}
		MotorJogo.Instanciar(entidadeCongelada, transform.position, transform.rotation);
		MotorJogo.Destruir(gameObject);
	}
	
	[RPC]
	public virtual void BotarFogo() {
		if (MotorJogo.multiJogador && !networkView.isMine) {
			transform.networkView.RPC("BotarFogo", RPCMode.OthersBuffered);
			return;
		}
		MotorJogo.Instanciar(entidadeChamas, transform.position, transform.rotation);
		MotorJogo.Destruir(gameObject);
	}
	
	[RPC]
	public virtual void Explodir() {
		if (MotorJogo.multiJogador && !networkView.isMine) {
			transform.networkView.RPC("Explodir", RPCMode.OthersBuffered);
			return;
		}
		MotorJogo.Instanciar(entidadeExplosao, transform.position, transform.rotation);
		MotorJogo.Destruir(gameObject);
	}
}
