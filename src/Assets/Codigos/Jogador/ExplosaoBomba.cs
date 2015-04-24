using UnityEngine;
using System.Collections;

public class ExplosaoBomba : MonoBehaviour {
	
	public void Awake() {
		if (!networkView.isMine) enabled = false;
	}
	
	public IEnumerator Start() {
		yield return new WaitForSeconds(3);
		MotorJogo.Destruir(gameObject);
	}
}
