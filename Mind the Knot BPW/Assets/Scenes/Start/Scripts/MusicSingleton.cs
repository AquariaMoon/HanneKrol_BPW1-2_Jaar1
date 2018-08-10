using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSingleton : MonoBehaviour {

	//Create a variable of the singleton and set it to null
	//Set the current instance of the class to the variable 'instance'
	private static MusicSingleton instance = null; 
	public static MusicSingleton Instance {
		get{ return instance;}
	}

	public AudioSource source; //Used for background music
	public AudioSource stepSource; //Used for footsteps of the player
	public AudioClip mainSong; //The first and main background music file
	public AudioClip endSong; //The second background music file, to be played at the end

	private IEnumerator coroutine; //So you only need to re-define the coroutines later

	void Awake () {
		//If the instance is not null and not the current instance- destroy the game object
		//This keeps clones from appearing
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			//Otherwise set 'instance' to the current instance 
			instance = this;
		}
		//Don destroy this gameobject on loading a new scene
		DontDestroyOnLoad (this.gameObject);
	}

	void Update () {
		//If the player presses 'escape', the game application stops
		if (Input.GetKey ("escape")) {
			Application.Quit ();
		}
	}

	//Makes the audiosource start playing
	public void Play (){
		source.Play ();
	}

	//Makes the audiosource stop playing
	public void Stop (){
		source.Stop ();
	}

	//Defines and calls a coroutine to fade the music down
	public void Fade (){
		coroutine = FadeOut (source, 2.5f);
		StartCoroutine (coroutine);
	}

	//Resets the audiosource's volume, sets a different audioclip and starts playing that
	public void Swap (){
		source.volume = 1;
		source.clip = endSong;
		Play ();
	}

	//Coroutine to lower the volume of the audiosource
	public IEnumerator FadeOut (AudioSource audiosource, float FadeTime){
		float startVol = audiosource.volume; //assigns the current volume of the given audiosource to 'startVol'
		while (audiosource.volume > 0){ //while the volume is higher than 0
			audiosource.volume -= (startVol * Time.deltaTime / FadeTime); //gradually lower it
			yield return null;
		}
	}
}
