using UnityEngine;
using System.Collections;

public class TriggerRiverEvent : MonoBehaviour
{

	public GameObject text;
	public GameObject othertext;
	public GameObject notText;
	public GameObject log;
	public GameObject blockMesh;
	bool logPlaced;
	PickUpObject pickUpItem;
	GameObject playerController;
	Collider playerCollider;

	// Use this for initialization
	void Start ()
	{
		logPlaced = false;
		playerController = GameObject.FindWithTag ("Player");
		pickUpItem = playerController.GetComponent<PickUpObject> ();
		playerCollider = playerController.GetComponent<Collider> ();
		text.SetActive (false);
		log.SetActive (false);
	}

	
	// Update is called once per frame
	void OnTriggerStay (Collider player)
	{
		if (player == playerCollider && logPlaced == false) {
			GameObject go = pickUpItem.getCarriedObject ();
			if (go != null) {
				pickUpItem.setZone(true);
				Log l = go.GetComponent<Log> ();
				notText.SetActive (false);
				othertext.SetActive (false);
				if (l != null) {
					text.SetActive (true);
					if (Input.GetKeyDown (KeyCode.G)) {
						pickUpItem.dropObject ();
						go.SetActive (false);
						log.SetActive (true);
						blockMesh.SetActive (false);
						notText.SetActive (false);
						othertext.SetActive (false);
						text.SetActive(false);
						logPlaced = true;
					}
				}
			}
		}
	}

	void OnTriggerExit ()
	{
		text.SetActive (false);
		pickUpItem.setZone(false);
	}
}
