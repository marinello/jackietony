using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Radar : MonoBehaviour {
	public Texture2D pontoInimigo;  
	public Texture2D pontoJogador; 
	public Texture2D pontoAmigo; 
	
	public static Transform jogador;
	
	private float larguraRadar;
	private float alturaRadar;
	private float escala;
	private Vector2 centro;
	private float ultimaAtualizacao = 0;
	private float ultimaPiscada = 0;
	private bool haInimigosColados = false;
	private List<Vetor2> inimigosProximos = new List<Vetor2>();
	private Vetor2 amigo;
	
	private static readonly float tempoAtualizacao = 1;
	private static readonly float raioRastreioRadar = 200;
	private static readonly float intervaloPiscada = 1;
	private static readonly string rotuloInimigo = "enemy";
	//private static readonly string rotuloDificuldadeFacil = "EASY";
	private static readonly string rotuloInimigoColado = "wingEnemy";
	private static readonly float larguraRadarPadrao = 150;
	
	public void OnGUI() {
		
		if (MotorJogo.estado != MotorJogo.Estado.Ligado || jogador == null) return;
		
		LocalizarMapa();
		
		bool pintaInimigoColado = false;
		
		if (haInimigosColados) {
			if (Time.time < ultimaPiscada + intervaloPiscada / 2) {
				pintaInimigoColado = true;
			} else if (Time.time > ultimaPiscada + intervaloPiscada) {
				ultimaPiscada = Time.time;
			}
		}
		
		GUI.DrawTexture(new Rect(centro.x - 4 * escala, centro.y - 4 * escala, 8 * escala, 8 * escala), pintaInimigoColado ? pontoInimigo : pontoJogador);
		
		if (Time.time > ultimaAtualizacao + tempoAtualizacao) {
			BuscarInimigosProximos();
			BuscarInimigosColados();
			if (MotorJogo.multiJogador) {
				GameObject jogadorRemoto = GameObject.FindWithTag("player" + (jogador.tag.EndsWith("Jack") ? "Tony" : "Jack"));
				if (jogadorRemoto != null) {
					amigo = CalcularPosicao(jogadorRemoto.transform);
				}
			}
			ultimaAtualizacao = Time.time;
		}
		
		foreach (Vetor2 inimigo in inimigosProximos) {
			GUI.DrawTexture(new Rect(centro.x + inimigo.x * larguraRadar / 2, centro.y + inimigo.y * alturaRadar / 2, 4 * escala, 4 * escala), pontoInimigo);
	    }
		
		if (MotorJogo.multiJogador && amigo != null) {
			GUI.DrawTexture(new Rect(centro.x + amigo.x * larguraRadar / 2, centro.y + amigo.y * alturaRadar / 2, 4 * escala, 4 * escala), pontoAmigo);
		}
		
	}
	
	private void LocalizarMapa() {
		larguraRadar = Atributos.areaRadar.width;
		alturaRadar = Atributos.areaRadar.height;
		escala = larguraRadar / larguraRadarPadrao;
		centro = new Vector2(
			Atributos.areaRadar.x + larguraRadar / 2,
			Atributos.areaRadar.y + alturaRadar / 2
		);
		
	}
	
	private void BuscarInimigosColados() {
	    GameObject[] inimigosColados = GameObject.FindGameObjectsWithTag(rotuloInimigoColado);
		bool encontrou = false;
		if (inimigosColados != null) {
			foreach (GameObject inimigoColado in inimigosColados) {
				if (inimigoColado.GetComponent<InimigoColado>().jogadorAlvoColado.tag == jogador.tag) {
					encontrou = true;
					break;
				}
			}
		}
		haInimigosColados = encontrou;
	}
	
	private void BuscarInimigosProximos() {
	    inimigosProximos.Clear();
	    
	    GameObject[] inimigos = GameObject.FindGameObjectsWithTag(rotuloInimigo);
	
	    foreach (GameObject inimigo in inimigos) {
			if (inimigo && jogador != null) {
				Vetor2 posicao = CalcularPosicao(inimigo.transform);
				
				if (posicao != null) {
					inimigosProximos.Add(posicao);
				}	
			}
	     }
	 }
	
	private Vetor2 CalcularPosicao(Transform alvo) {
		Vector3 posicaoJogador = jogador.position;
		Vector3 posicaoAlvo = alvo.position;
		float distancia = Vector3.Distance(posicaoJogador, posicaoAlvo);
		
		if (distancia > raioRastreioRadar) return null;
		
		float distanciaX = posicaoJogador.x - posicaoAlvo.x;
	    float distanciaY = posicaoJogador.z - posicaoAlvo.z;
		
		float deltaY = Mathf.Atan2(distanciaX, distanciaY) * Mathf.Rad2Deg - 270 - jogador.eulerAngles.y;
		float posicaoX = distancia * Mathf.Cos(deltaY * Mathf.Deg2Rad);
		float posicaoY = distancia * Mathf.Sin(deltaY * Mathf.Deg2Rad);
		
		return new Vetor2(posicaoX / raioRastreioRadar, posicaoY / raioRastreioRadar);
	}
	
	private class Vetor2 {
		public float x;
		public float y;
		
		public Vetor2(float x, float y) {
			this.x = x;
			this.y = y;
		}
	}
}
