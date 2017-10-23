using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class hideoutScript : MonoBehaviour {

	public Text pickupTooltip;
	private string text = "Press 'F' to pickup";
	GameObject[] objects;
	GameObject wall;
	GameObject player;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		objects = GameObject.FindGameObjectsWithTag("hideoutPickup");
		wall = GameObject.FindGameObjectWithTag ("hideoutWall");
		pickupTooltip.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		foreach(GameObject target in objects) {
			if (target != null) {
				float distance = Vector3.Distance (target.transform.position, player.transform.position);
				if (distance <= 5) {
					pickupTooltip.enabled = true;
					pickupTooltip.text = text;
					return;
				}
			}
		}
		Debug.Log (Vector3.Distance (wall.transform.position, player.transform.position));
		if (wall != null && Vector3.Distance (wall.transform.position, player.transform.position) <= 5) {
			pickupTooltip.enabled = true;
			pickupTooltip.text = "I don't know if this is safe... I think I should ask around first.";
			return;
		}
		if (pickupTooltip.enabled) {
			pickupTooltip.enabled = false;
		}
	}
}
