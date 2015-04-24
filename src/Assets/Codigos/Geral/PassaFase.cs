using UnityEngine;
using System.Collections;

public class PassaFase : MonoBehaviour {
	public IEnumerator Start() {
		yield return new WaitForSeconds(3);
		MotorJogo.PassarFase();
	}
}
