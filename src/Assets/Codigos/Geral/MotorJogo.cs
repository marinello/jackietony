using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MotorJogo {
	
	public static int idJogador;
	public static Transform jogador;
	public static string rotuloJogador;
	public static Transform[] jogadorRemoto;
	public static string rotuloJogadorRemoto;
	public static int dificuldade = 0;
	public static bool multiJogador = false;
	public static bool servidor = false;
	public static bool primeiraFase = true;
	public static string enderecoServidor;
	public static Estado estado = Estado.Desligado;
	public static int fase = 1;
	public static NetworkView conversacao;
	public static List<Transform> jogadoresAlvo = new List<Transform>();
	public static Transform entidadeConversacao;
	public static Vector3 posicaoInicial = Vector3.zero;
	public static Vector3 rotacaoInicial = Quaternion.identity.eulerAngles;
	public static bool origemSalvo = false;
	public static ArquivoJogo.JogoSalvoJogador jogadorRemotoSalvo;
	public static Vector3 posicaoCriacao = Vector3.zero;
	public static Vector3 rotacaoCriacao = Quaternion.identity.eulerAngles;
	
	private static int portaServidor = 25001;
	
	private MotorJogo() {}
	
	private static void Sair() {
		Time.timeScale = 1;
		Screen.showCursor = true;
		Screen.lockCursor = false;
		estado = Estado.Desligado;
		if (multiJogador && Network.peerType != NetworkPeerType.Disconnected) {
			Destruir(conversacao.gameObject);
			conversacao = null;
			Network.RemoveRPCs(Network.player);
			Network.DestroyPlayerObjects(Network.player);
			Network.Disconnect(5);
		}
	}
	
	public static void MenuInicial() {
		if (estado == Estado.Desligado) return;
		Sair();
		Application.LoadLevel("MenuInicial");
	}
	
	public static void FimJogo() {
		if (estado == Estado.Desligado) return;
		Sair();
		Application.LoadLevel("FimJogo");
	}
	
	public static void Vitoria() {
		if (MotorJogo.estado != MotorJogo.Estado.Ligado) return;
		
		if (multiJogador && conversacao != null) conversacao.RPC("Vitoria", RPCMode.OthersBuffered);
		
		if (estado == Estado.Desligado) return;
		Sair();
		Application.LoadLevel("Vitoria");
	}
	
	public static void IniciarJogo() {
		if (primeiraFase) {
			if (!multiJogador || servidor) Atributos.rotuloJogador = rotuloJogador.ToUpper();
			Atributos.pontos = 0;
			Atributos.energia = 100;
			Atributos.vidas = 3;
			Atributos.temperatura = 0;
			Atributos.bomba = 0;
			Atributos.primeiroAnel = -1;
			conversacao = null;
		}

		origemSalvo = false;
		
		Application.LoadLevelAsync("Fase" +  fase);
		
		jogadoresAlvo.Clear();
		//Time.timeScale = 0;
		Screen.showCursor = false;
		Screen.lockCursor = true;
		
		estado = Estado.Carregando;
	}
	
	public static void IniciarMultiJogo() {
		if (!multiJogador) return;
		
		if (!primeiraFase) {
			JogoIniciado();
			return;
		}
		
		if (servidor) {
			Network.InitializeServer(1, portaServidor, false /*!Network.HavePublicAddress()*/);
		} else {
			Network.Connect(enderecoServidor, portaServidor);
		}
	}
	
	public static void IniciarJogoSalvo(ArquivoJogo.JogoSalvo jogoSalvo) {
		primeiraFase = true;
		multiJogador = jogoSalvo.multiJogador;
		if (multiJogador) servidor = true;
		dificuldade = jogoSalvo.dificuldade;
		fase = jogoSalvo.fase;
		idJogador = jogoSalvo.jogadorLocal.jogador;
		jogador = jogadorRemoto[idJogador];
		rotuloJogador = jogoSalvo.jogadorLocal.rotuloJogador;
		Atributos.rotuloJogador = rotuloJogador.ToUpper();
		origemSalvo = true;
		posicaoCriacao = jogoSalvo.jogadorLocal.posicao.ToVector3();
		rotacaoCriacao = jogoSalvo.jogadorLocal.rotacao.ToVector3();
		Atributos.pontos = jogoSalvo.jogadorLocal.pontos;
		Atributos.energia = jogoSalvo.jogadorLocal.energia;
		Atributos.vidas = jogoSalvo.jogadorLocal.vidas;
		Atributos.temperatura = jogoSalvo.jogadorLocal.temperatura;
		Atributos.bomba = jogoSalvo.jogadorLocal.bomba;
		Atributos.primeiroAnel = -1;
		jogadorRemotoSalvo = jogoSalvo.jogadorRemoto;
		rotuloJogadorRemoto = jogadorRemotoSalvo.rotuloJogador;

		Application.LoadLevelAsync("Fase" +  fase);

		jogadoresAlvo.Clear();
		MotorJogo.conversacao = null;
		Screen.showCursor = false;
		Screen.lockCursor = true;
		
		estado = Estado.Carregando;
	}
	
	public static Object Instanciar(Object objeto, Vector3 posicao, Quaternion rotacao) {
		if (multiJogador && Network.peerType != NetworkPeerType.Disconnected) {
			return Network.Instantiate(objeto, posicao, rotacao, 0);
		} else {
			return Object.Instantiate(objeto, posicao, rotacao);
		}
	}
	
	public static void Destruir(GameObject objeto) {
		if (multiJogador && Network.peerType != NetworkPeerType.Disconnected) {
			Network.Destroy(objeto);
		} else {
			Object.Destroy(objeto);
		}
	}
	
	public static void PrepararJogo() {
		if (!multiJogador || !primeiraFase) return;
		
		estado = Estado.Conectando;
		
		if (servidor) {
			conversacao = ((Transform) Instanciar(entidadeConversacao, Vector3.zero, Quaternion.identity)).networkView;
		} else {
			conversacao.RPC("RequisitarDadosServidor", RPCMode.OthersBuffered);
		}
	}

	public static void JogoIniciado() {
		if (estado == Estado.Desligado) {
			Application.LoadLevel("MenuInicial");
			return;
		}
		if (multiJogador && (primeiraFase && estado != Estado.Conectando || !primeiraFase && estado != Estado.Carregando) || !multiJogador && estado != Estado.Carregando || jogador == null) {
			MenuInicial();
			return;
		}
		
		if (!primeiraFase || !origemSalvo) {
			posicaoCriacao = posicaoInicial;
			rotacaoCriacao = rotacaoInicial;
		}

		if (multiJogador && !servidor && origemSalvo && primeiraFase && fase != 1) {
			primeiraFase = false;
			Application.LoadLevelAsync("Fase" +  fase);
			IniciarJogo();
			return;
		}

		NovoJogador(posicaoCriacao, Quaternion.Euler(rotacaoCriacao));
		
		primeiraFase = false;
		estado = Estado.Ligado;
		Time.timeScale = 1;
	}
	
	public static void NovoJogador(Vector3 posicao, Quaternion rotacao) {
		Transform novoJogador = (Transform) Instanciar(jogador, posicao, rotacao);
		jogadoresAlvo.Add(novoJogador);
		Camera.main.GetComponent<CameraJogador>().jogador = novoJogador;
		Radar.jogador = novoJogador;
		if (MotorJogo.conversacao != null) MotorJogo.conversacao.RPC("AtualizarAlvos", RPCMode.AllBuffered);
	}
	
	public static void Pausar() {
		Pausar(true);
	}
	
	public static void Pausar(bool notificar) {
		if (estado != Estado.Ligado) return;
		if (notificar && conversacao != null) conversacao.RPC("Pausar", RPCMode.OthersBuffered);
		Time.timeScale = 0;
		Screen.showCursor = true;
		Screen.lockCursor = false;
		estado = Estado.EmPausa;
	}
	
	public static void Continuar() {
		Continuar(true);
	}

	public static void Continuar(bool notificar) {
		if (estado != Estado.EmPausa) return;
		if (notificar && conversacao != null) conversacao.RPC("Continuar", RPCMode.OthersBuffered);
		Time.timeScale = 1;
		Screen.showCursor = false;
		Screen.lockCursor = true;
		estado = Estado.Ligado;
	}
	
	public static void Reiniciar() {
		if (estado != Estado.EmPausa) return;
		
		if (multiJogador && conversacao != null) {
			conversacao.RPC("Reiniciar", RPCMode.OthersBuffered);
		}
		
		jogadoresAlvo.Clear();
		if (Radar.jogador != null) {
			Destruir(Radar.jogador.gameObject);
			Radar.jogador = null;
		}
		
		GameObject[] inimigos = GameObject.FindGameObjectsWithTag("wingEnemy");
		foreach(GameObject inimigo in inimigos){
			if (inimigo != null) Destruir(inimigo);
		}
		
		for (int i = 1; i <= 5; i++) {
			GameObject[] aneis = GameObject.FindGameObjectsWithTag("anel" + i);
			foreach(GameObject anel in aneis){
				if (anel != null) {
					Anel cAnel = anel.GetComponent<Anel>();
					if (cAnel != null) cAnel.Reiniciar();
				}
			}
		}
		
		Atributos.pontos = 0;
		Atributos.energia = 100;
		Atributos.vidas = 3;
		Atributos.temperatura = 0;
		Atributos.bomba = 0;
		Atributos.primeiroAnel = -1;
		
		Screen.showCursor = false;
		Screen.lockCursor = true;
		
		if (multiJogador) estado = Estado.Conectando;
		else estado = Estado.Carregando;
		
		JogoIniciado();
	}
	
	public static void Ocultar(GameObject objeto, bool ocultar) {
		objeto.active = !ocultar;
		for (int i = 0, m = objeto.transform.GetChildCount(); i < m; i++) {
			Ocultar(objeto.transform.GetChild(i).gameObject, ocultar);
		}
	}
	
	public static void PassarFase() {
		if (MotorJogo.estado != MotorJogo.Estado.Ligado) return;
		
		fase++;
		if (fase > 3) {
			Vitoria();
			return;
		}
		
		if (multiJogador && conversacao != null) conversacao.RPC("PassarFase", RPCMode.OthersBuffered);
		
		Atributos.energia = 100;
		Atributos.vidas = 3;
		Atributos.temperatura = 0;
		Atributos.bomba = 0;
		Atributos.primeiroAnel = -1;
		IniciarJogo();
	}
	
	public enum Estado {
		Desligado,
		Carregando,
		Conectando,
		Ligado,
		EmPausa
	}
}

