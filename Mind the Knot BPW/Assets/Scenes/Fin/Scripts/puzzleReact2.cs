using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puzzleReact2 : MonoBehaviour {
	[Header("Soundstuff")]
	public AudioSource puzzleScrape; //Audiosource responsible for the big rotating disk's scraping sound
	public AudioSource diskScrape; //Audiosource responsible for the floor disk's scraping sound
	public AudioSource checkPlace; //Audiosource responsible for the big rotating disk's falling into place also having a sound now

	[Header("Turning Puzzle Transforms")]
	public Transform disk; //Floordisk
	public Transform puzzle;//Wallpuzzle whole
	public Transform outside1; //First outer puzzle cylinder
	public Transform outside2; //Second outer puzzle cylinder
	public Transform outside3; //Third outer puzzle cylinder
	public Transform mid1; //First middle layer
	public Transform mid2; //Second middle layer
	public Transform mid3; //Third middle layer
	public Transform inside; //Inner puzzle cylinder
	[Header("Sunlight Components")]
	public Transform rays; //Contains all the particle systems faking the sunrays
	public Light sunGlow; //Faking sunlight-light
	[Header("Final Message Transforms")]
	public Transform fade; //Contains both plane and spotlight

	[Header("Changing Texture Transforms")]
	public Transform wallFront; //Contains the big front wall on which the rotating disks and window are placed
	public Transform Stairs; //Contains the plane where the stairs are
	public Transform Floor; //Contains the long floor path
	public Transform Disk; //Contains the disk on the floor
	public Transform StatMad; //Contains one of the three statues on the floor disk
	public Transform StatSad; //Contains one of the three statues on the floor disk
	public Transform StatFear; //Contains one of the three statues on the floor disk
	public Transform Box; //Variable for in-unity object containing the other walls
	public Transform Pillars; //Variable for in-unity object containing the pillars
	public Transform PillarLeft; //Contains the small pillar to the left of the path
	public Transform PillarRight; //Contains the small pillar on the right of the path

	[Header("Game Developing Checks")]
	public bool outer = false; //Check to see which of the three sets in the puzzle is active (outer)
	public bool middle = false; //Check to see which of the three sets in the puzzle is active (middle)
	public bool center = false; //Check to see which of the three sets in the puzzle is active (center)
	public bool setActive = false; //Activates the puzzle pieces once the player reaches the end of the path
	public bool checkOut1 = false; //Check to see if part of the outside ring has fallen into place
	public bool checkOut2 = false; //See above
	public bool checkOut3 = false; //See above
	public bool checkMid1 = false; //Check to see if part of the middle ring has fallen into place
	public bool checkMid2 = false; //See above
	public bool checkMid3 = false; //See above
	public bool checkIn = false; //Check to see if the inner ring has fallen into place
	public bool waited = false; //Used to make sure the player has to wait a moment before the reveal
	public bool revealWait = false; //Used to make sure the butterfly is seen long enough before we fade to the final note

	[Header("Changing Colors")]
	public Color emissiveOut = new Color (0.335f, 0.219f, 0.360f); //New color for emission of the puzzle rings
	public Color emissiveMid = new Color (0.335f, 0.219f, 0.360f); //In case I want different ones for each ring
	public Color emissiveCen = new Color (0.335f, 0.219f, 0.360f); //See above
	public Color sunDark = new Color (225f, 237f, 163f, 0.39f); //New color for emission of the puzzle rings
	public Color sunLight = new Color (225f, 237f, 163f, 0.0f);
	public Transform Rimlight; //Variable for in-unity object containing the backlight/rimlight for the statues

	//Booleans to stop coroutines from running infinitely in Update
	private bool Moved = false;
	private bool Changed = false;
	private bool Sunned = false;
	private bool haveWaited = false;
	private bool moreWaited = false;
	private bool play = false;
	private bool playDisk = false;

	//Because there's a lot of coroutines
	private IEnumerator coroutine;

	//Floats for the speed with which the puzzle disks turn
	private float speed = 20f;
	private float puzzleSpeed = 15f;
	List<Renderer> ChangeList = new List<Renderer>(); //List to store renderers that need their texture changed at the end
	List<Light> RimlightList = new List<Light>(); //List to store lights that need to be turned on or off when the statues need rimlighting


	void Start () {
		//Get all those renderers in the list
		ChangeList.Add(wallFront.GetComponent<Renderer> ());
		ChangeList.Add(Stairs.GetComponent<Renderer> ());
		ChangeList.Add(Floor.GetComponent<Renderer> ());
		ChangeList.Add(Disk.GetComponent<Renderer> ());
		ChangeList.Add(StatMad.GetComponent<Renderer> ());
		ChangeList.Add(StatSad.GetComponent<Renderer> ());
		ChangeList.Add(StatFear.GetComponent<Renderer> ());
		ChangeList.Add(PillarRight.GetComponent<Renderer> ());
		ChangeList.Add(PillarLeft.GetComponent<Renderer> ());
		foreach (Transform pillar in Pillars) //For each in-unity pillar in the overarching object 'pillars'
		{
			if (pillar.GetChild (0).GetComponent<Renderer> ()) { //If the child of that pillar has a renderer
				ChangeList.Add(pillar.GetChild(0).GetComponent<Renderer>()); //Then get that component and add it to the list
			}
		}

		foreach (Transform wall in Box) //For each in-unity pillar in the overarching object 'pillars'
		{
			if (wall.GetComponent<Renderer> ()) { //If the child of that pillar has a renderer
				ChangeList.Add(wall.GetComponent<Renderer>()); //Then get that component and add it to the list
			}
		}

		foreach (Transform light in Rimlight) //For each in-unity light in the overarching object 'Rimlight'
		{
			RimlightList.Add(light.GetComponent<Light>()); //Add that light to the list
		}
	}
	
	void Update () 
	{

		if (setActive) //If the end of the path is reached
		{
			MusicSingleton.Instance.stepSource.Stop (); //Stops the footstep sound
			DoSideMove (); //Allow the ground disk to be moved
			if (outer) //If the trigger connects to the outer ring
			{
				//If the end rotation goal is met
				if ((outside1.transform.rotation.eulerAngles.z <= 1f) && (outside1.transform.rotation.eulerAngles.z >= 0.5f)) { 
					//And the first outer check is false
					if (!checkOut1) {
						//Turn that check to 'true' and make sure 'play' is false 
						checkOut1 = true;
						play = false;
						//If 'play' is false 
						if (!play) {
							//Play the sound that signifies the ring has reached its goal and turn 'play' back to true, showing that the sound has been played
							checkPlace.Play ();
							play = true;
						}
					}
				}
				else { //If the end rotation goal is not met
					outside1.Rotate (0, 0, puzzleSpeed * Time.deltaTime, Space.Self); //Rotate the ring
				}

				//See previous ring, just for the second outer ring
				if ((outside2.transform.rotation.eulerAngles.z <= 1f) && (outside2.transform.rotation.eulerAngles.z >= 0f)) { 
					if (!checkOut2) {
						checkOut2 = true;
						puzzleScrape.Stop ();
						play = false;
						if (!play) {
							checkPlace.Play ();
							play = true;
						}
					}
				}
				else { //If the end rotation goal is not met
					outside2.Rotate (0, 0, -puzzleSpeed * Time.deltaTime, Space.Self); //Rotate the ring
					//If 'play' is false
					if (!play) {
						//Play the scraping sound and turn 'play' to true, keeping it from starting too often
						//(located here because this ring takes the longest to fall into place, meaning the scraping sound remains until it is done)
						puzzleScrape.Play ();
						play = true;
					}
				}

				//See previous ring, just for the third outer ring
				if ((outside3.transform.rotation.eulerAngles.z <= 2f) && (outside3.transform.rotation.eulerAngles.z >= 1f)) { //If the end rotation goal is met
					if (!checkOut3) {
						checkOut3 = true;
						play = false;
						if (!play) {
							checkPlace.Play ();
							play = true;
						}
					}
				}
				else { //If the end rotation goal is not met
					outside3.Rotate (0, 0, puzzleSpeed * Time.deltaTime, Space.Self); //Rotate the ring
				}
			} 
			else if (middle) //If the trigger connects to the middle ring
			{
				//See previous rings, just for the first middle ring
				if ((mid1.transform.localEulerAngles.y <= 1f) && (mid1.transform.localEulerAngles.y >= 0f)) { 
					if (!checkMid1) {
						checkMid1 = true;
						puzzleScrape.Stop ();
						play = false;
						if (!play) {
							checkPlace.Play ();
							play = true;
						}
					}
				}
				else { //If the end rotation goal is not met
					mid1.Rotate (0, puzzleSpeed * Time.deltaTime, 0,  Space.Self); //Rotate the ring
					if (!play) {
						//Play the scraping sound and turn 'play' to true, keeping it from starting too often
						//(located here because this ring takes the longest to fall into place, meaning the scraping sound remains until it is done)
						puzzleScrape.Play ();
						play = true;
					}
				}

				//See previous rings, just for the second middle ring
				if ((mid2.transform.localEulerAngles.y <= 1f) && (mid2.transform.localEulerAngles.y >= 0f)) { //If the end rotation goal is met
					if (!checkMid2) {
						checkMid2 = true;
						play = false;
						if (!play) {
							checkPlace.Play ();
							play = true;
						}
					}
				}
				else { //If the end rotation goal is not met
					mid2.Rotate (0, -puzzleSpeed * Time.deltaTime, 0,  Space.Self); //Rotate the ring
				}

				//See previous rings, just for the third middle ring
				if ((mid3.transform.localEulerAngles.y <= 1f) && (mid3.transform.localEulerAngles.y >= 0f)) { //If the end rotation goal is met
					if (!checkMid3) {
						checkMid3 = true;
						play = false;
						if (!play) {
							checkPlace.Play ();
							play = true;
						}
					}
				}
				else { //If the end rotation goal is not met
					mid3.Rotate (0, 1.2f * puzzleSpeed * Time.deltaTime, 0,  Space.Self); //Rotate the ring
				}

			} 
			else if (center) //If the trigger connects to the center ring
			{
				//See previous rings, just for the inner ring
				if ((inside.transform.rotation.eulerAngles.z <= 1f) && (inside.transform.rotation.eulerAngles.z >= 0f)) { //If the end rotation goal is met
					if (!checkIn) {
						checkIn = true;
						puzzleScrape.Stop ();
						play = false;
						if (!play) {
							checkPlace.Play ();
							play = true;
						}
					} 
				}
			else { //If the end rotation goal is not met
				inside.Rotate (0, 0, puzzleSpeed * Time.deltaTime, Space.Self); //Rotate the ring
				if (!play) {
						puzzleScrape.Play ();
						play = true;
				}
			}

			} else { //If none of the triggers are triggered and thus none of the rings are activated
				//Stop the scrape sound and turn 'play' to false, ready to start again when a trigger gets triggered
				puzzleScrape.Stop ();
				play = false;
			}
		} else { 
		}

		if (checkOut1 && checkOut2 && checkOut3 && checkMid1 && checkMid2 && checkMid3 && checkIn)//If all the puzzles are solved 
		{
			//And haveWaited is false
			if (!haveWaited){
				//Define and call the coroutine to create a pause before the door opens.
				//Call the singleton function to make the playing music fade
				MusicSingleton.Instance.Fade ();
				coroutine = Wait ("one", 2.5f); 
				StartCoroutine (coroutine); 
			}

			//If 'waited' is true
			if (waited) {
				//Set target coordinates where the door has to move to
				Vector3 target = new Vector3 (13.51f, 9.66f, -0.94f); 
				//if 'Moved' is false
				if (!Moved){
					//Call singleton function 'Swap' to change the audioclip
					MusicSingleton.Instance.Swap ();
					//define coroutine name and parameters: overarching puzzle object (door), the target coordinates and the speed
					coroutine = MovePuzzle (puzzle, target, 10f); 
					StartCoroutine (coroutine); //Call coroutine
				}

				//if 'Changed' is false
				if (!Changed) {
					//Define and call the coroutine to make everything change the color of the textures
					coroutine = ChangeTexture ();
					StartCoroutine (coroutine);
				}

				//Call the trigger in pillarLights one more time to cause color shift
				transform.parent.GetChild (1).GetComponent<pillarLights> ().Trigger ();
				//Assign particle systems from the 'rays' object and play them
				ParticleSystem system1 = rays.GetChild(0).GetComponent<ParticleSystem>();
				ParticleSystem system2 = rays.GetChild(1).GetComponent<ParticleSystem>();
				system1.Play();
				system2.Play();
				//If 'Sunned' is false
				if (!Sunned){
					//Define and call coroutines changing the color of the particle systems and starting up anothe rlight for fake sunlight
					coroutine = StartSun (sunDark, sunLight, system1); 
					StartCoroutine (coroutine); 
					coroutine = StartSun (sunDark, sunLight, system2); 
					StartCoroutine (coroutine); 
					coroutine = StartSunGlow (sunGlow); 
					StartCoroutine (coroutine); 
				}

				//If the door has reached its target location, use more if-statements for more pauses in the flow of everything
				if (puzzle.transform.position == target) {
					if (!moreWaited) {
						coroutine = Wait ("two", 10f); 
						StartCoroutine (coroutine); 
					}
					//if 'revealWait' is true
					if (revealWait) {
						//Start the fadein of the 'ending'
						fade.GetComponent<FadeIn> ().start = true;
					}
				} else { //If the door has not yet reached its target location
					puzzle.Rotate (0, 0, -10f * Time.deltaTime, Space.Self);//Rotate it (causes illusion of door rolling away)
					rays.Rotate (0, -3.5f * Time.deltaTime, 0, Space.Self); //Rotate the object theparticle systems are in, making it look like the sunlight falls in
				}
			}
		}
	}

	void DoSideMove()//Called if the end of the path is reached
	{
		//Get player input- are left/right or A/D keys used?
		float hori = Input.GetAxis ("Horizontal");
		disk.Rotate (0, hori * speed * Time.deltaTime, 0); //Uses that keyboard input to rotate the ground disk
		//If there is input (aka hori isn 0)
		if (hori != 0) {
			//And playDisk is not true
			if (!playDisk) {
				//Play the scrapesound and turn playDisk to true so the sound won get started ten thousand times
				diskScrape.Play ();
				playDisk = true;
			}
		} else { //If hori is 0
			//Don play the sound and turn playDisk to false, making it ready to start up again
			diskScrape.Stop ();
			playDisk = false;
		}
	}

	//Directs emissive glow from the puzzle, called from the disk triggers
	public void Glow (string level, string active) 
	{
		if (level=="outer") //If given string 'level' is 'outer'
		{
			Renderer rend1 = outside1.GetComponent<Renderer> ();//Grab the renderer connected to the first outer ring
			Renderer rend2 = outside2.GetComponent<Renderer> ();//Grab the renderer connected to the second outer ring
			Renderer rend3 = outside3.GetComponent<Renderer> ();//Grab the renderer connected to the third outer ring
			Renderer statue = StatSad.GetComponent<Renderer> ();//Grab the renderer connected to the third outer ring
			if (active=="active") //If given string 'active' is 'active'
			{
				outer = true; //boolean 'outer' is set to true, triggers outer disks to rotate in Update()
				//Causes emission to show up
				coroutine = Emission (Color.black, emissiveOut, rend1); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
				coroutine = Emission (Color.black, emissiveOut, rend2); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
				coroutine = Emission (Color.black, emissiveOut, rend3); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
				coroutine = StatueLight (statue); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
				coroutine = StatueRimlight (0f, 2f, 0.03f); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
			} else //If given string 'active' is something other than 'active'
			{
				outer = false; //boolean 'outer' is set to false, stopping the rotation of outer disks in Update()
				//Makes the emission fade to nothing again
				coroutine = Fade (emissiveOut, Color.black, rend1); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine(coroutine); //Call coroutine
				coroutine = Fade (emissiveOut, Color.black, rend2); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine(coroutine); //Call coroutine
				coroutine = Fade (emissiveOut, Color.black, rend3); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine(coroutine); //Call coroutine
				coroutine = StatueDark (statue); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
				coroutine = StatueRimdark (2f, 0f, 0.03f); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
			}
		}
			
		if (level=="middle") //Same as 'outer' but for middle ring
		{
			Renderer rend1 = mid1.GetComponent<Renderer> ();
			Renderer rend2 = mid2.GetComponent<Renderer> ();
			Renderer rend3 = mid3.GetComponent<Renderer> ();
			Renderer statue = StatFear.GetComponent<Renderer> ();
			if (active=="active") 
			{
				middle = true;
				coroutine = Emission (Color.black, emissiveOut, rend1); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
				coroutine = Emission (Color.black, emissiveOut, rend2); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
				coroutine = Emission (Color.black, emissiveOut, rend3); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
				coroutine = StatueLight (statue); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
				coroutine = StatueRimlight (0f, 2f, 0.03f); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
			} else 
			{
				middle = false;
				coroutine = Fade (emissiveOut, Color.black, rend1); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine(coroutine); //Call coroutine
				coroutine = Fade (emissiveOut, Color.black, rend2); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine(coroutine); //Call coroutine
				coroutine = Fade (emissiveOut, Color.black, rend3); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine(coroutine); //Call coroutine
				coroutine = StatueDark (statue); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
				coroutine = StatueRimdark (2f, 0f, 0.03f); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
			}
		}

		if (level=="inner")//Same as outer but for the center disk
		{
			Renderer rend = inside.GetComponent<Renderer> ();
			Renderer statue = StatMad.GetComponent<Renderer> ();
			if (active=="active") 
			{
				center = true;
				coroutine = Emission (Color.black, emissiveOut, rend); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
				coroutine = StatueLight (statue); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
				coroutine = StatueRimlight (0f, 2f, 0.03f); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
			} else 
			{
				center = false;
				coroutine = Fade (emissiveOut, Color.black, rend); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine(coroutine); //Call coroutine
				coroutine = StatueDark (statue); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
				coroutine = StatueRimdark (2f, 0f, 0.03f); //define coroutine name and parameters: emissive light starting color and ending color after the first trigger
				StartCoroutine (coroutine); //Call coroutine
			}
		}
	}

	//For emission starting
	//Coroutine to gradually change the emissive light, mix of 'Light' and 'Ambient' code in pillarLights
	private IEnumerator Emission(Color start, Color end, Renderer rend) { 
		for (float i =0; i < 1; i+=0.009f) 
		{
			rend.material.SetColor ("_EmissionColor", Color.Lerp (start, end, i));
			yield return null;
		}
		yield return null;
	}

	private IEnumerator StatueLight(Renderer rend) { //Coroutine to gradually turn up the statue's emission
		for (float i =0; i < 1; i+=0.009f) 
		{
			rend.material.SetFloat ("_Light", i); //Sets the slider defined as '_Light' in the renderer to 'i'
			yield return null;
		}
		yield return null;
	}

	//For emission fading
	private IEnumerator Fade(Color start, Color end, Renderer rend) { //Coroutine to gradually change the emissive light, mix of 'Light' and 'Ambient' code in pillarLights
		for (float i =0; i < 1; i+=0.009f) 
		{
			rend.material.SetColor ("_EmissionColor", Color.Lerp (start, end, i));
			yield return null;
		}
		yield return null;
	}

	private IEnumerator StatueDark(Renderer rend) { //Coroutine to gradually turn down the statue's emission
		for (float i =1; i > 0; i-=0.009f) 
		{
			rend.material.SetFloat ("_Light", i); //Sets the slider defined as '_Light' in the renderer to 'i'
			yield return null;
		}
		yield return null;
	}

	//Brighten rimlights
	private IEnumerator StatueRimlight(float start, float end, float speed) { //Coroutine to gradually change the lights
		for (float i = start; i < end; i+=speed) { //Determines the speed in which the light changes
			foreach (Light light in RimlightList) //Makes sure each light in the created list will change
			{
				light.intensity = i; //Determines each frame what new intensity the lights get
			}
			yield return null; //Breaks out of the loop until the next frame
		}
		yield return null;
	}

	//Darken rimlights
	private IEnumerator StatueRimdark(float start, float end, float speed) { //Coroutine to gradually change the lights
		for (float i = start; i > end; i-=speed) { //Determines the speed in which the light changes
			foreach (Light light in RimlightList) //Makes sure each light in the created list will change
			{
				light.intensity = i; //Determines each frame what new intensity the lights get
			}
			yield return null; //Breaks out of the loop until the next frame
		}
		yield return null;
	}

	//Causes the door to move to a new location
	private IEnumerator MovePuzzle(Transform transform, Vector3 position, float timeToMove){//Coroutine to gradually move the door as if it were rolling open
		Moved = true;
		var currentPos = transform.position; //Saves the door's current position in a variable
		var t = 0f;//Defines variable t at 0
		while (t < 1) //While t is less than 1
		{
			t += Time.deltaTime / timeToMove; //Changes t over time, during the specified time to move
			transform.position = Vector3.Lerp (currentPos, position, t); //Lerps between starting position and target position using t's value to decide where and how fast to move the object
			yield return null; //Breaks out and delays until next frame
		}
	}

	private IEnumerator ChangeTexture() { //Make list with all the things that need this changed- loop through them. In-editor just change the textures individually
		Changed = true;
		for (float i =0; i < 1; i+=0.004f) 
		{
			foreach (Renderer rend in ChangeList) {
				rend.material.SetFloat ("_Change", i);
			}
			yield return null;
		}
		yield return null;
	}

	//Delays execution of code for given amount of seconds, shifting booleans depending on given string to pace code continuation
	private IEnumerator Wait(string which, float seconds) { 
		if (which == "one") {
			haveWaited = true;
		} else if (which == "two") {
			moreWaited = true;
		}
			yield return new WaitForSeconds (seconds);
		if (which == "one") {
			waited = true;
		} else if (which == "two") {
			revealWait = true;
		}
	}

	//Changes color of given particle system once the door rolls away
	private IEnumerator StartSun(Color start, Color end, ParticleSystem system) { 
		for (float i =0; i < 1; i+=0.09f) 
		{
			var main = system.main;
			main.startColor = Color.Lerp (start, end, i);
			yield return null;
		}
		yield return null;
	}

	//Kickstart the fake sunlight
	private IEnumerator StartSunGlow(Light light) { //Coroutine to gradually change the lights
		Sunned = true;
		for (float i = 0; i < 1; i+=0.009f) { //Determines the speed in which the light changes
			light.intensity = i; //Determines each frame what new intensity the lights get
			yield return null; //Breaks out of the loop until the next frame
		}
		yield return null;
	}
}