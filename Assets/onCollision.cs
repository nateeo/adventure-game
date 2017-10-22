using UnityEngine;
using System.Collections;

public class onCollision : MonoBehaviour {
	private Rigidbody rb;
	bool icy = false;
	bool icyMovement;
	private GameObject playerCharacter;
	PlayerScript player;
	UIManager uimanage;
	CapsuleCollider collider;


	void Update() {
		playerCharacter = GameObject.FindWithTag ("Player");
		player = playerCharacter.GetComponent<PlayerScript> ();
		rb = playerCharacter.GetComponent<Rigidbody> ();
		collider = playerCharacter.GetComponent<CapsuleCollider> ();

		if (Input.GetKeyDown (KeyCode.P) && icy == false) {
			icy = true;
		} else if (Input.GetKeyDown (KeyCode.P) && icy == true) {
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
	}

}