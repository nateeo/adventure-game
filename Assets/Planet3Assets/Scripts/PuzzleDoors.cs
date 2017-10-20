using UnityEngine;
using System.Collections;

public class PuzzleDoors : MonoBehaviour {
	private static int MOVE_DOWN = 5; //How many times to move down

	private Vector3 startingPosition;
	private Vector3 target;

	private bool open = false;

	void Start () {
		// Get the initial starting position
		startingPosition = this.transform.position;
		target = new Vector3 (startingPosition.x, startingPosition.y - MOVE_DOWN, startingPosition.z);
	}
	
	void Update() {
		float step = 1f * Time.deltaTime;

		if (open) {
			transform.position = Vector3.MoveTowards (transform.position, target, step);
		} else {
			if (transform.position != startingPosition) {
				transform.position = Vector3.MoveTowards (transform.position, startingPosition, step);
			}
		}
	}

	public void openDoor() {
		open = true;
	}

	public void closeDoor() {
		open = false;
	}
}
