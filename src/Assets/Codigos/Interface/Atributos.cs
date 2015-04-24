using UnityEngine;
using System.Collections;

public class Atributos : MonoBehaviour {
	
	public static string rotuloJogador = "Jogador";
	public static int pontos = 0;
	public static int energia = 100;
	public static int vidas = 3;
	public static int temperatura = 0;
	public static int bomba = 0;
	public static Rect areaRadar;
	public static int primeiroAnel = -1;
	public static int ultimoAnel = 0;
	public static int ultimaPontuacaoAnel = 0;
	
	public GUISkin aparenciaMenu;
	public GUISkin aparenciaAtributos;
	public Texture2D fundoCarregando;
	public Texture2D fundoMenu;
	public Texture2D[] imagensEnergia;
	public Texture2D imagemVida;
	public Texture2D[] imagensTemperatura;
	public Texture2D[] imagensBombas;
	public Texture2D imagemRadar;
	
	public void OnGUI() {
		if (MotorJogo.estado == MotorJogo.Estado.Ligado) {
			GUI.skin = aparenciaAtributos;
			GUI.Label(
				new Rect(10, 10, 400, 30), // TODO: calcular tamanho
				rotuloJogador
			);
			
			energia = Mathf.Clamp(energia, 0, 100);
			Texture2D imagemEnergia = imagensEnergia[energia * imagensEnergia.Length / 101];
			GUI.DrawTexture(
				new Rect(0, 25, Screen.width * 4 / 10, Screen.height / 5),
				imagemEnergia,
				ScaleMode.ScaleToFit
			);
			
			vidas = Mathf.Clamp(vidas, 0, 3);
			for (int i = 0; i < vidas; i++) {
				GUI.DrawTexture(
					new Rect(Screen.width - (Screen.width / 15) * (3 - i) - 10, 10, Screen.width / 15, Screen.height / 10),
					imagemVida,
					ScaleMode.ScaleToFit
				);
			}
			
			GUI.Label(
				new Rect((Screen.width * 4 / 5 - 660) / 2 + 350, 10, 300, 30), // TODO: calcular tamanho e centralizar
				"Pontos " + pontos
			);
			
			temperatura = Mathf.Clamp(temperatura, 0, 100);
			Texture2D imagemTemperatura = imagensTemperatura[temperatura * imagensTemperatura.Length / 101];
			GUI.DrawTexture(
				new Rect(10, Screen.height - Screen.height / 5 - 10, Screen.height / 5, Screen.height / 5),
				imagemTemperatura,
				ScaleMode.ScaleToFit
			);
			
			GUI.DrawTexture(
				new Rect(20 + Screen.height / 5, Screen.height - Screen.height / 8 - 10, Screen.height / 8, Screen.height / 8),
				imagensBombas[bomba],
				ScaleMode.ScaleToFit
			);
			
			areaRadar = new Rect(20 + Screen.height / 20, Screen.height * 2 / 3 - 10, Screen.height / 4, Screen.height / 4);
			GUI.DrawTexture(
				areaRadar,
				imagemRadar,
				ScaleMode.ScaleToFit
			);
		} else if (MotorJogo.estado == MotorJogo.Estado.Carregando || MotorJogo.estado == MotorJogo.Estado.Conectando) {
			GUI.skin = aparenciaMenu;
			MenuInicial.Carregando(fundoCarregando);
		} else if (MotorJogo.estado == MotorJogo.Estado.EmPausa) {
			GUI.skin = aparenciaMenu;
			
			if (fundoMenu != null) {
				GUI.DrawTexture(
					new Rect(
						(Screen.width - fundoMenu.width) / 2,
						(Screen.height - fundoMenu.height) / 2,
						fundoMenu.width,
						fundoMenu.height
					),
					fundoMenu
				);
			}

			GUILayout.BeginArea(
				new Rect(
					(Screen.width - 250) / 2,
					(Screen.height - 200) / 2,
					250,
					250
				)
			);
			
			bool podeSalvar = !MotorJogo.multiJogador || Network.isServer;
			
			int tamanho = podeSalvar ? 5 : 4;

			GUILayout.BeginArea(new Rect(0, 0, 250, 60));
			if (GUILayout.Button("Retornar ao Jogo")) {
				MotorJogo.Continuar();
			}
			GUILayout.EndArea();

			GUILayout.BeginArea(new Rect(0, 250 / tamanho, 250, 60));
			if (GUILayout.Button("Reiniciar Jogo")) {
				MotorJogo.Reiniciar();
			}
			GUILayout.EndArea();

			if (podeSalvar) {
				GUILayout.BeginArea(new Rect(0, 500 / tamanho, 250, 60));
				if (GUILayout.Button("Salvar Jogo")) {
					ArquivoJogo.SalvarJogoAtual();
				}
				GUILayout.EndArea();
			}

			GUILayout.BeginArea(new Rect(0, (podeSalvar ? 750 : 500) / tamanho, 250, 60));
			if (GUILayout.Button("Sair do Jogo")) {
				MotorJogo.MenuInicial();
			}
			GUILayout.EndArea();

			GUILayout.EndArea();
		}
	}
}
