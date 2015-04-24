using UnityEngine;
using System.Collections;

public class FimJogo : MonoBehaviour {

	public GUISkin aparencia;

	void  OnGUI (){
		GUI.skin = aparencia;
		
		// 1056 x 594
		
		GUI.Label(
			new Rect(
				Screen.width * 3 / 5,
				Screen.height / 7,
				400,
				500
			),
			"Game Over"
		);

		GUILayout.BeginArea(
			new Rect(
				Screen.width * 3 / 5,
				Screen.height * 3 / 4,
				300,
				50
			)
		);
		if (GUILayout.Button("Voltar ao Menu")) {
			Application.LoadLevel("MenuInicial");
		}
		GUILayout.EndArea();
	}
}