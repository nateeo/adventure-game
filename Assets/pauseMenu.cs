using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * Pause menu which stops the game and allows the player to return to the main menu
 */
public class pauseMenu : MonoBehaviour {

	public UIManager _diagUI;

	GameObject _panel;
	bool _paused;
	// Use this for initialization
	void Start () {
		_panel = this.gameObject.transform.GetChild(0).gameObject;
		_panel.SetActive (false);
		_paused = false;
	}
	
	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (_paused) {
				setPauseState (false);
			} else {
				setPauseState (true);
			}
		}
	}

	// for the resume button we manually unpause
	public void Unpause() {
		setPauseState (false);
	}

	public void ReturnToMainMenu() {
		Time.timeScale = 1;
		SceneManager.LoadScene (0);
	}

	private void setPauseState(bool shouldPause) {
		_paused = shouldPause;
		if (shouldPause) {
			_panel.SetActive (true);
			Time.timeScale = 0;
			_diagUI.interfaceOpen ();
		} else {
			_panel.SetActive (false);
			Time.timeScale = 1;
			_diagUI.interfaceClosed ();
		}
	}
}
