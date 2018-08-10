using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stopTrigger : MonoBehaviour {
	[Header("Which trigger are you?")]
	public string triggerPal = "level";

	void Update(){
		//If the 'outer' trigger is active AND the outer ring is solved
		if (triggerPal == "outer" && transform.parent.parent.parent.GetChild (2).GetComponent<puzzleReact2> ().checkOut1 && transform.parent.parent.parent.GetChild (2).GetComponent<puzzleReact2> ().checkOut2 && transform.parent.parent.parent.GetChild (2).GetComponent<puzzleReact2> ().checkOut3) {
			//triggers the 'Glow' function in 'puzzleReact2' with 'inactive', causing the glow to fade
			transform.parent.parent.parent.GetChild(2).GetComponent<puzzleReact2>().Glow(triggerPal, "inactive");
			Destroy (this.gameObject); //The trigger destroys itself, preventing players from being able to start the glow again to save confusion
		}

		//If the 'middle' trigger is active AND the middle ring is solved
		if (triggerPal == "middle" && transform.parent.parent.parent.GetChild (2).GetComponent<puzzleReact2> ().checkMid1 && transform.parent.parent.parent.GetChild (2).GetComponent<puzzleReact2> ().checkMid2 && transform.parent.parent.parent.GetChild (2).GetComponent<puzzleReact2> ().checkMid3) {
			//triggers the 'Glow' function in 'puzzleReact2' with 'inactive', causing the glow to fade
			transform.parent.parent.parent.GetChild(2).GetComponent<puzzleReact2>().Glow(triggerPal, "inactive");
			Destroy (this.gameObject); //The trigger destroys itself
		}

		//If the 'middle' trigger is active AND the middle ring is solved
		if (triggerPal == "inner" && transform.parent.parent.parent.GetChild (2).GetComponent<puzzleReact2> ().checkIn) {
			//triggers the 'Glow' function in 'puzzleReact2' with 'inactive', causing the glow to fade
			transform.parent.parent.parent.GetChild(2).GetComponent<puzzleReact2>().Glow(triggerPal, "inactive");
			Destroy (this.gameObject); //The trigger destroys itself
		}
		
	}

	void OnTriggerEnter(Collider other) //If an other collider enters the trigger
	{
		other.GetComponent<Move> ().canMove = false; //Sets canMove boolean in 'Move' to false
		//Sets 'setActive' in'puzzleReact' to 'true', allowing the player to move the floor disk
		transform.parent.parent.parent.GetChild(2).GetComponent<puzzleReact2>().setActive = true;
		//triggers the 'Glow' function in 'puzzleReact2' with 'active', causing the glow to start up
		transform.parent.parent.parent.GetChild(2).GetComponent<puzzleReact2> ().Glow (triggerPal, "active");
	}

	void OnTriggerExit(Collider other)
	{
		//triggers the 'Glow' function in 'puzzleReact2' with 'inactive', causing the glow to fade
		transform.parent.parent.parent.GetChild(2).GetComponent<puzzleReact2>().Glow(triggerPal, "inactive");
	}

}
