using UnityEngine;
using System.Collections;

public class CCTVPlayerDetection : MonoBehaviour 
{
	private GameObject player;                          // Reference to the player.//Referendcia el jugador
	private LastPlayerSighting lastPlayerSighting;      // Reference to the global last sighting of the player.//Referencia el ultimo avistamiento global del jugador.
	
	
	void Awake ()
	{
		// Setting up the references.
		//Configura las referencias
		player = GameObject.FindGameObjectWithTag(Tags.player);
		//obtiene la referencia del script LastPlayerSighting de gamecontroller a traves del script tags.
		lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
	}
	
	
	void OnTriggerStay (Collider other)
	{
		// If the colliding gameobject is the player...
		// Si el objeto colisionado es el jugador...
		if(other.gameObject == player)
		{
			/*Necesitamos cuidado en este punto, podria ser que nuestro jugador se encuentre en el otro lado del muro
			pero todavia se intersecte su collider, para resolverlo utilizaremos raycast de la lente de la camara
			 hacia el jugador y comprobaremos la linea de vision */
			// ... raycast from the camera towards the player.
			//... emiterayo de la camara hacia el jugador.
			Vector3 relPlayerPos = player.transform.position - transform.position;
			RaycastHit hit;
			
			if(Physics.Raycast(transform.position, relPlayerPos, out hit))
				// If the raycast hits the player...//Si Raycast alcanzo al jugador
				if(hit.collider.gameObject == player)
					// ... set the last global sighting of the player to the player's position.
					//... establezaca el ultimo avistamiento global de el jugador con la posicion del jugador
					lastPlayerSighting.position = player.transform.position;
		}
	}
}
