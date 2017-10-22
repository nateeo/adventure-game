using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SignScript : MonoBehaviour {

	// to display on the user UI
	public Text signText;
	public string text;
	public float signDistance;
	static int inRange;
	bool activeSelf;

	void Start() {
		activeSelf = false;
	}

	void Update() {
		
		if (Vector3.Distance (transform.position, Camera.main.transform.position) < signDistance) {
			if (!activeSelf) {
				activeSelf = true;
				inRange++;
				signText.text = text;
			}
		} else {
			if (activeSelf) {
				activeSelf = false;
				inRange--;
				if (inRange == 0) {
					signText.text = "";
				}
			}
		}
	}
}
