using UnityEngine;
using System.Collections;

public class WaterAnim : MonoBehaviour {

	public float scrollSpeed = 0.5f;
	public bool u = false;
	public bool v = false;

	void Update () {
		float offset = Time.time * scrollSpeed % 1;
		if (!u && !v)
		{
			GetComponent<Renderer>().material.SetTextureOffset ("_MainTex",new Vector2(offset,offset));
		}
		else if (u == true)
		{
			GetComponent<Renderer>().material.SetTextureOffset ("_MainTex",new Vector2(offset,0));
		}
		else if (v == true)
		{
			GetComponent<Renderer>().material.SetTextureOffset ("_MainTex",new Vector2(0,offset));
		}


	}
}