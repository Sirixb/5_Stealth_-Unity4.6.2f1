using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
	public float smooth = 1.5f;         // The relative speed at which the camera will catch up.
										//La velocidad relativa con la que la camara - capturara arriba/-se pondra al dia.	
	
	private Transform player;           // Reference to the player's transform.
	private Vector3 relCameraPos;       // The relative position of the camera from the player.//La posicion relativa de la camara del jugador.
	private float relCameraPosMag;      // The distance of the camera from the player.//la distancia de la camara del jugador.
	private Vector3 newPos;             // The position the camera is trying to reach.//La posicion que la camara esta tratando de alcanzar.
	
	public float zoomAdicional=2f; //6.089293 posicion camara en Y

	void Awake ()
	{
		// Setting up the reference.
		player = GameObject.FindGameObjectWithTag(Tags.player).transform;
		
		// Setting the relative position as the initial relative position of the camera in the scene.
		//Configuracion de la posicion relativa como la posicion relativa inicial de la camara en la escena.
		relCameraPos = transform.position - player.position;
		relCameraPosMag = relCameraPos.magnitude - 0.5f;//reducimos porque la posicion apunta a sus pies, asi que cuando emitimos rayo para ver si nada lo esta tapando podria golpear el suelo y no queremos eso.
	}
	
	
	void FixedUpdate ()
	{
		//Codigo extra para mejorar la sugerencia de mi jugador
		bool presCam = Input.GetKey(KeyCode.C);
		Vector3 zoom = presCam ? new Vector3(0, zoomAdicional,0):Vector3.zero;

		//transform.position = new Vector3(transform.position.x, Mathf.Clamp (transform.position.y, 6.089293f, transform.position.y + zoomAdicional),transform.position.z);
		// The standard position of the camera is the relative position of the camera from the player.
		// La posicion estandar de la camara es la posicion relativa de la camara del jugador.
		Vector3 standardPos = player.position + relCameraPos+ zoom ;//+ zoom
		
		// The abovePos is directly above the player at the same distance as the standard position.
		//La abovePos es directamente arriba del jugador en la misma distancia como la posicion estandar.
		Vector3 abovePos = player.position + Vector3.up * relCameraPosMag + zoom;//+ zoom
		
		// An array of 5 points to check if the camera can see the player.
		//Un Array de 5 puntos para verificar si la camara puede ver el jugador.
		Vector3[] checkPoints = new Vector3[5];
		
		// The first is the standard position of the camera.
		//El primero es la posicion estandar de la camara.
		checkPoints[0] = standardPos;
		
		// The next three are 25%, 50% and 75% of the distance between the standard position and abovePos.
		//Los siguientes 3 son 25%, 50% and 75% de la distancia entre la posicion estandar y la posicion de arriba.
		checkPoints[1] = Vector3.Lerp(standardPos, abovePos, 0.25f);
		checkPoints[2] = Vector3.Lerp(standardPos, abovePos, 0.5f);
		checkPoints[3] = Vector3.Lerp(standardPos, abovePos, 0.75f);
		
		// The last is the abovePos.//La ultima es la posicion de arriba (abovePos).
		checkPoints[4] = abovePos;
		
		// Run through the check points...//Correr atravez de los puntos de comprobacion.
		for(int i = 0; i < checkPoints.Length; i++)
		{
			// ... if the camera can see the player...//si la camara puede ver al jugador.
			if(ViewingPosCheck(checkPoints[i]))
				// ... break from the loop.//Salga de el loop, solo si el if es positivo.
				break;
		}
		

		// Lerp the camera's position between it's current position and it's new position.
		//Lerp(varie) la posicion de la camara entre la posicion actual y la nueva posicion.
		transform.position = Vector3.Lerp(transform.position, newPos, smooth * Time.deltaTime);
		
		// Make sure the camera is looking at the player.
		//Cerciorece de que la camara esta mirando en direccion del jugador.
		SmoothLookAt();
	}
	
	
	bool ViewingPosCheck (Vector3 checkPos)
	{
		RaycastHit hit;
		
		// If a raycast from the check position to the player hits something...
		//Si una proyeccion de rayo desde la posicion de verificacion golpea al jugador o a alguna cosa.
		if(Physics.Raycast(checkPos, player.position - checkPos, out hit, relCameraPosMag))
			// ... if it is not the player...//...Si eso no es el jugador...
			if(hit.transform != player)
				// This position isn't appropriate.//Esta posicion no es apropiada.
				return false;
		
		// If we haven't hit anything or we've hit the player, this is an appropriate position.
		//Si no tenemos ninguna cosa alcanzada o hemos alcanzado al jugador, esta es una posicion apropiada.
		newPos = checkPos;
		return true;
	}
	
	
	void SmoothLookAt ()
	{
		// Create a vector from the camera towards the player.
		//Crea un vector de la camara hacia el jugdor.
		Vector3 relPlayerPosition = player.position - transform.position;
		
		// Create a rotation based on the relative position of the player being the forward vector.
		//Cree una rotacion basada en la posicion relativa de el jugador iniciando hacia delante el vector.
		Quaternion lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);
		
		// Lerp the camera's rotation between it's current rotation and the rotation that looks at the player.
		//Varie la rotacion de las camaras entre la actual rotacion y la rotacion que se ve en el jugador.
		transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
	}
}
