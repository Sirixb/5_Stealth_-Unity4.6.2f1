using UnityEngine;
using System.Collections;

public class LastPlayerSighting : MonoBehaviour 
{
	public Vector3 position = new Vector3(1000f, 1000f, 1000f);         // The last global sighting of the player.// El último avistamiento global del jugador 
	public Vector3 resetPosition = new Vector3(1000f, 1000f, 1000f);    // The default position if the player is not in sight.// La posición predeterminada si el jugador no está a la vista.
	public float lightHighIntensity = 0.25f;                            // The directional light's intensity when the alarms are off.// Intensidad dela luz direccional cuando las alarmas están apagadas.
	public float lightLowIntensity = 0f;                                // The directional light's intensity when the alarms are on.// Intensidad del luz direccional cuando las alarmas están encendidas.
	public float fadeSpeed = 7f;                                        // How fast the light fades between low and high intensity.// Que tan rapido las luces se desbanecen entre baja y alta intensidad.
	public float musicFadeSpeed = 1f;                                   // The speed at which the //La velocidad ala que la musica cambia.
	
	
	private AlarmLight alarm;                                           // Reference to the AlarmLight script.
	private Light mainLight;                                            // Reference to the main light.
	private AudioSource panicAudio;                                     // Reference to the AudioSource of the panic msuic.
	private AudioSource[] sirens;                                       // Reference to the AudioSources of the megaphones.
	
	
	void Awake ()
	{
		// Setup the reference to the alarm light.//configura la referencia de la luz de alarma
		alarm = GameObject.FindGameObjectWithTag(Tags.alarm).GetComponent<AlarmLight>();
		
		// Setup the reference to the main directional light in the scene.
		//configura la referencia de la luz principal direccional en la escena.
		mainLight = GameObject.FindGameObjectWithTag(Tags.mainLight).light;
		
		// Setup the reference to the additonal audio source.
		//Configura la referencia de el audio source aderido.
		panicAudio = transform.Find("secondaryMusic").audio;
		
		// Find an array of the siren gameobjects.// Encuentra un array de los objetos de juego de sirena
		GameObject[] sirenGameObjects = GameObject.FindGameObjectsWithTag(Tags.siren);
		
		// Set the sirens array to have the same number of elements as there are gameobjects.
		//Establece el array sirena para tener el mismo numero de elementos como de objetos de juego.
		sirens = new AudioSource[sirenGameObjects.Length];
		
		// For all the sirens allocate the audio source of the gameobjects.
		// Para todas la sirenas asigne el audio source en el objeto de juego.
		for(int i = 0; i < sirens.Length; i++)
		{
			sirens[i] = sirenGameObjects[i].audio;
		}
	}
	
	
	void Update ()
	{
		// Switch the alarms and fade the music.
		// Cambie las alarmas y desvanezca la musica.
		SwitchAlarms();
		MusicFading();
	}
	
	
	void SwitchAlarms ()//cambio de alarma, sub de update
	{
		// Set the alarm light to be on or off.//Establezca la luz de alarma para estar en on o en off.
		alarm.alarmOn = position != resetPosition;//da como resultado true o false para el script AlarmLight.
		
		// Create a new intensity.//Cree una nueva intensidad
		float newIntensity;
		
		// If the position is not the reset position...//Si la posicion no es resestposition
		if(position != resetPosition)
			// ... then set the new intensity to low.
			//entonces establezca la nueva intensidad(newIntensity) para bajo.
			newIntensity = lightLowIntensity;
		else
			// Otherwise set the new intensity to high.//de lo contrario establezca newIntensity para alto.
			newIntensity = lightHighIntensity;
		
		// Fade the directional light's intensity in or out.
		//Atenuar la intensidad de la luz direccional dentro o fuera.
		mainLight.intensity = Mathf.Lerp(mainLight.intensity, newIntensity, fadeSpeed * Time.deltaTime);
		
		// For all of the sirens...//para todas las sirenas....
		for(int i = 0; i < sirens.Length; i++)
		{
			// ... if alarm is triggered and the audio isn't playing, then play the audio.
			//si la alarma es activada y el sonido no se esta reproduciendo.
			if(position != resetPosition && !sirens[i].isPlaying)
				sirens[i].Play();
			// Otherwise if the alarm isn't triggered, stop the audio.
			// de lo contrario si la alarma no es activada, detenga el audio.
			else if(position == resetPosition)
				sirens[i].Stop();
		}
	}
	
	
	void MusicFading ()//sub de update.
	{
		// If the alarm is not being triggered...
		// si la alarma no se comienza a activar
		if(position != resetPosition)
		{
			// ... fade out the normal music...
			//desaparece la musica normal."que se encuentra aderida aca mismo en gamecontroller.
			audio.volume = Mathf.Lerp(audio.volume, 0f, musicFadeSpeed * Time.deltaTime);
			
			// ... and fade in the panic music.
			//... y aparece la musica de panico.
			panicAudio.volume = Mathf.Lerp(panicAudio.volume, 0.8f, musicFadeSpeed * Time.deltaTime);
		}
		else
		{
			// Otherwise fade in the normal music and fade out the panic music.
			// de lo contrario aparece la muscia normal y desaparece la musica de panico.
			audio.volume = Mathf.Lerp(audio.volume, 0.8f, musicFadeSpeed * Time.deltaTime);
			panicAudio.volume = Mathf.Lerp(panicAudio.volume, 0f, musicFadeSpeed * Time.deltaTime);
		}
	}

}
