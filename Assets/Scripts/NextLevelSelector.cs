using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class NextLevelSelector : MonoBehaviour {

	int sceneIndex;
	// Use this for initialization
	void Start () {
		if (PlayerPrefs.HasKey ("sceneIndex")) {
			int lastPlayedSceneIndex = PlayerPrefs.GetInt ("sceneIndex");

			// If our last world that we played was Planet 1
			if (lastPlayedSceneIndex == 1) {
				// Load planet 2 which has scene index of 12
				sceneIndex = 12;
			}

			if (lastPlayedSceneIndex == 12) {
				sceneIndex = 6;
				PlayerPrefs.SetString ("SpawnMine","false");
			}
		} else {
			Debug.Log ("no scene index");
		}
	}
	
	public void NextLevel() {
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		SceneManager.LoadScene (sceneIndex);
	}
}
