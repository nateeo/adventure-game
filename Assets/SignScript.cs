using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SignScript : MonoBehaviour {

	// to display on the user UI
	public Text signText;
	public string text;
	public float signDistance;

	void Start() {

	}

	void Update() {
		
		if (Vector3.Distance (transform.position, Camera.main.transform.position) < signDistance) {
			if (!signText.enabled) {
				signText.enabled = true;
				signText.text = text;
			}
		} else {
			// reset and hide sign text
			if (signText.enabled) {
				signText.text = "";
				signText.enabled = false;
			}
		}
	}
}
