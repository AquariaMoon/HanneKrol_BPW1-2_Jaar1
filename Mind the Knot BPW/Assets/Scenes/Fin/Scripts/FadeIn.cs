using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour {

	//Boolean 'start' triggered from PuzzleReact2 to signify at which point things need to come into view. 
	//Others used to manage speed with which consecutive changes are made.
	public bool start = false;
	public bool hope = false;
	public bool hold = false;
	public bool end = false;
	public bool go = false;
	public Light spotlight; //For predictable lightsource


	void Update () {
		//Get the renderer of the transform's first child
		Renderer background = transform.GetChild (0).GetComponent<Renderer> ();
		if (start) { //if 'start' has been set to true, define and start this coroutine
			IEnumerator coroutine = FadeStart (background, 0.0025f, "_Visible"); 
			StartCoroutine (coroutine); 

			if (go) {//if 'go' has been set to true, define and start this coroutine
				coroutine = Delay ("one", 2.5f); 
				StartCoroutine (coroutine); 

				if (hope) {//if 'hope' has been set to true, define and start these coroutines
					coroutine = FadeStart (background, 0.009f, "_Hope"); 
					StartCoroutine (coroutine); 
					coroutine = Delay ("two", 4.5f); 
					StartCoroutine (coroutine);

					if (hold) {//if 'hold' has been set to true, define and start these coroutines
						coroutine = FadeStart (background, 0.009f, "_Hold"); 
						StartCoroutine (coroutine); 
						coroutine = Delay ("three", 10f);
						StartCoroutine (coroutine); 

						if (end) { //if 'end' has been set to true, define and start these coroutines
							coroutine = FadeStart (background, 0.009f, "_End"); 
							StartCoroutine (coroutine); 
							coroutine = FadeStart (background, 0.009f, "_Emission");
							StartCoroutine (coroutine); 
						}					
					}
				}
			}
		}
	}
	//Coroutine to gradually change the visibility of the plane's shader
	private IEnumerator FadeStart (Renderer rend, float time, string slider) { 
		for (float i =0; i < 1; i+=time) 
		{
			//If the slider is the first one to be changes, turn on the spotlight too (for predictable lightsource)
			if (slider == "_Visible") {
				spotlight.intensity = 2 * i;
			} else if (slider == "_End") {
				
			}
			rend.material.SetFloat (slider, i);
			yield return null;
		}
		//Once the image is fully visible, set 'go' (next if-statement condition) to true
		go = true; 
		yield return null;
	}

	//Used to make the game wait a bit before continuing, so people can read the text and create a sense of rest
	private IEnumerator Delay(string which, float seconds) {
		yield return new WaitForSeconds (seconds);
		//Depending on the string given to the coroutine, different booleans are set to true, triggering the next if-statement conditions
		if (which == "one") {
			hope = true;
		} else if (which == "two") {
			hold = true;
		} else if (which == "three") {
			end = true;
		}
	}
}
