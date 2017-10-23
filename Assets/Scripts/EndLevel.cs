using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour {

	public PlayerScore ps;

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.RightBracket)) {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.Confined;
			ps.endSceneAndDisplayScore ();
		}
	}
}
