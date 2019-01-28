using UnityEngine;
using System.Collections;

public class LaserSwitchDeactivation : MonoBehaviour 
{
	public GameObject laser;                // Reference to the laser that can we turned off at this switch.//Referencia para el laser cuando lo apagamos con este suiche.
	public Material unlockedMat;            // The screen's material to show the laser has been unloacked.//El material de la pantalla para mostrar que el laser a sido desbloqueado.
	
	private GameObject player;              // Reference to the player.//referencia al jugador.
	
	
	void Awake ()
	{
		// Setting up the reference.
		//configura la referencia.
		player = GameObject.FindGameObjectWithTag(Tags.player);
	}
	
	
	void OnTriggerStay (Collider other)
	{
		// If the colliding gameobject is the player...
		//Si el Colider del gameobject es el jugador...
		if(other.gameObject == player)
			// ... and the switch button is pressed...
			//... y el boton del suiche es presionado...
			if(Input.GetButton("Switch"))
				// ... deactivate the laser.//Desactiva el laser.
				LaserDeactivation();
	}
	
	
	void LaserDeactivation ()
	{
		// Deactivate the laser GameObject.//Desactiva el objeto de juego Laser.
		laser.SetActive(false);
		
		// Store the renderer component of the screen.
		//Almacena el componente de render de la pantalla.
		Renderer screen = transform.Find("prop_switchUnit_screen").renderer;
		
		// Change the material of the screen to the unlocked material.
		//Cambia el material de la pantalla para el material desbloqueado.
		screen.material = unlockedMat;
		
		// Play switch deactivation audio clip.
		//Reproduce el clip de audio de desactivacion de suiche.
		audio.Play();
	}
}
