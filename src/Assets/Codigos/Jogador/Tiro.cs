using UnityEngine;
using System.Collections;

public class Tiro : MonoBehaviour {
	public int dano = 5;
	public int pontos = 5;
	public float distanciaMaxima = 150;
	public bool bomba = false;
	public float raioExplosao = 5;
	public Transform explosao;
	public float tempoExpiracao = 30;
	public bool fogo = false;
	public bool gelo = false;
	
	private Vector3 posicaoInicial;
	private float tempo = 0;
	private bool destruido = false;
	
	
	public void Awake() {
		if (!networkView.isMine) enabled = false;
	}
	
	public void Start() {
		tempo = Time.time;
		posicaoInicial = transform.position;
	}
	
	public void Update() {
		if (MotorJogo.estado != MotorJogo.Estado.Ligado) return;
		
		if (Vector3.Distance(posicaoInicial, transform.position) > distanciaMaxima || Time.time - tempo > tempoExpiracao) {
			ExplodirAgora(transform.position, transform.rotation);
		}
		if (destruido && Time.time - tempo > 0.1f) {
			MotorJogo.Destruir(gameObject);
		}
	}
	
	public void OnCollisionEnter(Collision colisao) {
		if (!networkView.isMine || colisao.gameObject.tag.StartsWith("player") || colisao.gameObject.tag == "Bullet" || colisao.gameObject.tag == "bomb" || colisao.gameObject.tag.StartsWith("anel")) return;
	
		if (bomba) {
			ContactPoint contato = colisao.contacts[0];
			ExplodirAgora(contato.point, Quaternion.FromToRotation(Vector3.up, contato.normal));
		} else {
			if (colisao.gameObject.tag == "enemy") Acerta(colisao.gameObject);
			destruido = true;
		}
	}
	
	private void ExplodirAgora(Vector3 posicao, Quaternion rotacao) {
		if (bomba) {
			MotorJogo.Instanciar(explosao, posicao, rotacao);
			ParticleEmitter emissor = GetComponentInChildren<ParticleEmitter>();
			if (emissor != null) emissor.emit = false;
			
			Collider[] colisoes = Physics.OverlapSphere(transform.position, raioExplosao);
			foreach (Collider colisaoBomba in colisoes) {
				if (colisaoBomba != null && colisaoBomba.gameObject.tag == "enemy") Acerta(colisaoBomba.gameObject);
			}
		}
		destruido = true;
	}
	
	private void Acerta(GameObject inimigo) {
		if (inimigo != null) {
			if (fogo) inimigo.GetComponent<InimigoPadrao>().BotarFogo(); // TODO: passar pontos para servidor, para garantir que não contabilize pontos depois que o inimigo morrer
			else if (gelo) inimigo.GetComponent<InimigoPadrao>().Congelar();
			else inimigo.GetComponent<InimigoPadrao>().AplicarDano(dano);
			Atributos.pontos += pontos;
		}
	}
}
