using UnityEngine;
using System.Collections;

public class Introducao : MonoBehaviour {
	
	public Texture2D imagem;
	
	public IEnumerator Start() {
		yield return new WaitForSeconds(5);
		Application.LoadLevel("video");
	}
	
	public void OnGUI() {
		if (imagem == null) return;
		GUI.DrawTexture(
			new Rect(
				(Screen.width - imagem.width) / 2,
				(Screen.height - imagem.height) /2,
				imagem.width,
				imagem.height
			),
			imagem
		);
	}
}