using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class finishScene : MonoBehaviour {

	public int sceneIndex;
	public GameObject text;
	GameObject player;
    public PlayerScore ps;


	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		text.SetActive (false);
	}
	
	// Update is called once per frame
	void OnTriggerStay(Collider col) {
		if (col == player.GetComponent<Collider> ()) {
			text.SetActive (true);
			if(Input.GetKeyDown(KeyCode.F)) {
				Cursor.lockState = CursorLockMode.Confined;
				Cursor.visible = true;
                ps.endSceneAndDisplayScore();
				text.SetActive (false);
				Destroy (text);
				Destroy (this);
			}
		}
	}

	void OnTriggerExit() {
		text.SetActive (false);
	}
}
