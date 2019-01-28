using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour 
{
	public float health = 100f;                         // How much health the player has left.//Cuanta salud ha dejado el jugador.
	public float resetAfterDeathTime = 5f;              // How much time from the player dying to the level reseting.//Cuanto tiempo dsde que el jugador muere para resetear el nivel
	public AudioClip deathClip;                         // The sound effect of the player dying.//el efecto de sonido de el jugador muriendo
	
	
	private Animator anim;                              // Reference to the animator component.
	private PlayerMovement playerMovement;              // Reference to the player movement script.
	private HashIDs hash;                               // Reference to the HashIDs.
	private SceneFadeInOut sceneFadeInOut;              // Reference to the SceneFadeInOut script.
	private LastPlayerSighting lastPlayerSighting;      // Reference to the LastPlayerSighting script.
	private float timer;                                // A timer for counting to the reset of the level once the player is dead.//Un temporizador para contar el reseteo del nivel una vez el jugdor muere.
	private bool playerDead;  //falso por defecto       // A bool to show if the player is dead or not.
	
	
	void Awake ()
	{
		// Setting up the references.
		anim = GetComponent<Animator>();
		playerMovement = GetComponent<PlayerMovement>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
		sceneFadeInOut = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<SceneFadeInOut>();
		lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
	}
	
	
	void Update ()
	{
		// If health is less than or equal to 0...
		//Si la salud es menor o igual a cero
		if(health <= 0f)
		{
			// ... and if the player is not yet dead...
			//... y si el jugador no muere todavia...
			if(!playerDead)//Si esta en falso: la primera vez entra, la segunda vez que pregunta esta en verdadero por la funcion PlayerDying.
				// ... call the PlayerDying function.
				//... llama a la funcion muriendo.
				PlayerDying();
			else
			{
				// Otherwise, if the player is dead, call the PlayerDead and LevelReset functions.
				// de otro modo, si el jugador esta muerto, llame a la funcion jugadormuerto(playerDead) y Resetear nivel
				PlayerDead();
				LevelReset();
			}
		}
	}
	
	
	void PlayerDying ()
	{
		// The player is now dead.//El jugador esta ahora muerto.
		playerDead = true;
		
		// Set the animator's dead parameter to true also.//Establece el parametro de animacion muerte a verdadero tambien.
		anim.SetBool(hash.deadBool, playerDead);
		
		// Play the dying sound effect at the player's location.
		//Reproduce el efecto de sonido mueriendo en la locacion del jugador.
		AudioSource.PlayClipAtPoint(deathClip, transform.position);
	}
	
	
	void PlayerDead ()
	{
		// If the player is in the dying state then reset the dead parameter.
		//Si el jugador esta en estado de muriendo(dying) restablezca los parametros de muerte.
		//recuerda que el cero representa el indice de capa de animaciones
		if(anim.GetCurrentAnimatorStateInfo(0).nameHash == hash.dyingState)
			anim.SetBool(hash.deadBool, false);
		
		// Disable the movement.//Deshabilite el movimiento.
		anim.SetFloat(hash.speedFloat, 0f);
		playerMovement.enabled = false;
		
		// Reset the player sighting to turn off the alarms.
		//Restablezcca el ultimo avistamiento para apagar las alarmas.
		lastPlayerSighting.position = lastPlayerSighting.resetPosition;
		
		// Stop the footsteps playing.// detenga las reproducciones de pasos.
		audio.Stop();
	}
	
	
	void LevelReset ()
	{
		// Increment the timer.
		//incremente el temporizador.
		timer += Time.deltaTime;
		
		//If the timer is greater than or equal to the time before the level resets...
		//Si el temporizador es mayor o igual a le tiempo antes de que se restablezca el nivel...
		if(timer >= resetAfterDeathTime)
			// ... reset the level.//restablezca el nivel.
			sceneFadeInOut.EndScene();//llama la funcion EndScene del script sceneFadeInOut
	}
	
	
	public void TakeDamage (float amount)
	{
		// Decrement the player's health by amount.//decremente la cantidad de salud del jugador.
		health -= amount;
	}
}
