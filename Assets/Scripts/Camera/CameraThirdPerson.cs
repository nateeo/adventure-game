using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraThirdPerson : MonoBehaviour {

	public bool dialogFix = false;

	private const float Y_ANGLE_MIN = 0f;
	private const float Y_ANGLE_MAX = 50.0f;
	public Transform lookAt;
	public Transform camTransform;

	private Camera cam;

	private float distance = 10.0f;
	private float currentX = 0.0f;
	private float currentY = 0.0f;
	private float sensitivityX = 8.0f;
	private float sensitivityY = 8.0f;

	// Use this for initialization
	void Start () {
		camTransform = transform;
		cam = Camera.main;
	}

	void Update() {
		if (dialogFix) {
			transform.position.Set(transform.position.x, transform.position.y, lookAt.position.z - distance);
			return;
		}
		currentX += sensitivityX * Input.GetAxis ("Mouse X");
		currentY += sensitivityY * Input.GetAxis ("Mouse Y");

		currentY = Mathf.Clamp (currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (dialogFix) {
			return;
		}
		float camDistance = currentY < 10f ? 5f : distance;
		Vector3 dir = new Vector3 (0, 0, -camDistance);
		Vector3 camVec = Input.mousePosition;
		Quaternion rotation = Quaternion.Euler (currentY, camVec.x, 0);
		camTransform.position = lookAt.position + rotation * dir;
		camTransform.LookAt (lookAt.position);
	}
}
