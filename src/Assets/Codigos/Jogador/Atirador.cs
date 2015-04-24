using UnityEngine;
using System.Collections;

public class Atirador : MonoBehaviour {
	public Transform arma;
	public Transform tiro;
	public Transform[] bombas; // basica, forte, fogo, gelo, luz
	
	private ControleJogador controleJogador;
	private float ultimoTiro = 0;
	private float ultimoResfriamento = 0;
	private float ultimaBomba = 0;
	
	private static readonly float velocidadeTiro = 300;
	private static readonly float intervaloTiro = 0.1f;
	private static readonly float intervaloAquecimento = 1;
	private static readonly float velocidadeResfriamento = intervaloTiro / 2;
	private static readonly int velocidadeBomba = 150;
	private static readonly float intervaloBomba = 2;
	
	void Awake() {
		if(!networkView.isMine) enabled = false;	
	}
	
	void Start() {
		controleJogador = GetComponent<ControleJogador>();
	}
	
	public void Update() {
		if (MotorJogo.estado != MotorJogo.Estado.Ligado) return;
		
 		bool atirar = Input.GetButton("Fire1");
		bool bombardear = Input.GetButton("Fire2");
		
		if(Atributos.temperatura > 0) {
			if(Time.time > ultimoTiro + intervaloAquecimento && Time.time > ultimoResfriamento + velocidadeResfriamento) {
				Atributos.temperatura--;
				ultimoResfriamento = Time.time;
				//SendTemperatureGun();
			}
		}
		
		if (atirar && Time.time > ultimoTiro + intervaloTiro && Atributos.temperatura < 100) {
			Transform tiroLocal = (Transform) MotorJogo.Instanciar(tiro, arma.position, transform.rotation);
			tiroLocal.tag = tag.Substring(6);
			tiroLocal.rigidbody.AddForce(transform.forward * velocidadeTiro * controleJogador.GetVelocidade());
			ultimoTiro = Time.time;
			Atributos.temperatura++;
			//SendTemperatureGun();
		}
		
		if (bombardear && Time.time > ultimaBomba + intervaloBomba) {
			Transform bomba = (Transform) MotorJogo.Instanciar(bombas[Atributos.bomba], arma.position, transform.rotation);
			bomba.tag = tag.Substring(6);
			bomba.rigidbody.AddForce(transform.forward * velocidadeBomba * controleJogador.GetVelocidade());
			ultimaBomba = Time.time;
			// TODO: implementar limite de bombas
		}
	}
}
