using UnityEngine;
using System.Collections;

public class MotorFase3 : MotorFaseGenerico {
	public void Awake() {
		MotorJogo.rotacaoInicial = Quaternion.identity.eulerAngles;
		MotorJogo.posicaoInicial = new Vector3(50, 20, -5);
	}
}
