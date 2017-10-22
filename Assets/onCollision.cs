using UnityEngine;
using System.Collections;

public class onCollision : MonoBehaviour {

	//Get all fields needed to change
	private Rigidbody rb;
	bool icy = false;
	bool icyMovement;
	private GameObject playerCharacter;
	PlayerScript player;
	UIManager uimanage;
	CapsuleCollider collider;

	public bool icyPuzzle = false;//flag to know if in the puzzle or not


	void Update() {
		if (icyPuzzle == true && icy == false) {
			//puzzle starts hence must change collision mechanic
			icy = true;
		} else if (icyPuzzle == false && icy == true) {
			//puzzle ends change collision mechanic back
			icy = false;
		}

	}
	void OnCollisionEnter ( Collision collision) {
			if (icy == false) {
				return;
			}

		//get items to populate the fields
		playerCharacter = GameObject.FindWithTag ("Player");
		player = playerCharacter.GetComponent<PlayerScript> ();
		rb = playerCharacter.GetComponent<Rigidbody> ();
		collider = playerCharacter.GetComponent<CapsuleCollider> ();

		if (collision.collider.ToString() == "Udo 1 (UnityEngine.BoxCollider)") {

			//so the player is not affected by forces while moving
			rb.isKinematic = true;

			//allow access to the keyboard
			player.dialogFix = false;

			//enable the collider so the player does not go through walls
			collider.enabled = true;

		}
	}

}
