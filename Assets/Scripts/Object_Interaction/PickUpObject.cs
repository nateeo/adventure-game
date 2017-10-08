using UnityEngine;
using System.Collections;

public class PickUpObject : MonoBehaviour {
	
	GameObject mainCamera;
	GameObject playerCharacter;
	bool carrying;
	bool inZone;
	GameObject carriedObject;
	public float distance;
	public float smooth;
	public GameObject text; 
	public GameObject dropText;
	// Use this for initialization
	void Start () {
		mainCamera = GameObject.FindWithTag("MainCamera");
		playerCharacter = GameObject.FindWithTag ("Player");
		text.SetActive (false);
		dropText.SetActive (false);
	}

	// Update is called once per frame
	void Update () {
		if(carrying) {
			if(!inZone) {
				dropText.SetActive(true);
			}
			carry(carriedObject);
			checkDrop();
			//rotateObject();
		}
	}

	void rotateObject() {
		carriedObject.transform.Rotate(5,10,15);
	}

	void carry(GameObject o) {
		Debug.Log ("I am carrying");
		Debug.Log (playerCharacter.transform.position);
		o.transform.position = playerCharacter.transform.position + new Vector3(1,0,1);
		Debug.Log (o.transform.position);
		o.transform.rotation = Quaternion.identity;
	}

	void OnTriggerStay(Collider pickUpObject) {
		if(!carrying && (pickUpObject.GetComponent<Log>() != null)) {
			text.SetActive (true);
			if(Input.GetKeyDown (KeyCode.F)) {
				Debug.Log("Hi there I don't know what I;m doing");
				Pickupable p = pickUpObject.GetComponent<Pickupable>();
				if(p != null) {
					Debug.Log ("carry that shit?");
					carrying = true;
					carriedObject = p.gameObject;
					//p.gameObject.rigidbody.isKinematic = true;
					p.gameObject.GetComponent<Rigidbody>().useGravity = false;
					text.SetActive (false);
				}
			}
		}
	}

	void OnTriggerExit(Collider pickUpObject) {
		Pickupable p = pickUpObject.GetComponent<Pickupable>();
		if(p != null) {
			text.SetActive(false);
		}
	}

	void checkDrop() {
		if(Input.GetKeyDown (KeyCode.E)) {
			dropObject();
		}
	}

	public void dropObject() {
		carrying = false;
		//carriedObject.gameObject.rigidbody.isKinematic = false;
		carriedObject.transform.position = playerCharacter.transform.position + new Vector3(1,0,1);
		carriedObject.gameObject.GetComponent<Rigidbody>().useGravity = true;
		carriedObject = null;
		dropText.SetActive (false);
	}

	public GameObject getCarriedObject() {
		return carriedObject;
	}

	public void setZone(bool val) {
		inZone = val;
	}
}