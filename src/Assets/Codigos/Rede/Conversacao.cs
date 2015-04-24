using UnityEngine;
using System.Collections;

public class Conversacao : MonoBehaviour {
	
	public void Awake() {
		DontDestroyOnLoad(gameObject);
	}
	
	// CLIENTE PARA SERVIDOR
	[RPC]
	public void RequisitarDadosServidor() {
		if (!MotorJogo.servidor || MotorJogo.estado != MotorJogo.Estado.Conectando) return;
		
		float posicaoX = 0;
		float posicaoY = 0;
		float posicaoZ = 0;
		float rotacaoX = 0;
		float rotacaoY = 0;
		float rotacaoZ = 0;
		int pontos = 0;
		int energia = 100;
		int vidas = 3;
		int temperatura = 0;
		int bomba = 0;
		if (MotorJogo.origemSalvo && MotorJogo.jogadorRemotoSalvo != null) {
			posicaoX = MotorJogo.jogadorRemotoSalvo.posicao.x;
			posicaoY = MotorJogo.jogadorRemotoSalvo.posicao.y;
			posicaoZ = MotorJogo.jogadorRemotoSalvo.posicao.z;
			rotacaoX = MotorJogo.jogadorRemotoSalvo.rotacao.x;
			rotacaoY = MotorJogo.jogadorRemotoSalvo.rotacao.y;
			rotacaoZ = MotorJogo.jogadorRemotoSalvo.rotacao.z;
			pontos = MotorJogo.jogadorRemotoSalvo.pontos;
			energia = MotorJogo.jogadorRemotoSalvo.energia;
			vidas = MotorJogo.jogadorRemotoSalvo.vidas;
			temperatura = MotorJogo.jogadorRemotoSalvo.temperatura;
			bomba = MotorJogo.jogadorRemotoSalvo.bomba;
			networkView.RPC("AtributosSalvos", RPCMode.OthersBuffered, posicaoX, posicaoY, posicaoZ, rotacaoX, rotacaoY, rotacaoZ, pontos, energia, vidas, temperatura, bomba);
		}
		networkView.RPC("DadosServidor", RPCMode.OthersBuffered, 1 - MotorJogo.idJogador, MotorJogo.rotuloJogadorRemoto, MotorJogo.dificuldade, MotorJogo.fase);
	}
	
	[RPC]
	public void ClientePronto() {
		if (!MotorJogo.servidor || MotorJogo.estado != MotorJogo.Estado.Conectando) return;
		
		MotorJogo.JogoIniciado();
	}

	[RPC]
	public void JogadorRemoto(ArquivoJogo.JogoSalvoJogador jogadorRemoto) {
		if (!MotorJogo.servidor || MotorJogo.estado != MotorJogo.Estado.Conectando && MotorJogo.estado != MotorJogo.Estado.EmPausa) return;

		ArquivoJogo.SalvarJogoAtual(jogadorRemoto);
	}
	
	// SERVIDOR PARA CLIENTE
	[RPC]
	public void AtributosSalvos(float posicaoX, float posicaoY, float posicaoZ, float rotacaoX, float rotacaoY, float rotacaoZ, int pontos, int energia, int vidas, int temperatura, int bomba) {
		if (!MotorJogo.primeiraFase || MotorJogo.servidor || MotorJogo.estado != MotorJogo.Estado.Conectando) return;
		
		MotorJogo.origemSalvo = true;
		MotorJogo.posicaoCriacao = new Vector3(posicaoX, posicaoY, posicaoZ);
		MotorJogo.rotacaoCriacao = new Vector3(rotacaoX, rotacaoY, rotacaoZ);
		Atributos.pontos = pontos;
		Atributos.energia = energia;
		Atributos.vidas = vidas;
		Atributos.temperatura = temperatura;
		Atributos.bomba = bomba;
		Atributos.primeiroAnel = -1;
	}
	
	[RPC]
	public void DadosServidor(int jogador, string rotuloJogador, int dificuldade, int fase) {
		if (MotorJogo.servidor || MotorJogo.estado != MotorJogo.Estado.Conectando) return;
		
		MotorJogo.jogador = MotorJogo.jogadorRemoto[jogador];
		MotorJogo.rotuloJogador = rotuloJogador;
		MotorJogo.dificuldade = dificuldade;
		MotorJogo.fase = fase;
		Atributos.rotuloJogador = rotuloJogador.ToUpper();

		networkView.RPC("ClientePronto", RPCMode.OthersBuffered);
		MotorJogo.JogoIniciado();
	}

	[RPC]
	public void SalvarJogoRemoto() {
		if (MotorJogo.servidor || MotorJogo.estado != MotorJogo.Estado.Conectando && MotorJogo.estado != MotorJogo.Estado.EmPausa) return;

		networkView.RPC("JogadorRemoto", RPCMode.OthersBuffered, ArquivoJogo.GerarJogadorSalvo());
	}
	
	// PARA AMBOS
	[RPC]
	public void AtualizarAlvos() {
		if (MotorJogo.estado != MotorJogo.Estado.Ligado) return;
		
		MotorJogo.jogadoresAlvo.Clear();
		GameObject alvo = GameObject.FindWithTag("playerJack");
		if (alvo != null) MotorJogo.jogadoresAlvo.Add(alvo.transform);
		
		alvo = GameObject.FindWithTag("playerTony");
		if (alvo != null) MotorJogo.jogadoresAlvo.Add(alvo.transform);
	}
	
	[RPC]
	public void Pausar() {
		if (MotorJogo.estado != MotorJogo.Estado.Ligado) return;
		
		MotorJogo.Pausar(false);
	}
	
	[RPC]
	public void Continuar() {
		if (MotorJogo.estado != MotorJogo.Estado.EmPausa) return;
		
		MotorJogo.Continuar(false);
	}
	
	[RPC]
	public void Reiniciar() {
		if (MotorJogo.estado != MotorJogo.Estado.EmPausa) return;
		
		MotorJogo.Reiniciar();
	}
	
	[RPC]
	public void Vitoria() {
		if (MotorJogo.estado != MotorJogo.Estado.Ligado) return;
		
		MotorJogo.Vitoria();
	}
	
	[RPC]
	public void PassarFase() {
		if (MotorJogo.estado != MotorJogo.Estado.Ligado) return;
		
		MotorJogo.PassarFase();
	}
	
}
