using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightShoot : MonoBehaviour {

	[Header("Magic light")]
	public Transform glow; //Particle system that mimics a light orb
	public Transform sparks; //Particle system that gives little sparkles
	public Transform orb; //Used to pinpoint the location of the magic orb
	public Light light; //Lightsource for the magic light, giving the illusion that it actually sheds light

	[Header ("First person camera")]
	public Camera fpsCam; //Used for the raycast

	[Header ("Sound stuff")]
	public AudioSource doorHit; //Used for sound effect of the door getting hit/the beam hitting something
	public AudioSource magic1; //Used to give the magic light some magicky sounds
	public AudioSource magic2; //See above

	private string upOrDown; //Used to figure out whether particle systems needs to grow or shrink
	private string lightUpOrDown; //Used to figure out whether light needs to grow or shrink
	private string attack;
	private LineRenderer laserLine; //Used to visualize the raycast
	private float wait = 1.5f;
	private string doorWaited = "nope"; //Used to see if the door can be made to open yet
	private bool onOrOff = false; //Used to keep coroutines from being called more than once every time the mousekey is kept pressed and to reset once it's released
	private bool attackSound = false; //Used to keep coroutines from being called too often

	void Start () {
		laserLine = GetComponent<LineRenderer> (); //Stores the line renderer in variable for later raycast use
	}

	void Update () {
		//If the left mouse button is pressed
		if (Input.GetMouseButtonDown(0)) {
			//And onOrOff is false
			if (!onOrOff) {
				//Set the two upOrDown checks to "up" and define + call the three coroutines that will let the magic light grow
				upOrDown = "up";
				lightUpOrDown = "up";
				IEnumerator coroutine = LightGrow (glow); 
				StartCoroutine (coroutine); 
				coroutine = LightGrow (sparks); 
				StartCoroutine (coroutine); 
				coroutine = LightRangeUp (light);
				StartCoroutine (coroutine);
				//Play the magic noises, since the magic light is visible now
				//Set onOrOff to true to keep coroutines from getting called a bunch and to allow the light to shrink when the mousekey is released
				magic1.Play ();
				magic2.Play ();
				onOrOff = true;
			}
		}
		//If the left mouse button is released
		if (Input.GetMouseButtonUp(0)) {
			//And onOrOff is true
			if (onOrOff) {
				//Set the two upOrDown checks to "down" and define + call the three coroutines that will let the magic light shrink
				upOrDown = "down";
				lightUpOrDown = "down";
				IEnumerator coroutine = LightGone (glow); 
				StartCoroutine (coroutine); 
				coroutine = LightGone (sparks); 
				StartCoroutine (coroutine); 
				coroutine = LightRangeDown (light);
				StartCoroutine (coroutine);
				//Stop the magic noises since the light is fading 
				//Set onOrOff to false to keep coroutines from getting called a bunch and to allow the light to grow again when the mousekey is pressed
				magic1.Stop ();
				magic2.Stop ();
				onOrOff = false;
				//Set 'attacksound' to false so it can be used to re-start the coroutine when the attack is used again
				attackSound = false;
			}
		}

		//If 'attack' is set to "charge"
		if(attack == "charge"){
			//And 'attacksound' is set to false
			if (!attackSound) {
				//Play the sound of th beam hitting something and set 'attacksound' to true so the sound doesn get started more than once
				doorHit.Play ();
				attackSound = true;
			}
			//Set ray origin to the center of the first person cameraś view
			Vector3 rayOrigin = fpsCam.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, 0));
			RaycastHit hit; //Initiate a raycast to variable 'hit'
			//Set the origin of the rendered line to the position of the magic light and animate the texture so it looks like it's a laser shooting out
			laserLine.SetPosition (0, orb.position); 
			laserLine.material.mainTextureOffset = new Vector2 (-Time.time, 0);

			//Cast the raycast 50 units out forwards from the center of the first person camera- if it hits something, do this
			if(Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, 50f)){
				//Set the second point of the rendered line to where the raycast hit something
				laserLine.SetPosition (1, hit.point);
				//Call coroutine to wait for a moment, creating some tension before the door opens
				StartCoroutine (waitDoor ());
				//If doorWaited is set to "go"
				if (doorWaited == "go") {
					//Trigger the boolean 'SlideActive' to set to true in the DoorTriggers script on the component attached to the hit collider
					//causing the door to slide open
					hit.collider.GetComponent<DoorTriggers> ().SlideActive ();
				}
			}
		}
	}

	//Used to make the particle systems grow larger by manipulating the scale of their transforms
	private IEnumerator LightGrow(Transform system) { 
		float i = system.localScale.x; //Set 'i' by getting the current float of one point of the transform's scale
		while (upOrDown == "up") {
			//Loop through, increasing 'i' each loop and setting all parts of the transform's scale to said 'i', making it grow larger
			system.localScale = new Vector3(i, i, i);
			i += 0.009f;
			//Once 'i' has reached 1 the increase is stopped and 'attack' gets set to "charge"
			//Aka when the light has reached its maximum size the attack beam gets triggered
			if (i >= 1) {
				upOrDown = "stop";
				attack = "charge";
				//The two points have been set, draw the line renderer
				laserLine.enabled = true;
			}
			yield return null;
		}
	}

	//Used to make the particle systems grow smaller by manipulating the scale of their transforms. See previous coroutine, simply downscaling instead of upscaling
	private IEnumerator LightGone(Transform system) { 
		float i = system.localScale.x;
		while (upOrDown == "down") {
			system.localScale = new Vector3(i, i, i);
			i -= 0.009f;
			//Once 'i' has reached 0 the shrinking is stopped
			if (i <= 0) {
				upOrDown = "stop";
			}
			//If the attack is already happening it gets stopped once the light grows less
			if (attack == "charge") {
				attack = "stopCharge";
				//The attack has been stopped, drop the line renderer too
				laserLine.enabled = false;
			}
			yield return null;
		}
	}

	//Similar coroutine to the above two, only made to increase the light's range to make it seem like the growing magic light casts more actual light
	//Max for the range is 20 instead of 1
	//Increase of it is 0.15 instead of 0.009
	private IEnumerator LightRangeUp(Light light)
	{
		float i = light.range;
		while (lightUpOrDown == "up") 
		{
			i += 0.15f;
			light.range = i;
			if (i >= 20) {
				lightUpOrDown = "stop";
			}
			yield return null;
		}
	}

	//Similar coroutine to the top two, only made to decrease the light's range to make it seem like the shrinking magic light casts less actual light
	//Decrease of it is 0.15 instead of 0.009
	private IEnumerator LightRangeDown(Light light)
	{
		float i = light.range;
		while (lightUpOrDown == "down") 
		{
			i -= 0.1f;
			light.range = i;
			if (i <= 0) {
				lightUpOrDown = "stop";
			}
			yield return null;
		}
	}

	//Make the code wait before it continues and lets the door open
	private IEnumerator waitDoor(){
		yield return new WaitForSeconds (wait);
		doorWaited = "go";
	}
}
