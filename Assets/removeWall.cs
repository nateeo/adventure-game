using UnityEngine;
using System.Collections;

public class removeWall : MonoBehaviour {

	public GameObject box;
	public GameObject startBox;
	void OnTriggerExit(Collider collider) {
		Debug.Log ("WORKS");
		box.SetActive (false);
		onCollision col = startBox.GetComponent<onCollision> ();
		col.icyPuzzle = false;
	}
}
