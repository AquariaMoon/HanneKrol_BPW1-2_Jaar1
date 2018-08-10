using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggers : MonoBehaviour {

	[Header("Say 'doors' if you swing open")]
	public string trigger; //Used to check which trigger is triggered

	[Header("Specify Transforms, Audiosources and Audioclips")]
	public Transform doorRight; //Used to store right side of the doors the trigger is connected to
	public Transform doorLeft; //Used to store right side of the doors the trigger is connected to
	public AudioSource slidingDoors; //Stores audiosource for the sliding doors
	public AudioSource swingingDoors; //Stores audiosource for the swinging doors
	public AudioClip door1; //Stores audioclip for first set of doors (mostly to see if I could in fact assign audioclips bys script)
	public AudioClip door2; //Stores audioclip for second set of doors

	[Header("Player transform")]
	public Transform player; //Stores player transform

	private HingeJoint hinge; //Defines hingejoint variable
	private bool initiate = true; //Used to keep sound and coroutines from getting initiated too often

	void Update(){
		//If the angle of the right door is greater than 99, stop the sound of the door opening 
		//(This is the point the door is fully open. left door could also be used)
		if(doorRight.eulerAngles.y > 99){
			swingingDoors.Stop ();
		}
	}

	void OnTriggerEnter(Collider other) //If an other collider enters the trigger
	{
		if (trigger == "doors") { //if the trigger has the string 'doors' this sets 'usespring' on both doors to 'true', causing them to open automatically
			//Also sets a clip to the audiosource and plays it, giving a sound effect
			swingingDoors.clip = door2;
			swingingDoors.Play ();
			hinge = doorRight.GetComponent<HingeJoint>();
			hinge.useSpring = true;
			hinge = doorLeft.GetComponent<HingeJoint> ();
			hinge.useSpring = true;
		}
	}

	//Triggered in 'LightShoot' script
	public void SlideActive(){
		//If 'initiate' is true
		if (initiate){
			//Player can start to move around
			player.GetComponent<Move> ().canMove = true;
			//Both sliding doors are given target destinations, coroutines are defined and called so the doors can gradually slide open
			Vector3 target = new Vector3 (-2.0f, 0.1f, -52.2f); 
			IEnumerator coroutine = SlideDoor (doorRight, target, 10f); 
			StartCoroutine (coroutine); 
			target = new Vector3 (8.4f, 0.1f, -52.2f); 
			coroutine = SlideDoor (doorLeft, target, 10f);
			StartCoroutine (coroutine);
		}
	}

	private IEnumerator SlideDoor(Transform transform, Vector3 position, float timeToMove){//Coroutine to gradually move the door as if it were rolling open
		//Sets initiate to false so the coroutine won be called more than once.
		initiate = false; 
		//Assigns audioclip to audiosource and plays it
		slidingDoors.clip = door1;
		slidingDoors.Play ();
		var currentPos = transform.position; //Saves the door's current position in a variable
		var t = 0f;//Defines variable t at 0
		while (t < 1) //While t is less than 1
		{
			t += Time.deltaTime / timeToMove; //Changes t over time, during the specified time to move
			transform.position = Vector3.Lerp (currentPos, position, t); //Lerps between starting position and target position using t's value to decide where and how fast to move the object
			yield return null; //Breaks out and delays until next frame
		}
		//Stops the audiosource from playing, stopping the sound effect once the doors stop
		slidingDoors.Stop ();
	}
}
