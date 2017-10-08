using UnityEngine;
using System.Collections;

public class TriggerRiverEvent : MonoBehaviour {

	public GameObject text;
	public GameObject othertext;
	public GameObject notText;
	public GameObject log;
	PickUpObject pickUpShit;
	GameObject playerController;
	Collider playerCollider;

	// Use this for initialization
	void Start () {

		playerController = GameObject.FindWithTag ("Player");
		pickUpShit = playerController.GetComponent<PickUpObject>();
		playerCollider = playerController.GetComponent<Collider>();
		text.SetActive (false);
		log.SetActive (false);
	}

	
	// Update is called once per frame
	void OnTriggerStay(Collider player) {
		Debug.Log ("Debug me please");
		Debug.Log (player);
		if(player == playerCollider) {
			text.SetActive (true);
			GameObject go = pickUpShit.getCarriedObject ();
			if(go != null) {
				Log l = go.GetComponent<Log>();
				notText.SetActive (false);
				othertext.SetActive (false);
				if (l != null) {
					if (Input.GetKeyDown (KeyCode.G)) {
						pickUpShit.dropObject ();
						go.SetActive (false);
						log.SetActive (true);
						notText.SetActive (false);
						othertext.SetActive (false);
					}
				}
			}
		}
	}

	void OnTriggerExit() {
		text.SetActive(false);
	}
}
