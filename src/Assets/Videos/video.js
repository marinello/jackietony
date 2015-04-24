

function Start() {
			renderer.material.mainTexture.Play();
			renderer.material.mainTexture.loop = false;

}

function Update () {
	if (renderer.material.mainTexture.isPlaying == false || Input.GetButtonDown ("Jump")) {
	
	Application.LoadLevel("MenuInicial");
		
	}

}