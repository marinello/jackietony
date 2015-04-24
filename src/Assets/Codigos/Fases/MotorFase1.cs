using UnityEngine;
using System.Collections;

public class MotorFase1 : MotorFaseGenerico {	
	public void Awake() {
		MotorJogo.rotacaoInicial = Quaternion.identity.eulerAngles;
		MotorJogo.posicaoInicial = new Vector3(4, 0, -10);
	}
}
