using UnityEngine;
using System.Collections;

//This script is used to open and close doors. It is attached to all of the movable doors.
//Author: Daniel Wong
public class PuzzleDoors : MonoBehaviour {
	private static int MOVE_DOWN = 8; //How many times to move down

	private Vector3 startingPosition;
	private Vector3 target;

	private bool open = false;

	void Start () {
		// Get the initial starting position
		startingPosition = this.transform.position;
		target = new Vector3 (startingPosition.x, startingPosition.y - MOVE_DOWN, startingPosition.z);
	}
	
	void Update() {
		float step = 2f * Time.deltaTime;

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
