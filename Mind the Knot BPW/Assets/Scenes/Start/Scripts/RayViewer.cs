using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayViewer : MonoBehaviour {

	public Camera fpsCam; //Used to define starting point of raycast

	void Update () {
		//Used to view in the scene view how the raycasts functions. Invisible in game view.
		Vector3 lineOrigin = fpsCam.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, 0));
		Debug.DrawRay(lineOrigin, fpsCam.transform.forward*50, Color.cyan);
	}
}
