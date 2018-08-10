using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pillarLights: MonoBehaviour {

	public Transform Pillars; //Variable for in-unity object containing the pillars
	public Transform Lights; //Variable for in-unity object containing the lights in the pillars

	public int triggered=0; //Variable to keep track of triggers
	public Color normLight = new Color (0.335f, 0.219f, 0.360f); //Ambient light color after trigger 3
	public Color finLight = new Color (0.335f, 0.219f, 0.360f); //Ambient light color after trigger 4
	public Color one = new Color (0.107f, 0.024f, 0.125f); //Ambient light color after trigger 1
	public Color two = new Color (0.187f, 0.072f, 0.213f); //Ambient light color after trigger 2
	public Color three = new Color (0.335f, 0.219f, 0.360f); //Ambient light color after trigger 3
	public Color four = new Color (0.335f, 0.219f, 0.360f); //Ambient light color after trigger 4
	public Color emissive1 = new Color (0.107f, 0.024f, 0.125f); //Emissive light color after trigger 1
	public Color emissive2 = new Color (0.187f, 0.072f, 0.213f); //Emissive light color after trigger 2
	public Color emissive3 = new Color (0.335f, 0.219f, 0.360f); //Emissive light color after trigger 3
	public Color emissive4 = new Color (0.335f, 0.219f, 0.360f); //Emissive light color after trigger 4

	List<Renderer> PillarList = new List<Renderer>(); //List for renderers with emission
	List<Light> LightList = new List<Light>(); //List for lights

	private float normSpeed = 0.009f; //Used for adjustment speed when the player is walking through the pillars
	private float finSpeed = 0.004f; //Used when the final puzzle rolls away to reveal the butterfly

	void Start () {

		foreach (Transform pillar in Pillars) //For each in-unity pillar in the overarching object 'pillars'
		{
			if (pillar.GetChild (0).GetComponent<Renderer> ()) { //If the child of the child of that pillar has a renderer
				PillarList.Add(pillar.GetChild(0).GetComponent<Renderer>()); //Then get that component and add it to the list
			}
		}

		foreach (Transform light in Lights) //For each in-unity light in the overarching object 'lights'
		{
			LightList.Add(light.GetComponent<Light>()); //Add that light to the list
		}
	}

	public void Trigger () //Called from 'pillarTrigger'
	{
		triggered += 1; //Keeps track of triggers

		if (triggered == 1) { //If thereś only been 1 trigger
			IEnumerator coroutine = Light (0, 2, normSpeed); //Declare coroutine, define name and parameters: light starting value and ending value after the first trigger
			StartCoroutine(coroutine); //Call coroutine
			coroutine = Ambient (Color.black, one, normSpeed); //re-define coroutine name and parameters: ambient light starting color and ending color after the first trigger
			StartCoroutine(coroutine); //Call coroutine
			coroutine = Emission (0f, 0.25f, normSpeed/4); //re-define coroutine name and parameters: emissive light starting color and ending color after the first trigger
			StartCoroutine(coroutine); //Call coroutine
		} 
		else if (triggered == 2) //If thereś been 2 triggers
		{
			IEnumerator coroutine = Light (2, 4, normSpeed); //See 1 trigger, but using the 'end' value as start and moving on to the next needed value
			StartCoroutine(coroutine);
			coroutine = Ambient (one, two, normSpeed);
			StartCoroutine(coroutine);
			coroutine = Emission (0.25f, 0.5f, normSpeed/4);
			StartCoroutine(coroutine);
		} 
		else if (triggered == 3) //If thereś been 3 triggers
		{
			IEnumerator coroutine = Light (4, 6, normSpeed); //See 2 triggers
			StartCoroutine(coroutine);
			coroutine = Ambient (two, three, normSpeed);
			StartCoroutine(coroutine);
			coroutine = Emission (0.5f, 0.75f, normSpeed/4);
			StartCoroutine(coroutine);
		}
		else if (triggered == 4) //If thereś been 4 or more triggers
		{
			IEnumerator coroutine = LightLess (10, 5, finSpeed); //See 2 triggers
			StartCoroutine(coroutine);
			coroutine = Ambient (three, four, finSpeed);
			StartCoroutine(coroutine);
			coroutine = Emission (0.75f, 1f, finSpeed/4);
			StartCoroutine(coroutine);
			coroutine = FinaleLight (normLight, finLight, finSpeed); //
			StartCoroutine(coroutine);
		} else {
		}
	}

	private IEnumerator Light(int start, int end, float speed) { //Coroutine to gradually change the lights
		for (float i = start; i < end; i+=speed) { //Determines the speed in which the light changes
			foreach (Light light in LightList) //Makes sure each light in the created list will change
			{
				light.intensity = i; //Determines each frame what new intensity the lights get
			}
			yield return null; //Breaks out of the loop until the next frame
		}
		yield return null;
	}

	private IEnumerator LightLess(int start, int end, float speed) { //Coroutine to gradually change the lights
		for (float i = start; i > end; i-=speed) { //Determines the speed in which the light changes
			foreach (Light light in LightList) //Makes sure each light in the created list will change
			{
				light.intensity = i; //Determines each frame what new intensity the lights get
			}
			yield return null; //Breaks out of the loop until the next frame
		}
		yield return null;
	}

	private IEnumerator Ambient(Color start, Color end, float speed) { //Coroutine to gradually change the ambient light
		for (float i =0; i < 1; i+=speed) //Determines the speed with which the color changes
		{
			RenderSettings.ambientLight= Color.Lerp (start, end, i); //Uses i to determine where between start and end color the ambient light will be this frame
			yield return null;
		}
		yield return null;
	}

	private IEnumerator Emission(float start, float end, float speed) { //Coroutine to gradually change the emissive light, mix of 'Light' and 'Ambient'
		for (float i =start; i < end; i+=speed) {	
			foreach (Renderer renderer in PillarList)
			{
				renderer.material.SetFloat ("_Light", i);
			}
			yield return null;
		}
		yield return null;
	}
		
	private IEnumerator FinaleLight(Color start, Color end, float speed) { //Coroutine to gradually change COLOR of the lights to an orange tone for the finale. Repurposed code of 'Light'
		for (float i =0; i < 1; i+=speed) {	
			foreach (Light light in LightList)
			{
				light.color = Color.Lerp (start, end, i);
			}
			yield return null;
		}
		yield return null;
	}
}
