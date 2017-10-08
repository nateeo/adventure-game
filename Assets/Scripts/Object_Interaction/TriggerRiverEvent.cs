using UnityEngine;
using System.Collections;

public class TriggerRiverEvent : MonoBehaviour {

	public GameObject text;
	public GameObject log1;
	PickUpObject pickUpShit;
	GameObject playerController;
	Collider playerCollider;

	// Use this for initialization
	void Start () {

		playerController = GameObject.FindWithTag ("Player");
		pickUpShit = playerController.GetComponent<PickUpObject>();
		playerCollider = playerController.GetComponent<Collider>();
		text.SetActive (false);
		log1.SetActive (false);
	}

	
	// Update is called once per frame
	void OnTriggerStay(Collider player) {
		Debug.Log (player);
		if(player == playerCollider) {
			text.SetActive (true);
			GameObject go = pickUpShit.getCarriedObject ();
			if(go != null) {
				Log l = go.GetComponent<Log>();
				if (l != null) {
					if (Input.GetKeyDown (KeyCode.G)) {
						pickUpShit.dropObject ();
						go.SetActive (false);
						log1.SetActive (true);
					}
				}
			}
		}
	}

	void OnTriggerExit() {
		text.SetActive(false);
	}
}
