using UnityEngine;
using System.Collections;

public class MotorFaseGenerico : MonoBehaviour {
	public void Start() {
		if (MotorJogo.multiJogador) {
			MotorJogo.IniciarMultiJogo();
		} else {
			MotorJogo.JogoIniciado();
		}
		// TODO: Remover inimigos que não pertencem a dificuldade selecionada
	}
	
	public void OnServerInitialized() {
		MotorJogo.PrepararJogo();
	}
	
	public IEnumerator OnConnectedToServer() {
		if (MotorJogo.primeiraFase) {
			GameObject elemento;
			while ((elemento = GameObject.FindWithTag("GameController")) == null) yield return new WaitForSeconds(0.1f);
			MotorJogo.conversacao = elemento.transform.networkView;
		}
		MotorJogo.PrepararJogo();
	}
	
	public void OnFailedToConnect(NetworkConnectionError erro) {
		MotorJogo.MenuInicial();
	}
	
	public void OnFailedToConnectToMasterServer(NetworkConnectionError erro) {
		MotorJogo.MenuInicial();
	}
	
	public void OnPlayerDisconnected(NetworkPlayer jogador) {
		Network.RemoveRPCs(jogador);
		Network.DestroyPlayerObjects(jogador);
		MotorJogo.MenuInicial();
	}
	
	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		MotorJogo.MenuInicial();
	}
}