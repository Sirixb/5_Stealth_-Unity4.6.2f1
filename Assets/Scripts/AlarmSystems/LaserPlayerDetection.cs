using UnityEngine;
using System.Collections;

public class LaserPlayerDetection : MonoBehaviour
{
	private GameObject player;                          // Reference to the player.//Referencia al jugador
	private LastPlayerSighting lastPlayerSighting;      // Reference to the global last sighting of the player.//Referencia del ultimo avistamiento global del jugador.
	
	
	void Awake ()
	{
		// Setting up references.
		//Configura las referencias a player y al gamecontroller a traves del script de tags.
		player = GameObject.FindGameObjectWithTag(Tags.player);
		//Coja el script de LastPlayerSighting a traves de S.Tags de S.Gamecontroller.
		lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
	}
	
	
	void OnTriggerStay(Collider other)
	{
		// If the beam is on...//Si el rayo esta encendido
		if(renderer.enabled)
			// ... and if the colliding gameobject is the player...
			//.. y si el colisionador del objeto de juego es el jugador..
			if(other.gameObject == player)
				// ... set the last global sighting of the player to the colliding object's position.
				//... establezca la el ultimo avistamiento global del jugador  para la posicion del objeto colisionador.
				lastPlayerSighting.position = other.transform.position;//lleva directamente informacion a el script del gamecontroller.
	}
}
