using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class skipPlanetOneCheatCode : MonoBehaviour {

	public int sceneToSkipTo;
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.DownArrow)) {
			SceneManager.LoadScene (sceneToSkipTo);
		}
	}
}
