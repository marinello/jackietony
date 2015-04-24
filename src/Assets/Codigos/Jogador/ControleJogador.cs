using UnityEngine;
using System.Collections;

public class ControleJogador : MonoBehaviour {
	public Transform jogadorMorto;
	
	
	private float velocidade = 40;
	private float rotacaoHorizontal = 0;
	private float rotacaoVertical = 0;
	private float rotacaoGiro = 0;
	private float ultimaMorte = 0;
	private float ultimoGiro = 0;
	private bool morto = false;
	
	private static readonly float limiteMouse = 5;
	private static readonly float velocidadeHorizontal = 20;
	private static readonly float velocidadeVertical = 15;
	private static readonly float velocidadeRotacao = 30;
	private static readonly float amortecimentoRotacao = 20;
	private static readonly float amortecimentoHorizontal = 5;
	private static readonly float amortecimentoVertical = 5;
	private static readonly float anguloMaximoVertical = 60;
	private static readonly float anguloMinimoVertical = 45;
	private static readonly float tempoInvulnerabilidade = 3;
	private static readonly float velocidadeGiro = 30;
	private static readonly float intervaloGiro = 0.1f;
	
	private static readonly float velocidadeAceleracao = 2;
	private static readonly float velocidadeMaxima = 80;
	private static readonly float velocidadeMinima = 20;
	
	public void Awake() {
		if (!networkView.isMine) enabled = false;
	}
	
	public void Start() {
		ultimaMorte = Time.time;
		// TODO: Fazer brilhar para indicar invulnerabilidade
	}
	
	public void Update() {
		if (MotorJogo.estado != MotorJogo.Estado.Ligado && MotorJogo.estado != MotorJogo.Estado.EmPausa) return;
		
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (MotorJogo.estado == MotorJogo.Estado.EmPausa) {
				MotorJogo.Continuar();
			} else {
				MotorJogo.Pausar();
			}
		}
		
		if (MotorJogo.estado != MotorJogo.Estado.Ligado) return;
		
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		float aceleracao = Input.GetAxis("Acceleration");
		float giro = Input.GetAxis("Shake");

		transform.rigidbody.velocity = Vector3.zero;

		mouseX = Mathf.Clamp(mouseX, -limiteMouse, limiteMouse);
		mouseY = Mathf.Clamp(mouseY, -limiteMouse, limiteMouse);
		
		Mover(horizontal, vertical, aceleracao);
		
		rotacaoGiro += giro * Time.deltaTime * velocidadeGiro;
		if (Mathf.Abs(rotacaoGiro) > 180 / amortecimentoRotacao) rotacaoGiro -= Mathf.Sign(rotacaoGiro) * 360 / amortecimentoRotacao;
		if (giro == 0 & rotacaoGiro != 0) {
			if (Mathf.Abs(rotacaoGiro) < 1) rotacaoGiro = 0;
			else rotacaoGiro -= Mathf.Sign(rotacaoGiro);
		}
		if (giro != 0) {
			Girar();
		}
		
		Mirar(mouseX, mouseY, horizontal + rotacaoGiro);
	}
	
	public float GetVelocidade() {
		return velocidade;
	}
	
	[RPC]
	public void AplicarDano(int dano) {
		if (MotorJogo.multiJogador && (Radar.jogador == null || tag != Radar.jogador.tag)) {
			transform.networkView.RPC("AplicarDano", RPCMode.OthersBuffered, dano);
			return;
		}
		
		if (Time.time < ultimaMorte + tempoInvulnerabilidade) return;
		Atributos.energia -= dano;
		

		if (Atributos.energia <= 0 && !morto) {
			morto = true;
			if (Atributos.vidas > 0) {
				Matar(false);
			} else {
				Matar(true);
			}
		}
	}
	
	public void BateVolta() {
		rotacaoHorizontal += 180;
	}
	
	private void Mover(float horizontal, float vertical, float aceleracao) {
		velocidade = Mathf.Clamp(velocidade + aceleracao * velocidadeAceleracao, velocidadeMinima, velocidadeMaxima);
		
		Quaternion rotacaoAntiga = transform.rotation;
		transform.rotation = Quaternion.Euler(rotacaoVertical, rotacaoHorizontal, 0);
		transform.Translate(
			Vector3.forward * Time.deltaTime * velocidade +
			Vector3.right * Time.deltaTime * horizontal * velocidadeHorizontal
		);
		transform.Translate(Vector3.up * Time.deltaTime * vertical * velocidadeVertical, Space.World);
		transform.rotation = rotacaoAntiga;
	}
	
	private void Mirar(float horizontal, float vertical, float giro) {
		rotacaoHorizontal += horizontal * amortecimentoHorizontal;
		if (Mathf.Abs(rotacaoHorizontal) > 360) rotacaoHorizontal -= 360 * Mathf.Sign(rotacaoHorizontal);
		rotacaoVertical = Mathf.Clamp(rotacaoVertical - vertical * amortecimentoVertical, -anguloMaximoVertical, anguloMinimoVertical);
		
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotacaoVertical, rotacaoHorizontal, -giro * amortecimentoRotacao), Time.deltaTime * velocidadeRotacao);
	}
	
	private void Matar(bool fim) {
		Transform jogador = (Transform) MotorJogo.Instanciar(jogadorMorto, transform.position, transform.rotation);
		
		GameObject[] inimigos = GameObject.FindGameObjectsWithTag("wingEnemy");
		foreach(GameObject inimigo in inimigos){
			if (inimigo != null) {
				inimigo.GetComponent<InimigoColado>().Explodir();
			}
		}
		
		MorteJogador morteJogador = jogador.GetComponent<MorteJogador>();
		morteJogador.posicao = transform.position;
		morteJogador.rotacao = transform.rotation.eulerAngles;
		morteJogador.renascer = !fim;
		
		MotorJogo.jogadoresAlvo.Remove(transform);
		MotorJogo.Destruir(gameObject);
		Camera.main.GetComponent<CameraJogador>().jogador = jogador;
		Radar.jogador = null;
	}
	
	private void Girar() {
		if (Time.time > ultimoGiro + intervaloGiro) {
			GameObject[] inimigos = GameObject.FindGameObjectsWithTag("wingEnemy");
			foreach(GameObject inimigo in inimigos){
				if (inimigo != null) {
					inimigo.GetComponent<InimigoColado>().Girar(transform.tag);
				}
			}
			ultimoGiro = Time.time;
		}
	}
}
