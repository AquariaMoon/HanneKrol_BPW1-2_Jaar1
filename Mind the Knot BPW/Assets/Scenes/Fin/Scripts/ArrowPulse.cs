using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPulse : MonoBehaviour {
	[Header("Small Pillar Transforms")] //Not to need to search the scene using code
	public Transform pillarRight;
	public Transform pillarLeft;

	private string move = "start"; //Used to keep coroutines from being initiated indefinitely
	private bool right = false; //Used as check on which direction the player is pressing (right)
	private bool left = false; //Used as check on which direction the player is pressing (left)
	private bool nothing = false; //Used as check if the player is pressing no direction

	void Update () {
		//If the player has reached the end of the path and the main 'puzzle' is set to active
		if (transform.parent.GetChild (2).GetComponent<puzzleReact2> ().setActive) { 
			//Determine if the player is using left/right or A/D keys
			float movement = Input.GetAxis ("Horizontal");
			//Check which side the player is pressing or whether no input is given at all. Change the check-booleans accordingly
			if (movement > 0) {
				nothing = false;
				left = false;
				right = true;
			} else if (movement < 0) {
				nothing = false;
				right = false;
				left = true;
			} else if (movement == 0) {
				right = false;
				left = false;
				nothing = true;
			}

			Pulse (); //Call the 'Pulse' function
		} else {
		}
	}

	void Pulse(){
		//Get the renderers from the transforms, put them in their own variables for easier referencing
		Renderer rendRight = pillarRight.GetComponent<Renderer> ();
		Renderer rendLeft = pillarLeft.GetComponent<Renderer> ();

		//If check-boolean 'right' is true and movement has not been set to "right"
		if (right && move != "right") {
			//Start coroutine for the right pillar's renderer
			IEnumerator coroutine = PillarLight (rendRight);
			StartCoroutine (coroutine);

			//Float variable used to check the value of the slider '_Change' in the left pillar's renderer
			float leftCheck = rendLeft.material.GetFloat ("_Change");
			if(leftCheck>0.90){ //If said value is larger than 0.90 (aka it's glowing), start the coroutine to turn it dark for the left pillar
				coroutine = PillarDark (rendLeft);
				StartCoroutine (coroutine);
			}
			//Set movement to "right" to keep this if-statement from initiating more coroutines than needed
			move = "right";
		} else if (left && move != "left") { //See previous section, only for left pillar instead of right
			IEnumerator coroutine = PillarLight (rendLeft);
			StartCoroutine (coroutine);

			float rightCheck = rendRight.material.GetFloat ("_Change");
			if(rightCheck>0.90){
				coroutine = PillarDark (rendRight);
				StartCoroutine (coroutine);
			}

			move = "left";
		} else if (nothing && move != "null") { //If check-boolean 'nothing' is true and movement has not been set to "null"
			//Float variables used to check the value of the slider '_Change' in both pillar renderers
			float rightCheck = rendRight.material.GetFloat ("_Change");
			float leftCheck = rendLeft.material.GetFloat ("_Change");

			//Initiate coroutine for either pillar if the value is below 0,10 (aka it's not glowing)
			if (rightCheck < 0.10) {
				IEnumerator coroutine = PillarLight (rendRight);
				StartCoroutine (coroutine);
			}

			if (leftCheck < 0.10){
				IEnumerator coroutine = PillarLight (rendLeft);
				StartCoroutine (coroutine);
			}

			//Set movement to "null" to keep this if-statement from initiating more coroutines than needed
			move = "null";
		}
	}

	private IEnumerator PillarLight(Renderer rend) { //Coroutine to gradually change the emissive light, mix of 'Light' and 'Ambient' code in pillarLights
		for (float i =0; i < 1; i+=0.015f) 
		{
			rend.material.SetFloat ("_Change", i);
			yield return null;
		}
		yield return null;
	}	

	private IEnumerator PillarDark(Renderer rend) { //Coroutine to gradually change the emissive light, mix of 'Light' and 'Ambient' code in pillarLights
		for (float i =1; i > 0; i-=0.015f) 
		{
			rend.material.SetFloat ("_Change", i);
			yield return null;
		}
		yield return null;
	}
}
