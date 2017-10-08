using UnityEngine;
using System.Collections;

public class SignScript : MonoBehaviour {
	MeshRenderer renderer;

	void Start() {
		renderer = GetComponent<MeshRenderer> ();
		renderer.enabled = false;
	}

	void Update() {
		
		if (Vector3.Distance (renderer.transform.position, Camera.main.transform.position) < 30.0f) {
			if (renderer.enabled == false) {
				renderer.enabled = true;
			}
		} else {
			if (renderer.enabled == true) {
				renderer.enabled = false;
			}
		}
	}
}
