using UnityEngine;
using System.Collections;

public class enterPuzzle : MonoBehaviour {
	public GameObject box;
	public GameObject puzzle;
	public GameObject player;
	public bool icyPuzzle;
	// Use this for initialization

	void OnTriggerExit (Collider collider) {
		onCollision[] collisions = puzzle.GetComponentsInChildren<onCollision> ();
		collisions = collisions;
		onCollision collide = puzzle.GetComponentInParent<onCollision> ();
		mapCamera cam = player.GetComponent<mapCamera> ();


		if (!icyPuzzle) {
			PlayerScript playerscript = player.GetComponent<PlayerScript> ();
			playerscript.icyPuzzle = true;
			for (int i = 0; i < collisions.Length; i++) {
				collisions [i].icyPuzzle = true;
			}
			icyPuzzle = true;
			box.SetActive (true);
			Debug.Log ("happened");
			collide.icyPuzzle = true;
			cam.icyPuzzle = true;
		} else if (icyPuzzle) {
			PlayerScript playerscript = player.GetComponent<PlayerScript> ();
			playerscript.icyPuzzle = false;
			for (int i = 0; i < collisions.Length; i++) {
				collisions [i].icyPuzzle = false;
			}
			box.SetActive (true);
			Debug.Log ("happened");
			icyPuzzle = false;
			collide.icyPuzzle = false;
			cam.icyPuzzle = false;
		}
		Destroy (this);
	}
}
