using UnityEngine;
using System.Collections;

public class AlarmLight : MonoBehaviour 
{
	public float fadeSpeed = 2f;            // How fast the light fades between intensities.//que tan rapido las luces varian en intensidad.
	public float highIntensity = 2f;        // The maximum intensity of the light whilst the alarm is on.//La maxima intensidad de la luz mientras la alarma esta encendida.
	public float lowIntensity = 0.5f;       // The minimum intensity of the light whilst the alarm is on.//La minima intensidad de la luz mientras la alarma esta encendida.
	public float changeMargin = 0.2f;       // The margin within which the target intensity is changed.//El margen dentro del cual se cambia la intensidad de destino.
	public bool alarmOn;                    // Whether or not the alarm is on.//si o no esta activada la alarma
	
	
	private float targetIntensity;          // The intensity that the light is aiming for currently.//La intensidad que la luz está apuntando a la actualidad.
	
	
	void Awake ()
	{
		// When the level starts we want the light to be "off".
		//cuando el nivel inicia queremos que la luz este apagada.
		light.intensity = 0f;
		
		// When the alarm starts for the first time, the light should aim to have the maximum intensity.
		//Cuando la alarma inice el primer tiempo, la luz debe tender a tener la maxima intensidad. 
		targetIntensity = highIntensity;
	}
	
	
	void Update ()
	{
		// If the light is on...
		//Si la luz esta encendida.
		if(alarmOn)
		{
			// ... Lerp the light's intensity towards the current target.
			//...Lerp(refiere a incrementar) las luces con intensidad hacia el objetivo actual
			light.intensity = Mathf.Lerp(light.intensity, targetIntensity, fadeSpeed * Time.deltaTime);
			
			// Check whether the target intensity needs changing and change it if so.
			//Compreve si el objetivo de intensidad necesita cambiar y cambie si es asi.
			CheckTargetIntensity();
		}
		else
			// Otherwise fade the light's intensity to zero.
			//De otro modo reduzca la intesidad de luz a cero.
			light.intensity = Mathf.Lerp(light.intensity, 0f, fadeSpeed * Time.deltaTime);
	}
	
	
	void CheckTargetIntensity ()
	{
		// If the difference between the target and current intensities is less than the change margin...
		//Si la diferencia entre el objetivo y la intensidad actual es menor de el margen de cambio...//2-0.2=1.8 < 0.2 ascendiendo no entra. //2-1.9=0.1 < 0.2 a partir de aqui entra //2-2=0 < 0.2 en el punto alto entra.
		if(Mathf.Abs(targetIntensity - light.intensity) < changeMargin)//0.5-1.5=-1 < 0.2 entre
		{
			// ... if the target intensity is high...
			//... Si la intensidad de objetivo es alto...//2==2
			if(targetIntensity == highIntensity)
				// ... then set the target to low.
				//... Entonces establezca el objteivo en bajo.//2=0.5
				targetIntensity = lowIntensity;
			else
				// Otherwise set the targer to high.
				// En caso contrario ajuste el objetivo en alto.
				targetIntensity = highIntensity;
		}
	}
}
