using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TooltipScript : MonoBehaviour {

	public Text tooltip;
	public string text;
	private GameObject player;
	private static int near = 0;
	private bool thisActive;

	// Use this for initialization
	void Start () {
		thisActive = false;
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance (player.transform.position, gameObject.transform.position) < 3) {
			//enable tooltip within range
			if (!thisActive) {
				near++;
				thisActive = true;
				tooltip.text = text;
			}
		} else {
			// remove active if out of range
			if (thisActive) {
				near--;
				thisActive = false;
				if (near == 0) {
					tooltip.text = "";
				}
			}
		}
	}
}
