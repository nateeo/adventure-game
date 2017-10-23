using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour {

	void Start() {
		PlayerPrefs.SetInt ("sceneIndex", 7);
	}

	public void endGame() {
		SceneManager.LoadScene (2);
	}
}
