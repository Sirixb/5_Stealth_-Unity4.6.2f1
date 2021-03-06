using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public AudioClip shoutingClip;      // Audio clip of the player shouting.//Aucio clip del jugador gritando.
	public float turnSmoothing = 15f;   // A smoothing value for turning the player.//Valor de suavidad para voltear el jugador.
	public float speedDampTime = 0.1f;  // The damping for the speed parameter//La amortiguacion del parametro de velocidad.
	
	
	private Animator anim;              // Reference to the animator component.//Referencia para el componente Animator
	private HashIDs hash;               // Reference to the HashIDs.//Referencia para el HashIDs.
	
	
	void Awake ()
	{
		// Setting up the references.//Creacion de referencias
		anim = GetComponent<Animator>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
		
		// Set the weight of the shouting layer to 1.//Establece el peso de la capa Shouting(gritantdo) a 1 (recuerda que el primer 1 refiere al indice de capa).
		anim.SetLayerWeight(1, 1f);
	}
	
	
	void FixedUpdate ()
	{
		// Cache the inputs.//Submemoria ultrarapida de las entradas.
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		bool sneak = Input.GetButton("Sneak");
		
		MovementManagement(h, v, sneak);
	}
	
	
	void Update ()
	{
		// Cache the attention attracting input.//Submemoria de entrada de llamado de atencion.
		bool shout = Input.GetButtonDown("Attract");
		
		// Set the animator shouting parameter.//Establece el parametro shouting del animator.
		anim.SetBool(hash.shoutingBool, shout);
		
		AudioManagement(shout);
	}
	
	
	void MovementManagement (float horizontal, float vertical, bool sneaking)
	{
		// Set the sneaking parameter to the sneak input.
		//Establece el parametro Sigiloso para la entrada de sigilo.
		anim.SetBool(hash.sneakingBool, sneaking);
		
		// If there is some axis input...// si estos son entradas de ejes iguales .
		if(horizontal != 0f || vertical != 0f)
		{
			// ... set the players rotation and set the speed parameter to 5.5f.
			//... Establece la rotacion del jugdor y establece el parametro de velocidad para 5.5f.
			Rotating(horizontal, vertical);
			anim.SetFloat(hash.speedFloat, 5.5f, speedDampTime, Time.deltaTime);
		}
		else
			// Otherwise set the speed parameter to 0.
			// De otro modo establezca el parametro de velocidad en 0.
			anim.SetFloat(hash.speedFloat, 0);
	}
	
	
	void Rotating (float horizontal, float vertical)
	{
		// Create a new vector of the horizontal and vertical inputs.
		// Cree un nuevo vector de entrada horizontal y vertical.
		Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
		
		// Create a rotation based on this new vector assuming that up is the global y axis.
		//Cree una rotacion basada en la direccion, asumiendo un new vector hasta el eje global y .
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
		
		// Create a rotation that is an increment closer to the target rotation from the player's rotation.
		//Crea una rotacion que incrementa mas cerca del objetivo de rotacion para el player's rotation.
		Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);
		
		// Change the players rotation to this new rotation.
		//Cambia la rotacion del jugador para una nueva rotacion.
		rigidbody.MoveRotation(newRotation);
	}
	
	
	void AudioManagement (bool shout)
	{
		// If the player is currently in the run state...
		//Si el jugador es esta en estado de corrido...
		if(anim.GetCurrentAnimatorStateInfo(0).nameHash == hash.locomotionState)
		{
			// ... and if the footsteps are not playing...
			//.. y si los pasos no estan sonando...
			if(!audio.isPlaying)
				// ... play them.//...Reproduzcalos.
				audio.Play();
		}
		else
			// Otherwise stop the footsteps.//De otra manera detenga el audio de los pasos
			audio.Stop();
		
		// If the shout input has been pressed...//Si la entrada de grito ha sido presionada.
		if(shout)
			// ... play the shouting clip where we are.//reproducir el grito clip donde estamos.
			AudioSource.PlayClipAtPoint(shoutingClip, transform.position);
	} 
}
