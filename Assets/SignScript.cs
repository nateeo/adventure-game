using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SignScript : MonoBehaviour {

	// to display on the user UI, singleton signText
	public Text signText;
	public string text;
	public float signDistance;
	GameObject player;
	static int inRange;
	bool activeSelf;

	void Start() {
		player = GameObject.FindGameObjectWithTag ("Player");
		activeSelf = false;
	}

	void Update() {

		if (Vector3.Distance (transform.position, player.transform.position) < signDistance) {
			if (!activeSelf) {
				activeSelf = true;
				inRange++;
				signText.text = text;
			}
		} else {
			// disable the signText if no other signs are in range
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
