using UnityEngine;
using System.Collections;

public class onCollision : MonoBehaviour {
	private Rigidbody rb;
	bool icy = false;
	bool icyMovement;
	private GameObject playerCharacter;
	PlayerScript player;

	void Update() {
		playerCharacter = GameObject.FindWithTag ("Player");
		PlayerScript player = playerCharacter.AddComponent<PlayerScript> ();


		if (Input.GetKeyDown (KeyCode.P) && icy == false) {
			Debug.Log ("hi icy");
			icy = true;
		} else if (Input.GetKeyDown (KeyCode.P) && icy == true) {
			Debug.Log ("bye icy");
			icy = false;
		}
			
	}
	void OnCollisionEnter ( Collision collision) {
		Debug.Log ("urgggghhh");
		Debug.Log (icy);


		if (icy == false) {
			Debug.Log ("not icy");

			return;
		}
		
		player.icyMovement = true;
		Debug.Log ("icyyyyy");

	}

}