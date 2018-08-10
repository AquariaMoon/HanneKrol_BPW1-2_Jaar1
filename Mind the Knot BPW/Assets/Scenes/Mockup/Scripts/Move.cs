using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Move : MonoBehaviour {
	[Header("Movement factors")]
	public bool canMove = false; //Used to check if the end of the path is reached, changed in 'stopTrigger'
	public float moveSpeed = 3f; //Used to determine speed with which player moves
	public int countTriggers = 0; //Used to keep count of how many triggers are already interacted with

	private AudioSource steps; //Used to store the player's own audiosource
	private bool walking = false; //Used to check whether the player is moving or not

	void Start () {
		//Assign the audiosource from the singleton to local variable
		steps = MusicSingleton.Instance.stepSource;
	}

	void Update () {
		if (canMove) //If the end of the path isn reached yet
		{
			DoMovement (); //Call function 'DoMovement'
		} else {
		}
	}

	void DoMovement()//Called if the end of the path isn reached yet
	{
		float vert = Input.GetAxis("Vertical"); //Gets input from the keyboard if applicable (up/down, W/S keys)
		float hori = Input.GetAxis("Horizontal"); //Gets input from the keyboard if applicable (left/right, A/D keys)
		transform.position += vert * transform.forward * moveSpeed * Time.deltaTime; //Calculates actual motion (forwards/backwards)
		transform.Rotate(Vector3.up * hori * (moveSpeed * 10) * Time.deltaTime); //Calculates how the player turns around by rotating the playerś transform

		//If 'vert' is not 0 (aka player is moving)
		if (vert !=0) {
			//And 'walking' is false
			if (!walking) {
				//Play sound of footsteps
				steps.Play ();
			}
			//Set 'walking' to true so the footsteps aren initiated more than once
			walking = true;
		} else { //If 'vert' is 0, aka the player is standing still
			//And 'walking' is true, signifying that the sound was previously on
			if (walking) {
				//Stop the sound of footsteps
				steps.Stop ();
			}
			//Set 'walking' to false, so the sound can be played again once the player begins to move
			walking = false;
		}
	}

}
