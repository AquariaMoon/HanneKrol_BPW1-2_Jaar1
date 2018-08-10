using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	
	void Start () {
		//Start the background music by calling snigleton function 'Play'
		MusicSingleton.Instance.Play ();
	}

	public void StartGame () {
		//Used by one of the UI buttons in the scene, making the game load the second scene in build order
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
	}

	public void QuitGame () {
		//Used by one of the UI buttons in the scene, making the game application shut down
		Application.Quit ();
	}
}
