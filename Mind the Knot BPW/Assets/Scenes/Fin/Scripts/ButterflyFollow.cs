using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyFollow : MonoBehaviour {
	[Header("Target to be followed")]
	public Transform player; 
	[Header("Follower")]
	public Transform butterfly;
	public float followSpeed = 0.02f;

	private bool inRange = false; //Used to check if the follower can stop moving closer, prevents from getting too close

	void Update () {
		//Angles the transform towards the target making it seem like itś looking at the target, player in this case
		transform.LookAt (player.transform);
		//If the follower is not in range it is moved towards the position of the target at the set followSpeed
		if (!inRange) {
			butterfly.transform.position = Vector3.MoveTowards (transform.position, player.transform.position, followSpeed);
		} else {
		}
	}

	//When another collider enters the trigger collider and that colliderś transform has the 'Player' tag, the follower is in range
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player") {
			inRange = true;
		}
	}

	//When another collider exits the trigger collider and that colliderś transform has the 'Player' tag, the follower is no longer in range
	void OnTriggerExit (Collider other)
	{
		if (other.tag == "Player") {
			inRange = false;
		}
	}
}
