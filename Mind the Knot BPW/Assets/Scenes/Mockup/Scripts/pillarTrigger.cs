using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pillarTrigger : MonoBehaviour {

	[Header("Unique Attributes to this Trigger")]
	public GameObject Glow; //Used for management of the light specific to each trigger
	public GameObject Orb; //Used for management of the particle system specific to each trigger
	public GameObject Sparkle; //Used for management of the particle system specific to each trigger

	[Header ("To be influenced")]
	public Transform Floor; //Turns on after 3 triggers are found
	public AudioSource pickUp; //Audiosource for when a pickup is triggered
	public AudioSource floorWind; //Audiosource signifying the player has to do something else- a change in mood occurs

	private bool Enter = false;
	private int countLocalTriggers = 0;

	void Update()
	{
		//If 'Enter' is true, aka the player has entered the trigger
		if (Enter) {
			//If the left mouse button is clicked
			if (Input.GetMouseButtonDown (0)) {
				//Call function 'Trigger'
				Trigger ();
			}
		}
	}

	void OnTriggerEnter(Collider other) //If an other collider enters the trigger
	{
		//Set 'Enter' to true
		Enter = true;
		//Get the trigger count from the player's 'Move' script and give it +1. Then use the resulting number to fill in 'countLocalTriggers'
		other.GetComponent<Move> ().countTriggers += 1;
		countLocalTriggers = other.GetComponent<Move> ().countTriggers;
	}

	void OnTriggerExit(Collider other) //Once an other collider exits the trigger
	{
		//Set 'Enter' to false
		Enter = false;
	}

	void Trigger()
	{
		//Play audiosource 'pickUp'
		pickUp.Play ();
		transform.parent.parent.GetComponent<pillarLights> ().Trigger (); //Calls 'trigger' function in 'pillarLights' script
		//Turn off the particle systems and lightsource connected to this trigger, showing the trigger has been picked up
		Glow.SetActive(false);
		Orb.SetActive (false);
		Sparkle.SetActive (false);
		//If this trigger is the third trigger
		if (countLocalTriggers == 3) 
		{
			//Play the audiosource 'floorWind' and change the emission of the floor shader, turning it 'on'
			floorWind.Play ();
			Renderer floorShader = Floor.GetComponent<Renderer> ();
			Color32 EmissiveColor = new Color32 (159, 162, 255, 0);
			floorShader.material.SetColor ("_Emission_Color", EmissiveColor);
		}

		Destroy (this.gameObject); //Destroys itself
	}
}
