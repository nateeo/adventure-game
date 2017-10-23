using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/*
 * This handles the scenechange at the end of the puzzle
 */
public class PuzzleEnding : MonoBehaviour {
	void OnTriggerStay(Collider collide) {
		SceneManager.LoadScene (5);
	}
}
