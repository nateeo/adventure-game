using UnityEngine;
using System.Collections;

// This function will spawn the player at the mine area. It will also reenable the gameObjects
public class SpawnMine : MonoBehaviour {

	string spawnAtMine;

	// Use this for initialization
	void Start () {
		Debug.Log("Start is being called");
		Debug.Log("Player prefs value: "+PlayerPrefs.GetString("SpawnMine"));
		if (PlayerPrefs.HasKey ("SpawnMine") && PlayerPrefs.GetString ("SpawnMine").Equals ("true")) {
			Debug.Log ("Key is contained");
			gameObject.transform.position = new Vector3 (100, 1, 882);
			PlayerPrefs.SetString ("SpawnMine", "false");
		} else {
			Debug.Log ("Not breaking because of the keysearch short circuit working");
			PlayerPrefs.SetString ("SpawnMine", "true");
		}
	}
}
