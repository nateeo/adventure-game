using UnityEngine;
using System.Collections;

public class inventoryPersist : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject pinventory = GameObject.FindGameObjectWithTag ("PersistedInventory");
		Debug.Log ("pinventory");
		Debug.Log (pinventory);
		Canvas canvas = pinventory.GetComponentInChildren<Canvas> ();
		GameObject playerObj = GameObject.FindGameObjectWithTag ("Player");
		Debug.Log ("player");
		Debug.Log (playerObj);
		GameObject inventory = canvas.transform.Find ("Panel - Inventory(Clone)").gameObject;
		Debug.Log ("inventory");
		Debug.Log (inventory);
		playerObj.GetComponent<PlayerInventory> ().inventory = inventory;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
