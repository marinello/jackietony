using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

class MenuInicial : MonoBehaviour {
	
	public GUISkin aparencia = null;
	public Texture2D fundo = null;
	public Texture2D[] imagensJogadores = null;
	public Transform[] jogadores = null;
	public Transform conversacao = null;
	
	private Menu menuAtual;
	private int jogador = 0;
	private int dificuldade = 0;
	private string[] rotulosJogosSalvos;
	private ArquivoJogo.JogoSalvo[] jogosSalvos;
	private Texture2D[] imagensJogosSalvos;
	private int jogoSalvo;
	private string enderecoServidor = "localhost";
	
	private static readonly string[] rotuloJogadores = new string[]{"Jackie Gambino", "Tony Barrow"};
	private static readonly string[] rotuloDificuldades = new string[]{"Facil", "Medio", "Dificil"};
	
	public MenuInicial() {
		Menu menuInicial = new Menu();
		Menu menuJogo = new Menu();
		Menu menuCarregar = new Menu(PintarJogosSalvos, 1);
		Menu menuJogoMono = new Menu(PintarJogador);
		Menu menuJogoMulti = new Menu();
		Menu menuJogoMultiCliente = new Menu(PintarServidor, 1);
		Menu menuJogoMultiServidor = new Menu(PintarJogador);
		
		menuInicial.itens.Add(new ItemMenu("Iniciar Jogo", menuJogo));
		menuInicial.itens.Add(new ItemMenu("Carregar Jogo", menuCarregar, ListarJogosSalvos));
		menuInicial.itens.Add(new ItemMenu("Sair do Jogo", Sair));
		
		menuJogo.itens.Add(new ItemMenu("Unico Jogador", menuJogoMono, RedefinirJogador));
		menuJogo.itens.Add(new ItemMenu("Multi-Jogador", menuJogoMulti));
		menuJogo.itens.Add(new ItemMenu("Voltar", menuInicial));
		
		menuCarregar.itens.Add(new ItemMenu("Carregar", CarregarJogo));
		menuCarregar.itens.Add(new ItemMenu("Voltar", menuInicial));
		
		menuJogoMono.itens.Add(new ItemMenu("Jogador: " + rotuloJogadores[jogador], AlterarJogador));
		menuJogoMono.itens.Add(new ItemMenu("Dificuldade: " + rotuloDificuldades[dificuldade], AlterarDificuldade));
		menuJogoMono.itens.Add(new ItemMenu("Iniciar Jogo", IniciarJogo));
		menuJogoMono.itens.Add(new ItemMenu("Voltar", menuJogo));
		
		menuJogoMulti.itens.Add(new ItemMenu("Conectar ao Servidor", menuJogoMultiCliente));
		menuJogoMulti.itens.Add(new ItemMenu("Iniciar Servidor", menuJogoMultiServidor, RedefinirJogador));
		menuJogoMulti.itens.Add(new ItemMenu("Voltar", menuJogo));
		
		menuJogoMultiCliente.itens.Add(new ItemMenu("Conectar", ConectarServidor));
		menuJogoMultiCliente.itens.Add(new ItemMenu("Voltar", menuJogoMulti));
		
		menuJogoMultiServidor.itens.Add(menuJogoMono.itens[0]);
		menuJogoMultiServidor.itens.Add(menuJogoMono.itens[1]);
		menuJogoMultiServidor.itens.Add(new ItemMenu("Iniciar Servidor", IniciarServidor));
		menuJogoMultiServidor.itens.Add(new ItemMenu("Voltar", menuJogoMulti));
		
		menuAtual = menuInicial;
	}
	
	public void OnGUI() {
		GUI.skin = aparencia;
		
		if (MotorJogo.estado == MotorJogo.Estado.Carregando || MotorJogo.estado == MotorJogo.Estado.Conectando) {
			Carregando(fundo);
			return;
		}
		
		if (fundo != null) {
			GUI.DrawTexture(
				new Rect(
					0,
					0,
					Screen.width,
					Screen.height
				),
				fundo,
				ScaleMode.ScaleAndCrop
			);
		}
		
		if (menuAtual.acao != null) menuAtual.acao();
		int posicao = menuAtual.primeiro;
		foreach (ItemMenu itemMenu in menuAtual.itens) {
			GUILayout.BeginArea(new Rect(
				Screen.width - 300 - posicao * 20, // TODO: refazer posicionamento proporcional ao tamanho da tela!
				Screen.height / 2 + posicao * 45,
				300,
				50
			));
			if (GUILayout.Button(itemMenu.rotulo)) {
				if (itemMenu.destino != null) menuAtual = itemMenu.destino;
				if (itemMenu.acao != null) itemMenu.acao();
			}
			GUILayout.EndArea();
			posicao++;
		}
	}
	
	public static void Carregando(Texture2D fundo) {
		string texto;
		float tamanho;
		if (MotorJogo.estado == MotorJogo.Estado.Carregando) {
			texto = "Carregando...";
			tamanho = 300; // TODO: calcular tamanho e centralizar
		} else {
			if (MotorJogo.servidor) {
				texto = "Aguardando Cliente...";
				tamanho = 470; // TODO: calcular tamanho e centralizar
			} else {
				texto = "Conectando...";
				tamanho = 300; // TODO: calcular tamanho e centralizar
			}
		}
		
		if (fundo != null) {
			GUI.DrawTexture(
				new Rect(
					0,
					0,
					Screen.width,
					Screen.height
				),
				fundo,
				ScaleMode.ScaleAndCrop
			);
		}

		GUI.Label(
			new Rect(
				(Screen.width - tamanho) / 2,
				(Screen.height - 60) * 3 / 4,
				tamanho,
				60
			),
			texto
		);
	}
	
	private void PintarJogosSalvos() {
		
		if (rotulosJogosSalvos != null && rotulosJogosSalvos.Length > 0) {
			GUILayout.BeginArea(
				new Rect(
					Screen.width - 300,
					Screen.height / 2,
					260,
					100
				)
			);
			//jogoSalvo = GUILayout.SelectionGrid(jogoSalvo, rotulosJogosSalvos, 1);
			if (GUILayout.Button(rotulosJogosSalvos[jogoSalvo])) {
				jogoSalvo++;
				if (jogoSalvo >= rotulosJogosSalvos.Length) jogoSalvo = 0;
			}
			GUILayout.EndArea();
			GUI.DrawTexture(
				new Rect(
					Screen.width / 2,
					10,
					Screen.width / 2 - 10,
					Screen.height / 2 - 10
				),
				imagensJogosSalvos[jogoSalvo],
				ScaleMode.ScaleToFit
			);
		} else {
			GUI.Label(
				new Rect(
					Screen.width - 300,
					Screen.height / 2,
					260,
					100
				),
				"Nenhum\nJogo salvo"
			);
		}
	}
	
	private void PintarJogador() {
		GUI.DrawTexture(
			new Rect(
				(Screen.width - imagensJogadores[jogador].width) - 50,
				Mathf.Clamp(Screen.height / 2 - imagensJogadores[jogador].height, 0, Screen.height / 2),
				imagensJogadores[jogador].width,
				imagensJogadores[jogador].height
			),
			imagensJogadores[jogador]
		);
	}
	
	private void PintarServidor() {
		GUILayout.BeginArea(
			new Rect(
				Screen.width - 300,
				Screen.height / 2,
				300,
				50
			)
		);
		enderecoServidor = GUILayout.TextField(enderecoServidor, GUILayout.MinWidth(150));
		GUILayout.EndArea();
	}
	
	private void ListarJogosSalvos() {
		List<ArquivoJogo.JogoSalvo> listaJogosSalvos = ArquivoJogo.ListarJogosSalvos();
		List<string> listaRotulos = new List<string>();
		List<Texture2D> listaImagens = new List<Texture2D>();
		
		foreach (ArquivoJogo.JogoSalvo jogoSalvoLocal in listaJogosSalvos) {
			listaRotulos.Add((jogoSalvoLocal.multiJogador ? "Multi" : "Solo") + " - " + jogoSalvoLocal.data.ToString("dd/MM/yyyy HH:mm"));
			WWW www = new WWW("file://" + Application.dataPath + "/../" + jogoSalvoLocal.imagem);
			CarregarImagem(www);
			listaImagens.Add(www.texture);
		}
		
		jogosSalvos = listaJogosSalvos.ToArray();
		rotulosJogosSalvos = listaRotulos.ToArray();
		imagensJogosSalvos = listaImagens.ToArray();
	}
	
	private IEnumerable CarregarImagem(WWW www) {
		yield return www;
		if (www.error != null) Debug.LogError("ERRO WWW: " + www.error);
	}
	
	private void Sair() {
		Application.Quit();
	}
	
	private void RedefinirJogador() {
		jogador = 0;
		dificuldade = 0;
		menuAtual.itens[0].rotulo = "Jogador: " + rotuloJogadores[jogador];
		menuAtual.itens[1].rotulo = "Dificuldade: " + rotuloDificuldades[dificuldade];
	}
	
	private void CarregarJogo() {
		if (jogosSalvos == null || jogosSalvos.Length == 0) return;
		
		MotorJogo.jogadorRemoto = jogadores;
		MotorJogo.IniciarJogoSalvo(jogosSalvos[jogoSalvo]);
	}
	
	private void AlterarJogador() {
		jogador++;
		if (jogador >= rotuloJogadores.Length) jogador = 0;
		menuAtual.itens[0].rotulo = "Jogador: " + rotuloJogadores[jogador];
	}
	
	private void AlterarDificuldade() {
		dificuldade++;
		if (dificuldade >= rotuloDificuldades.Length) dificuldade = 0;
		menuAtual.itens[1].rotulo = "Dificuldade: " + rotuloDificuldades[dificuldade];
	}
	
	private void IniciarJogo() {
		MotorJogo.multiJogador = false;
		MotorJogo.primeiraFase = true;
		MotorJogo.dificuldade = dificuldade;
		MotorJogo.idJogador = jogador;
		MotorJogo.jogador = jogadores[jogador];
		MotorJogo.rotuloJogador = rotuloJogadores[jogador];
		MotorJogo.fase = 1;
		MotorJogo.IniciarJogo();
	}
	
	private void ConectarServidor() {
		MotorJogo.multiJogador = true;
		MotorJogo.servidor = false;
		MotorJogo.primeiraFase = true;
		MotorJogo.enderecoServidor = enderecoServidor;
		MotorJogo.jogadorRemoto = jogadores;
		MotorJogo.IniciarJogo();
	}
	
	private void IniciarServidor() {
		MotorJogo.multiJogador = true;
		MotorJogo.servidor = true;
		MotorJogo.primeiraFase = true;
		MotorJogo.dificuldade = dificuldade;
		MotorJogo.idJogador = jogador;
		MotorJogo.jogador = jogadores[jogador];
		MotorJogo.rotuloJogador = rotuloJogadores[jogador];
		MotorJogo.fase = 1;
		MotorJogo.rotuloJogadorRemoto = rotuloJogadores[1 - jogador];
		MotorJogo.entidadeConversacao = conversacao;
		MotorJogo.IniciarJogo();
	}
	
	private delegate void Acao();

	private class ItemMenu {
		public string rotulo;
		public Menu destino;
		public Acao acao;
		
		public ItemMenu(string rotulo, Menu destino) {
			this.rotulo = rotulo;
			this.destino = destino;
			this.acao = null;
		}
		
		public ItemMenu(string rotulo, Acao acao) {
			this.rotulo = rotulo;
			this.destino = null;
			this.acao = acao;
		}
		
		public ItemMenu(string rotulo, Menu destino, Acao acao) {
			this.rotulo = rotulo;
			this.destino = destino;
			this.acao = acao;
		}
	}
	
	private class Menu {
		public List<ItemMenu> itens;
		public Acao acao;
		public int primeiro;
		
		public Menu() {
			this.itens = new List<ItemMenu>();
			this.acao = null;
			this.primeiro = 0;
		}
		
		public Menu(Acao acao) {
			this.itens = new List<ItemMenu>();
			this.acao = acao;
			this.primeiro = 0;
		}
		
		public Menu(Acao acao, int primeiro) {
			this.itens = new List<ItemMenu>();
			this.acao = acao;
			this.primeiro = primeiro;
		}
	}
}
