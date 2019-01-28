using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
	public float patrolSpeed = 2f;                          // The nav mesh agent's speed when patrolling.
	public float chaseSpeed = 5f;                           // The nav mesh agent's speed when chasing.
	public float chaseWaitTime = 5f;                        // The amount of time to wait when the last sighting is reached.
	public float patrolWaitTime = 1f;                       // The amount of time to wait when the patrol way point is reached.
	public Transform[] patrolWayPoints;                     // An array of transforms for the patrol route.
	
	
	private EnemySight enemySight;                          // Reference to the EnemySight script.
	private NavMeshAgent nav;                               // Reference to the nav mesh agent.
	private Transform player;                               // Reference to the player's transform.
	private PlayerHealth playerHealth;                      // Reference to the PlayerHealth script.
	private LastPlayerSighting lastPlayerSighting;          // Reference to the last global sighting of the player.
	private float chaseTimer;                               // A timer for the chaseWaitTime.
	private float patrolTimer;                              // A timer for the patrolWaitTime.
	private int wayPointIndex;                              // A counter for the way point array.
	
	
	void Awake ()
	{
		// Setting up the references.
		// configuracion de las referencias.
		enemySight = GetComponent<EnemySight>();
		nav = GetComponent<NavMeshAgent>();
		player = GameObject.FindGameObjectWithTag(Tags.player).transform;
		playerHealth = player.GetComponent<PlayerHealth>();
		lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
	}
	
	
	void Update ()
	{
		// If the player is in sight and is alive...
		// Si el jugador esta a la vista y esta vivo
		if(enemySight.playerInSight && playerHealth.health > 0f)
			// ... shoot.
			//dispare
			Shooting();
		
		// If the player has been sighted and isn't dead...
		//Si el jugador ha sido avistado y no ha muerto..
		else if(enemySight.personalLastSighting != lastPlayerSighting.resetPosition && playerHealth.health > 0f)
			// ... chase.
			//... persigalo.
			Chasing();
		
		// Otherwise...
		//de otro modo
		else
			// ... patrol.
			// ... patrulle.
			Patrolling();
	}
	
	
	void Shooting ()
	{
		// Stop the enemy where it is.
		// detenga el enemigo donde esta.
		nav.Stop();
	}
	
	
	void Chasing ()
	{
		// Create a vector from the enemy to the last sighting of the player.
		//cree un vector para el enemigo del ultimo avistamiento del jugador.
		Vector3 sightingDeltaPos = enemySight.personalLastSighting - transform.position;
		
		// If the the last personal sighting of the player is not close...
		//Si el ultimo avistamineto personal del jugador no esta cerca...
		if(sightingDeltaPos.sqrMagnitude > 4f)
			// ... set the destination for the NavMeshAgent to the last personal sighting of the player.
			//... establezca el destino para el NavMeshAgent para el ultimo avistamiento del jugador.
			nav.destination = enemySight.personalLastSighting;
		
		// Set the appropriate speed for the NavMeshAgent.
		// Ajuste la velocidad apropiada para el NavMeshAgent.
		nav.speed = chaseSpeed;
		
		// If near the last personal sighting...
		// Si cerca de el ultimo avistamiento personal
		if(nav.remainingDistance < nav.stoppingDistance)//quiere decir si tiene alcanzado el destino
		{
			// ... increment the timer.
			//... incremente el tiempo
			chaseTimer += Time.deltaTime;
			
			// If the timer exceeds the wait time...
			// Si el tiempo excede el tiempo de espera...
			if(chaseTimer >= chaseWaitTime)
			{
				// ... reset last global sighting, the last personal sighting and the timer.
				// ... resetee el ultimo avistamiento global, el ultimo avistamiento personal y el temporizador.
				lastPlayerSighting.position = lastPlayerSighting.resetPosition;
				enemySight.personalLastSighting = lastPlayerSighting.resetPosition;
				chaseTimer = 0f;
			}
		}
		else
			// If not near the last sighting personal sighting of the player, reset the timer.
			// si no esta cerca del ultimo avistamiento personal del jugador, reestablezca el temporizador.
			chaseTimer = 0f;
	}
	
	
	void Patrolling ()
	{
		// Set an appropriate speed for the NavMeshAgent.
		// establezca una velocidad apropiada para el NavMeshAgent.
		nav.speed = patrolSpeed;
		
		// If near the next waypoint or there is no destination...
		// Si esta cerca del siguiente waypoint o este no es destino ...
		if(nav.destination == lastPlayerSighting.resetPosition || nav.remainingDistance < nav.stoppingDistance)
		{
			// ... increment the timer.
			// ... incremente el temporizador.
			patrolTimer += Time.deltaTime;
			
			// If the timer exceeds the wait time...
			// si el temporizador excede el tiempo de espera...
			if(patrolTimer >= patrolWaitTime)
			{
				// ... increment the wayPointIndex.
				// ... incremente el wayPointIndex.
				if(wayPointIndex == patrolWayPoints.Length - 1)
					wayPointIndex = 0;
				else
					wayPointIndex++;
				
				// Reset the timer.
				// restablezca el temporizador.
				patrolTimer = 0;
			}
		}
		else
			// If not near a destination, reset the timer.
			// si no esta cerca de un destino, restablezca el temporizador.
			patrolTimer = 0;
		
		// Set the destination to the patrolWayPoint.
		// establezca el destino para el patrolWayPoint.
		nav.destination = patrolWayPoints[wayPointIndex].position;
	}
}