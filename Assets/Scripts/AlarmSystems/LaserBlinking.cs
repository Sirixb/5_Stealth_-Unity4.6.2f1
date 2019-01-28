using UnityEngine;
using System.Collections;

public class LaserBlinking : MonoBehaviour 
{
	public float onTime;            // Amount of time in seconds the laser is on for.//Cantidad de tiempo en segundos que el laser esta activado para.
	public float offTime;           // Amount of time in seconds the laser is off for.//Cantidad de tiempo en segundos que el laser esta apagado para.
	
	
	private float timer;            // Timer to time the laser blinking.//Temporizador(Timer) para el tiempo del laser parpadeando.
	
	
	void Update ()
	{
		// Increment the timer by the amount of time since the last frame.
		//Incrementa el contador de tiempo por la cantidad de tiempo transcurrido en el ultimo fotograma.
		timer += Time.deltaTime;
		
		// If the beam is on and the onTime has been reached...
		//Si el rayo esta encendido y el onTime fue alcanzado...
		if(renderer.enabled && timer >= onTime)
			// Switch the beam.
			SwitchBeam();
		
		// If the beam is off and the offTime has been reached...
		//Si el rayo esta apagado y el offTime fue alcanzado...
		if(!renderer.enabled && timer >= offTime)
			// Switch the beam.
			SwitchBeam();
	}
	
	
	void SwitchBeam ()
	{
		// Reset the timer.
		//Resetea el Temporizador
		timer = 0f;
		
		// Switch whether the beam and light are on or off.
		//Cambie si el rayo y la luz estan prendidas o apagadas.
		renderer.enabled = !renderer.enabled;
		light.enabled = !light.enabled;
	}
}
