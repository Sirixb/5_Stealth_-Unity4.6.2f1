using UnityEngine;
using System.Collections;

public class EnemyAnimation : MonoBehaviour 
{
	public float deadZone = 5f;             // The number of degrees for which the rotation isn't controlled by Mecanim.
											//El numero de grados  para que la rotacion no sea controlada por Mecanim.
	
	private Transform player;               // Reference to the player's transform.
	private EnemySight enemySight;          // Reference to the EnemySight script.
	private NavMeshAgent nav;               // Reference to the nav mesh agent.
	private Animator anim;                  // Reference to the Animator.
	private HashIDs hash;                   // Reference to the HashIDs script.
	private AnimatorSetup animSetup;        // An instance of the AnimatorSetup helper class.
	
	
	void Awake ()
	{
		// Setting up the references.
		//Congiguracion de las referencias.
		player = GameObject.FindGameObjectWithTag(Tags.player).transform;
		enemySight = GetComponent<EnemySight>();
		nav = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
		
		// Making sure the rotation is controlled by Mecanim.
		//Cerciorarse de que la rotacion es controlada por Mecanin y no por el navmeshagent.
		nav.updateRotation = false;
		
		// Creating an instance of the AnimatorSetup class and calling it's constructor.
		// Creando una instancia de la Clase AnimatorSetup y llamamos el constructor.
		animSetup = new AnimatorSetup(anim, hash);
		
		// Set the weights for the shooting and gun layers to 1.
		// Establecemos el peso de las capas Shooting y gun a 1.
		anim.SetLayerWeight(1, 1f);
		anim.SetLayerWeight(2, 1f);
		
		// We need to convert the angle for the deadzone from degrees to radians.
		// Necesitamos convertir el angulo de deadzone de grados a radianes, ya que el motor lo opera asi mas facil.
		deadZone *= Mathf.Deg2Rad;
	}
	
	
	void Update () 
	{
		// Calculate the parameters that need to be passed to the animator component.
		// Calcula los parametros que necesitan ser pasados a el componente animator.
		NavAnimSetup();
	}
	
	//esto deshabilita la opcion del animator de root motion en el inspector diciendo que sera manejado por script 
	void OnAnimatorMove ()//es llamado despues de cada updete frame
	{
		// Set the NavMeshAgent's velocity to the change in position since the last frame, by the time it took for the last frame.
		// Establece la velocidad del NavMeshAgent's para el cambio en posicion desde el ultimo cuadro, por el tiempo que tomo para el ultimo frame.
		nav.velocity = anim.deltaPosition / Time.deltaTime;//Establecemos la velocidad del agente a la posicion delta de la animacion divido por delta time. es un cambio de posicion por frame
		
		// The gameobject's rotation is driven by the animation's rotation.
		// La rotacion del gameobject es manejada por la rotacion de la animacion.
		transform.rotation = anim.rootRotation;
	}
	
	
	void NavAnimSetup ()
	{
		// Create the parameters to pass to the helper function.
		//Creamos los parametros que pasara a la funcion de ayuda(script AnimatorSetup).
		float speed;
		float angle;
		
		// If the player is in sight...
		// Si el jugador esta a la vista...
		if(enemySight.playerInSight)
		{
			// ... the enemy should stop...
			//... el enemigo deberia detenerse...
			speed = 0f;

			//EN EL VIDEO EXPLICA EN ESTE PUNTO QUE LA IZQUIERDA ES NEGATIVA Y DERECHA ES POSITIVA A PARTIR DEL VECTOR FORWARD QUE INDICA ADELANTE Y PASAMOS A CREAR LA FUNCION ANGLE MAS ABAJO.

			// ... and the angle to turn through is towards the player.
			//... y el angulo a traves del cual gira es decir hacia el jugador.
			angle = FindAngle(transform.forward, player.position - transform.position, transform.up);
		}
		else
		{
			// Otherwise the speed is a projection of desired velocity on to the forward vector...
			// de otro modo la variable velocidad es una proyeccion de velocidad deseada en el vector adelante...
			//(esto evitara que salga corriendo si no esta mirando en la direccion correcta.)
			speed = Vector3.Project(nav.desiredVelocity, transform.forward).magnitude;
			
			// ... and the angle is the angle between forward and the desired velocity.
			//... y el angulo es un angulo entre adelante y la velocidad deseada.
			angle = FindAngle(transform.forward, nav.desiredVelocity, transform.up);

			// If the angle is within the deadZone...
			// si el angulo esta dentro de la Zona muerta...
			if(Mathf.Abs(angle) < deadZone)
			{
				// ... set the direction to be along the desired direction and set the angle to be zero.
				//... establecemos la direccion a lo largo de la direccion deseada y estebleceremos el angulo en cero
				transform.LookAt(transform.position + nav.desiredVelocity);
				angle = 0f;
			}
		}
		
		// Call the Setup function of the helper class with the given parameters.
		// Llama a la funcion Setup de la clase ayudante(estamas arriba en este script) enviandole los parametros.
		animSetup.Setup(speed, angle);
	}
	
	// Encuentra el angulo entre la direccion de donde el enemigo deberia mirar y la direccion actual donde esta dando frente
	//datos recibidos: enemigo hacia adelante (eje z), posicion del jugador - la del enemigo y el eje Y identificando donde es arriba.
	float FindAngle (Vector3 fromVector, Vector3 toVector, Vector3 upVector)
	{
		// If the vector the angle is being calculated to is 0...
		// Si el  angulo del vector es calculado en cero...
		if(toVector == Vector3.zero)
			// ... the angle between them is 0.
			//... el angulo entre ellos es cero.
			return 0f;
		
		// Create a float to store the angle between the facing of the enemy and the direction it's travelling.
		//Crea un float para almacenar el angulo entre la postura del enemigo y la direccion que atraviesa.
		float angle = Vector3.Angle(fromVector, toVector);
		
		// Find the cross product(Producto vectorial) of the two vectors (this will point up if the velocity is to the right of forward).
		//Encuentra el producto vectorial de 2 vectores(esto apunta hacia arriba si la velocidad está a la derecha de adelante)
		Vector3 normal = Vector3.Cross(fromVector, toVector);
		
		// The dot product of the normal with the upVector will be positive if they point in the same direction.
		// El producto escalar de el normal es comparado con el upVector, será positivo(1)si apuntan en la misma dirección.// -1 si son completamente opuestas o cero si son perpendiculares.
		angle *= Mathf.Sign(Vector3.Dot(normal, upVector));//el angulo es multiplicado por el signo y esto nos dira si es derecha o izquierda.
		
		// We need to convert the angle we've found from degrees to radians.
		// necesitamos  para convertir el angulo que hemos encontrado de grados a radianes (para que unity trabaje mas rapido).
		angle *= Mathf.Deg2Rad;
		
		return angle;
	}
}
