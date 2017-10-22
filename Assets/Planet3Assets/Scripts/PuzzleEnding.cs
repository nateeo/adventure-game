using UnityEngine;
using System.Collections;

/*
 * This handles the scenechange at the end of the puzzle
 */
public class PuzzleEnding : MonoBehaviour {
	void OnTriggerStay(Collider collide) {
		Application.LoadLevel("Planet3");
	}
}
