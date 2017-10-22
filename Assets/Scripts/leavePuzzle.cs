using UnityEngine;
using System.Collections;

public class leavePuzzle : MonoBehaviour {
	private Rigidbody rb;
	bool icy = false;
	bool icyMovement;
	private GameObject playerCharacter;
	PlayerScript player;
	UIManager uimanage;
	CapsuleCollider collider;
	public bool icyPuzzle = false;


	void Update() {

		if (icyPuzzle == true && icy == false) {
			icy = true;
		} else if (icyPuzzle == false && icy == true) {
			icy = false;
		}

	}
	void OnCollisionEnter ( Collision collision) {
		//populate fields
		playerCharacter = GameObject.FindWithTag ("Player");
		player = playerCharacter.GetComponent<PlayerScript> ();
		rb = playerCharacter.GetComponent<Rigidbody> ();
		collider = playerCharacter.GetComponent<CapsuleCollider> ();

		if (icy == false) {
			//do not do anything if icy is false
			return;
		}
		if (collision.collider.ToString() == "Udo 1 (UnityEngine.BoxCollider)") {
		
			//return movement to normal
			player.dialogFix = false;
			collider.enabled = true;

		}

		//remove this barrier
		icyPuzzle = false;
		rb.isKinematic = false;
		Destroy (this);
	}

}