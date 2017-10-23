using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TooltipScript : MonoBehaviour {

	public Text tooltip;
	public GameObject player;

	// Use this for initialization
	void Start () {
		tooltip.enabled = false;
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance (player.transform.position, gameObject.transform.position) < 5) {
			tooltip.enabled = true;
		} else if (tooltip.enabled) {
			tooltip.enabled = false;
		}
	}
}
