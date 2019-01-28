using UnityEngine;
using System.Collections;
//Entendido
public class EnemySight : MonoBehaviour 
{
	public float fieldOfViewAngle = 110f;           // Number of degrees, centred on forward, for the enemy see.
	public bool playerInSight;                      // Whether or not the player is currently sighted.
	public Vector3 personalLastSighting;            // Last place this enemy spotted the player.
	
	
	private NavMeshAgent nav;                       // Reference to the NavMeshAgent component.
	private SphereCollider col;                     // Reference to the sphere collider trigger component.
	private Animator anim;                          // Reference to the Animator.
	private LastPlayerSighting lastPlayerSighting;  // Reference to last global sighting of the player.
	private GameObject player;                      // Reference to the player.
	private Animator playerAnim;                    // Reference to the player's animator component.
	private PlayerHealth playerHealth;              // Reference to the player's health script.
	private HashIDs hash;                           // Reference to the HashIDs.
	private Vector3 previousSighting;               // Where the player was sighted last frame.
	
	
	void Awake ()
	{
		// Setting up the references.
		nav = GetComponent<NavMeshAgent>();
		col = GetComponent<SphereCollider>();
		anim = GetComponent<Animator>();
		lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
		player = GameObject.FindGameObjectWithTag(Tags.player);
		playerAnim = player.GetComponent<Animator>();
		playerHealth = player.GetComponent<PlayerHealth>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
		
		// Set the personal sighting and the previous sighting to the reset position.
		personalLastSighting = lastPlayerSighting.resetPosition;
		previousSighting = lastPlayerSighting.resetPosition;
	}
	
	
	void Update ()
	{
		// If the last global sighting of the player has changed...
		// Si el ultimo avistamiento global del jugador ha cambiado...
		if(lastPlayerSighting.position != previousSighting)
			// ... then update the personal sighting to be the same as the global sighting.
			// ... Entonces actualice el avistamiento personal para que sea el mismo que el avistamiento global.
			personalLastSighting = lastPlayerSighting.position;
		
		// Set the previous sighting to the be the sighting from this frame.
		// Establezca el avistamiento previo para que sea el avistamiento de este marco.
		previousSighting = lastPlayerSighting.position;
		
		// If the player is alive...
		// si el jugador sigue vivo...
		if(playerHealth.health > 0f)
			// ... set the animator parameter to whether the player is in sight or not.
			// ... establezca el parametro del animador si el jugador esta a la vista o no.
			anim.SetBool(hash.playerInSightBool, playerInSight);
		else
			// ... set the animator parameter to false.
			// ... Establezca el parametro del animator en falso.
			anim.SetBool(hash.playerInSightBool, false);
	}
	
	
	void OnTriggerStay (Collider other)
	{
		// If the player has entered the trigger sphere...
		// si el jugador ha entrado en la esfera detonadora...
		if(other.gameObject == player)
		{
			// By default the player is not in sight.
			// Por defecto el jugador no ha sido avistado
			playerInSight = false;
			
			// Create a vector from the enemy to the player and store the angle between it and forward.
			// Cree un vector del enemiy para el jugador y almacene el angulo entre ellos y la direccion haciadelante.
			Vector3 direction = other.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);
			
			// If the angle between forward and where the player is, is less than half the angle of view...
			// si el angulo entre adelante y donde esta el jugador es, es menor que la mitad del angulo de vista...
			if(angle < fieldOfViewAngle * 0.5f)
			{
				RaycastHit hit;
				
				// ... and if a raycast towards the player hits something...
				//... y si el rayo hacia el jugador golpea alguna cosa...
				if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))
				{
					// ... and if the raycast hits the player...
					//... y si el rayo golpea al jugador...
					if(hit.collider.gameObject == player)
					{
						// ... the player is in sight.
						// ... el jugador es avistado.
						playerInSight = true;
						
						// Set the last global sighting is the players current position.
						// establezca el ultimo avistamiento global en la posicion actual del jugador.
						lastPlayerSighting.position = player.transform.position;
					}
				}
			}
			
			// Store the name hashes of the current states.
			// Almacene los nombres de los hashes del actual estado
			int playerLayerZeroStateHash = playerAnim.GetCurrentAnimatorStateInfo(0).nameHash;//locomotion corriendo
			int playerLayerOneStateHash = playerAnim.GetCurrentAnimatorStateInfo(1).nameHash;//shouting gritando
			
			// If the player is running or is attracting attention...
			//si el jugador esta corriendo o esta atrayendo la atencion...//Se compara el hasid actual que se esta ejecutando con el que deseo consultar.
			if(playerLayerZeroStateHash == hash.locomotionState || playerLayerOneStateHash == hash.shoutState)
			{
				// ... and if the player is within hearing range...
				//... si el jugador esta dentro del rango de escucha...
				if(CalculatePathLength(player.transform.position) <= col.radius)
					// ... set the last personal sighting of the player to the player's current position.
					//... establezca el ultimo avistamiento personal del jugador en la posicion actual del jugador.
					personalLastSighting = player.transform.position;
			}
		}
	}
	
	
	void OnTriggerExit (Collider other)
	{
		// If the player leaves the trigger zone...
		// si el jugador deja la zona de trigger...
		if(other.gameObject == player)
			// ... the player is not in sight.
			// ... el jugador no ha sido visto.
			playerInSight = false;
	}
	
	
	float CalculatePathLength (Vector3 targetPosition)
	{
		// Create a path and set it based on a target position.
		// Crear una ruta y establecezca esta basada en una posicion de destino.
		NavMeshPath path = new NavMeshPath();
		if(nav.enabled)
			nav.CalculatePath(targetPosition, path);
		
		// Create an array of points which is the length of the number of corners in the path + 2// para permitir la posicion del jugador y la del enemigo.
		// cree un array de puntos de la longitud del numero de esquinas en la ruta + 2 que almacenan la posicion del jugador y del enemigo.
		Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];
		
		// The first point is the enemy's position.
		// El primer puntos es la posicion del enemigo.
		allWayPoints[0] = transform.position;
		
		// The last point is the target position.
		// el ultimo punto es la posicion del objetivo.
		allWayPoints[allWayPoints.Length - 1] = targetPosition;
		
		// The points inbetween are the corners of the path.
		// Los puntos intermedios son las esquinas de la ruta
		for(int i = 0; i < path.corners.Length; i++)
		{
			allWayPoints[i + 1] = path.corners[i];
		}
		
		// Create a float to store the path length that is by default 0.
		// cree un flotante para almacenar la longitud de la ruta que es por defecto cero.
		float pathLength = 0;
		
		// Increment the path length by an amount equal to the distance between each waypoint and the next.
		// Incremente la longitud de la ruta en una cantidad igual a la distancia entre cada punto de paso y el siguiente.
		for(int i = 0; i < allWayPoints.Length - 1; i++)
		{
			pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
		}
		
		return pathLength;
	}
}
