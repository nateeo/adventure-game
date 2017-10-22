using UnityEngine;
using System.Collections;

public class ObjectiveMarker : MonoBehaviour {
	public Transform target;
	private float damp = 100f;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		//Get the distance to the object marker
		float distance = Vector3.Distance (target.position, transform.position);

		var rotationAngle = Quaternion.LookRotation ( target.position - transform.position); // we get the angle has to be rotated
		transform.rotation = Quaternion.Slerp ( transform.rotation, rotationAngle, Time.deltaTime * damp); // we rotate the rotationAngle 

		//If the scale is less than 1, keep it at 1, if the scale is greater than 30, keep it at 30
		float transformScale = (distance / 30) < 1 ? 1 : (distance / 30);
		transformScale = transformScale > 5 ? 5 : transformScale;

		//Transform the marker
		transform.localScale = new Vector3(transformScale, transformScale, transformScale);

		//Make the marker disappear when you are within a certain distance
		if (distance <= 10) {
			transform.localScale = new Vector3 (0, 0, 0);
		}
	}
}
