using UnityEngine;
using System.Collections;

/**
 * Follows the main camera. Matches it's position and rotation
 */
public class FollowCamera : MonoBehaviour {
	public GameObject camera;

	void FixedUpdate () {
		transform.position = camera.transform.position;
		transform.rotation = camera.transform.rotation;
	}
}
