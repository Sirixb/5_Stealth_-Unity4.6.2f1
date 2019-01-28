using UnityEngine;
using System.Collections;

public class SceneFadeInOut : MonoBehaviour 
{
	public float fadeSpeed = 1.5f;          // Speed that the screen fades to and from black.//Velocidad con que se desvanece la pantalla para y de negro.
	
	
	private bool sceneStarting = true;      // Whether or not the scene is still fading in.// si la escena se esta desvaneciendo o no.
	
	
	void Awake ()
	{
		// Set the texture so that it is the the size of the screen and covers it.
		//Ajusta la textura de manera que es la del tamaño de la pantalla y la cubre.
		guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
	}
	
	
	void Update ()
	{
		// If the scene is starting...
		// Si la escena inicio...
		if(sceneStarting)
			// ... call the StartScene function.
			//... llama a la funcion StarScene.
			StartScene();
	}
	
	
	void FadeToClear ()//sub de StarScene
	{
		// Lerp the colour of the texture between itself and transparent.
		// Lerp interpola el color de la textura entre simisma y la transparencia.
		guiTexture.color = Color.Lerp(guiTexture.color, Color.clear, fadeSpeed * Time.deltaTime);
	}
	
	
	void FadeToBlack ()//
	{
		// Lerp the colour of the texture between itself and black.
		// Lerp interpola el color de la textura entre simisma y el negro.
		guiTexture.color = Color.Lerp(guiTexture.color, Color.black, fadeSpeed * Time.deltaTime);
	}
	
	
	void StartScene ()
	{
		// Fade the texture to clear.//desvanece la textura a limpia
		FadeToClear();
		
		// If the texture is almost clear...//Si la textura es casi limpia
		if(guiTexture.color.a <= 0.05f)
		{
			// ... set the colour to clear and disable the GUITexture.
			//... ajuste el color a limpio y deshabilite el GuiTexture.
			guiTexture.color = Color.clear;
			guiTexture.enabled = false;
			
			// The scene is no longer starting.// La escena ya no esta empezando
			sceneStarting = false;
		}
	}
	
	
	public void EndScene ()
	{
		// Make sure the texture is enabled.//Asegurese de que la textura esta habilitada.
		guiTexture.enabled = true;
		
		// Start fading towards black.//Inicia la decoloración hacia el negro .
		FadeToBlack();
		
		// If the screen is almost black...//La pantalla es casi negra.
		if(guiTexture.color.a >= 0.95f)
			// ... reload the level.//recarge el nivel.
			Application.LoadLevel(0);
	}
}
