using UnityEngine;
using System.Collections;

public class RiverHint : MonoBehaviour {

	public GameObject text;
	GameObject playerController;
	Collider playerCollider;
	PickUpObject pickUpItem;

	// Use this for initialization
	void Start () {
		playerController = GameObject.FindWithTag ("Player");
		pickUpItem = playerController.GetComponent<PickUpObject> ();
		playerCollider = playerController.GetComponent<Collider> ();
		text.SetActive(false);
	}
	
	// Update is called once per frame
	void OnTriggerStay (Collider player) {
		if(player == playerCollider) {
			GameObject go = pickUpItem.getCarriedObject ();
			if (go == null) {
				text.SetActive(true);
			}
		}
	}

	void OnTriggerExit () {
		text.SetActive(false);
	}
}
