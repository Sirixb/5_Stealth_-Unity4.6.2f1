using UnityEngine;
using System.Collections;
//Entendido
public class AnimatorSetup 
{
	public float speedDampTime = 0.1f;              // Damping time for the Speed parameter.
	public float angularSpeedDampTime = 0.7f;       // Damping time for the AngularSpeed parameter
	public float angleResponseTime = 0.6f;          // Response time for turning an angle into angularSpeed.
													//Tiempo de respuesta para girar un angulo en angularspeed.

	private Animator anim;                          // Reference to the animator component.
	private HashIDs hash;                           // Reference to the HashIDs script.
	
	
	// Constructor
	public AnimatorSetup(Animator animator, HashIDs hashIDs)
	{
		anim = animator;
		hash = hashIDs;
	}
	
	
	public void Setup(float speed, float angle)
	{
		// Angular speed is the number of degrees per second.
		float angularSpeed = angle / angleResponseTime;
		
		// Set the mecanim parameters and apply the appropriate damping to them.
		// Establece los parametros en mecanin y aplica la amortiguacion apropiada a ellos.
		anim.SetFloat(hash.speedFloat, speed, speedDampTime, Time.deltaTime);
		anim.SetFloat(hash.angularSpeedFloat, angularSpeed, angularSpeedDampTime, Time.deltaTime);
	}   
}
