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
		playerCharacter = GameObject.FindWithTag ("Player");
		player = playerCharacter.GetComponent<PlayerScript> ();
		rb = playerCharacter.GetComponent<Rigidbody> ();
		collider = playerCharacter.GetComponent<CapsuleCollider> ();

		if (icyPuzzle == true && icy == false) {
			icy = true;
		} else if (icyPuzzle == false && icy == true) {
			icy = false;
		}

	}
	void OnCollisionEnter ( Collision collision) {

		if (icy == false) {
			return;
		}
		if (collision.collider.ToString() == "Udo 1 (UnityEngine.BoxCollider)") {

			//player.icyMovement = true;
			rb.isKinematic = true;
			player.dialogFix = false;
			Debug.Log (rb.isKinematic);
			collider.enabled = true;
			Debug.Log ("collider" + collider.enabled);

		}
		icyPuzzle = false;
		rb.isKinematic = false;
	}

}