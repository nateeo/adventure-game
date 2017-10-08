using UnityEngine;
using System.Collections;

public class TriggerRiverEvent : MonoBehaviour {

	public GameObject text;
	public GameObject log1;
	PickUpObject pickUpShit;

	// Use this for initialization
	void Start () {

		pickUpShit = GameObject.FindWithTag ("Player").GetComponent<PickUpObject>();
		text.SetActive (false);
		log1.SetActive (false);
	}
	
	// Update is called once per frame
	void OnTriggerStay(Collider player) {

		GameObject go = pickUpShit.getCarriedObject ();
		if(go != null) {
			
			Log l = go.GetComponent<Log>();
			if (l != null) {
				text.SetActive (true);
				if (Input.GetKeyDown (KeyCode.G)) {
					pickUpShit.dropObject ();
					go.SetActive (false);
					log1.SetActive (true);
				}
			}
		}
	}

	void OnTriggerExit() {
		text.SetActive(false);
	}
}
