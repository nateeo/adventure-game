using UnityEngine;
using System.Collections;

/**
 * This class handles the GameObjectives. It turns the Objective object and text to face the player and scales
 * the objective depending on how far away the player is.
 */
public class GameObjective : MonoBehaviour {
	public Transform target;
	public GameObjectiveController controller;
	private float damp = 100f;

	// Use this for initialization
	void Start () {}

	// Update is called once per frame
	void Update () {
		if (target != null) {
			var rotationAngle = Quaternion.LookRotation ( target.position - transform.position); // we get the angle has to be rotated
			transform.rotation = Quaternion.Slerp ( transform.rotation, rotationAngle, Time.deltaTime * damp); // we rotate the rotationAngle 

			//Get the distance to the object marker
			float distance = Vector3.Distance (target.position, transform.position);

			//If the scale is less than 1, keep it at 1, if the scale is greater than 15, keep it at 15
			float transformScale = (distance / 30) < 1 ? 1 : (distance / 30);
			transformScale = transformScale > 15 ? 15 : transformScale;

			//Transform the marker
			transform.localScale = new Vector3(transformScale, transformScale, transformScale);

			//Make the marker disappear when you are within a certain distance
			if (distance <= 20) {
				controller.disable (this);
			} 
		}
	}
}
