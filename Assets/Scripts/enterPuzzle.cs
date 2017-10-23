using UnityEngine;
using System.Collections;

public class enterPuzzle : MonoBehaviour {
	public GameObject box;
	public GameObject puzzle;
	public GameObject player;
	public bool icyPuzzle;
	// Use this for initialization

	void OnTriggerExit (Collider collider) {

		//get all the walls of the puzzle
		onCollision[] collisions = puzzle.GetComponentsInChildren<onCollision> ();
		collisions = collisions;
		onCollision collide = puzzle.GetComponentInParent<onCollision> ();

		//get the camera for the puzzle
		mapCamera cam = player.GetComponent<mapCamera> ();


		if (!icyPuzzle) {

			//set player movement to icy movement - change in mechanics of how player walks
			PlayerScript playerscript = player.GetComponent<PlayerScript> ();
			playerscript.icyPuzzle = true;

			//set all the walls to detect collisions and react accordingly for puzzle
			for (int i = 0; i < collisions.Length; i++) {
				collisions [i].icyPuzzle = true;
			}

			//set icy to true in this context
			icyPuzzle = true;

			//stop player from moving back
			box.SetActive (true);

			//set parent wall to detect collisions
			collide.icyPuzzle = true;

			//set camera to icy mode
			cam.icyPuzzle = true;

		} else if (icyPuzzle) {
			//return everything to normal
			PlayerScript playerscript = player.GetComponent<PlayerScript> ();
			playerscript.icyPuzzle = false;
			for (int i = 0; i < collisions.Length; i++) {
				//remove detection on walls
				collisions [i].icyPuzzle = false;
			}

			//close off the puzzle area
			box.SetActive (true);

			//set the puzzle to off
			icyPuzzle = false;

			//return parent wall to normal wall
			collide.icyPuzzle = false;

			//return to main camera
			cam.icyPuzzle = false;
		}
		Destroy (this);
	}
}
