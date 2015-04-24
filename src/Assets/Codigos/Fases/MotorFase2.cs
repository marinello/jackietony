using UnityEngine;
using System.Collections;

public class MotorFase2 : MotorFaseGenerico {
	public void Awake() {
		MotorJogo.rotacaoInicial = new Vector3(10, -10, -5);
		MotorJogo.posicaoInicial = new Vector3(210, 60, 445);
	}
}
