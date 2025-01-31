using UnityEngine;
using System.Collections;

public class TextFlash : MonoBehaviour {
	
	private Material _material;
	
	public Renderer objToFlash;
	
	[Header("Emisson Color")]
	public Color baseColor = Color.white;
	
	[Header("Emisson Strength")]
	public float minEmisson = 0.1f;
	public float maxEmisson = 1.1f;
	
	[Header("Time")]
	public float freqrency = 0.5f;
	
	void Start () 
	{
		Random.seed = Random.Range(0, int.MaxValue);
		
		// If there is no paramter then search it in children
		if(objToFlash == null) 
			objToFlash = gameObject.GetComponentInChildren<Renderer>();
		
		_material = objToFlash.material;
	}
	
	void Update () 
	{
		//float emission = minEmisson + Mathf.PingPong (Time.time * freqrency, maxEmisson - minEmisson);
		float emission = Mathf.PingPong (Time.time * freqrency, Random.Range(minEmisson, maxEmisson));
		Color finalColor = baseColor * Mathf.LinearToGammaSpace (emission);
		
		_material.SetColor ("_EmissionColor", finalColor);
	}
}