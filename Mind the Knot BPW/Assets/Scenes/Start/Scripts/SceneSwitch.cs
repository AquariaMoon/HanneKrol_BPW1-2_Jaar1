using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour {

	void OnTriggerEnter(Collider other) //If an other collider enters the trigger
	{
		//Load the next scene as said by the order of the build index. Stop the footsteps of the player (the player automatically stops in scene transfer)
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
		MusicSingleton.Instance.stepSource.Stop ();
	}
}
