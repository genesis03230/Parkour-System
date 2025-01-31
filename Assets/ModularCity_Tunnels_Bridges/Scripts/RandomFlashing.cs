using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFlashing : MonoBehaviour {

	public Light lightSource;
	public Material material1;
	public Material material2; 
	public Color lightColor;
	public Color materialColor1;
	public Color materialColor2; 

	public float minIntensity;
	public float maxIntensity;


	float flashingTimer;

	void Update()
	{
		flashingTimer += Time.deltaTime;

		if (flashingTimer >= 0.2f)
		{
			ChangeIntensityAndColor();
			flashingTimer = 0f; 
		}
	}

	void ChangeIntensityAndColor()
	{
		float newIntensity = Random.Range(minIntensity, maxIntensity);
		lightSource.intensity = newIntensity;


		material1.SetColor("_EmissionColor", materialColor1 * newIntensity);

	
		material2.SetColor("_EmissionColor", materialColor2 * newIntensity);

		lightSource.color = lightColor; 
	}
}
