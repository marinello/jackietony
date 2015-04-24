using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
//using System.Text;

public class ArquivoJogo {
	
	public static List<JogoSalvo> ListarJogosSalvos() {
		List<JogoSalvo> listaJogosSalvos = new List<JogoSalvo>();
		for (int i = 0; i < 10; i++) {
			string arquivo = "JogoSalvo" + i + ".save";
			if (File.Exists(arquivo)) {
				JogoSalvo jogoSalvoLocal = CarregarJogoSalvo(arquivo);
				if (jogoSalvoLocal != null) {
					listaJogosSalvos.Add(jogoSalvoLocal);
				}
			}
		}
		return listaJogosSalvos;
	}
	
	public static JogoSalvo CarregarJogoSalvo(string enderecoArquivo) {
		JogoSalvo jogoSalvo = null;
		FileStream arquivo = new FileStream(enderecoArquivo, FileMode.Open, FileAccess.Read);
		try {
			BinaryFormatter formatador = new BinaryFormatter();
			jogoSalvo = (JogoSalvo) formatador.Deserialize(arquivo);
			// TODO: carregar imagem
		} finally {
			arquivo.Close();
		}
		return jogoSalvo;
	}
	
	public static void GravarJogoSalvo(JogoSalvo jogoSalvo, string enderecoArquivo) {
		FileStream arquivo = new FileStream(enderecoArquivo, FileMode.OpenOrCreate, FileAccess.Write);
		try {
			BinaryFormatter formatador = new BinaryFormatter();
			formatador.Serialize(arquivo, jogoSalvo);
		} finally {
			arquivo.Close();
		}
	}

	public static JogoSalvoJogador GerarJogadorSalvo() {
		JogoSalvoJogador jogadorSalvo = new JogoSalvoJogador();
		jogadorSalvo.jogador = MotorJogo.idJogador;
		jogadorSalvo.rotuloJogador = MotorJogo.rotuloJogador;
		if (Radar.jogador == null) {
			MorteJogador jogadorMorto = Camera.main.GetComponent<CameraJogador>().jogador.GetComponent<MorteJogador>();
			jogadorSalvo.posicao = new Vetor3(jogadorMorto.posicao);
			jogadorSalvo.rotacao = new Vetor3(jogadorMorto.rotacao);
		} else {
			jogadorSalvo.posicao = new Vetor3(Radar.jogador.position);
			jogadorSalvo.rotacao = new Vetor3(Radar.jogador.rotation.eulerAngles);
		}
		jogadorSalvo.pontos = Atributos.pontos;
		jogadorSalvo.energia = Atributos.energia;
		jogadorSalvo.vidas = Atributos.vidas;
		jogadorSalvo.temperatura = Atributos.temperatura;
		jogadorSalvo.bomba = Atributos.bomba;
		return jogadorSalvo;
	}
	
	public static void SalvarJogoAtual() {
		if (MotorJogo.multiJogador && MotorJogo.servidor) {
			MotorJogo.conversacao.RPC("SalvarJogoRemoto", RPCMode.OthersBuffered);
			return;
		}

		SalvarJogoAtual(null);
	}

	public static void SalvarJogoAtual(ArquivoJogo.JogoSalvoJogador jogadorRemoto) {
		if (MotorJogo.multiJogador && MotorJogo.servidor && jogadorRemoto == null) {
			MotorJogo.conversacao.RPC("SalvarJogoRemoto", RPCMode.OthersBuffered);
			return;
		}

		JogoSalvo jogoSalvo = new JogoSalvo();
		jogoSalvo.data = DateTime.Now;
		jogoSalvo.multiJogador = MotorJogo.multiJogador;
		jogoSalvo.dificuldade = MotorJogo.dificuldade;
		jogoSalvo.fase = MotorJogo.fase;
		
		jogoSalvo.jogadorLocal = GerarJogadorSalvo();
		jogoSalvo.jogadorRemoto = jogadorRemoto;
		
		string arquivo = null;
		
		for (int i = 0; i < 10; i++) {
			string arquivoAtual = "JogoSalvo" + i + ".save";
			if (!File.Exists(arquivoAtual)) {
				arquivo = arquivoAtual;
				jogoSalvo.imagem = "JogoSalvo" + i + ".png";
				break;
			}
		}
		if (arquivo == null) {
			string ultimoArquivo = null;
			DateTime ultimaData = DateTime.Now;
			List<JogoSalvo> jogosSalvos = ListarJogosSalvos();
			for (int i = 0; i < 10; i++) {
				arquivo = "JogoSalvo" + i + ".save";
				JogoSalvo jogoSalvoLocal = CarregarJogoSalvo(arquivo);
				if (jogoSalvoLocal.data < ultimaData) {
					ultimoArquivo = arquivo;
					ultimaData = jogoSalvoLocal.data;
					jogoSalvo.imagem = "JogoSalvo" + i + ".png";
				}
			}
			arquivo = ultimoArquivo;
		}
		if (arquivo != null) {
			GravarJogoSalvo(jogoSalvo, arquivo);
			Application.CaptureScreenshot(jogoSalvo.imagem);
		}
	}
	
	public static void RegistrarEvento(string enderecoArquivo, string texto) {
		StreamWriter arquivo = new StreamWriter(enderecoArquivo, true);
		try {
			arquivo.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "] (" + (MotorJogo.multiJogador ? (MotorJogo.servidor ? "servidor" : "cliente") : "mono")  + "): " + texto);
		} finally {
			arquivo.Close();
		}
	}
	
	[Serializable]
	public class JogoSalvoJogador /*: ISerializable*/ {
		public int jogador;
		public string rotuloJogador;
		public Vetor3 posicao;
		public Vetor3 rotacao;
		public int pontos;
		public int energia;
		public int vidas;
		public int temperatura;
		public int bomba;
	}
	
	[Serializable]
	public class JogoSalvo /*: ISerializable*/ {
		public DateTime data;
		public bool multiJogador;
		public int dificuldade;
		public int fase;
		public string imagem;
		public JogoSalvoJogador jogadorLocal;
		public JogoSalvoJogador jogadorRemoto;
		// TODO: salvar anéis passados
		// TODO: guardar inimigos, inimigosGrudados, chefão, disparos?, efeitos?
	}
	
	[Serializable]
	public class Vetor3 /*: ISerializable*/ {
		public float x;
		public float y;
		public float z;
		
		public Vetor3(Vector3 vetor) {
			this.x = vetor.x;
			this.y = vetor.y;
			this.z = vetor.z;
		}
		
		public Vector3 ToVector3() {
			return new Vector3(x, y, z);
		}
	}
}

