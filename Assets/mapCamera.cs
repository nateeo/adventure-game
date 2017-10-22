using UnityEngine;
using System.Collections;

public class mapCamera : MonoBehaviour {
	public GameObject camera;
	bool visible;
	public bool icyPuzzle = false;

	// Use this for initialization
	void Start () {
		camera.SetActive(false);
		visible = false;
	}

	// Update is called once per frame
	void Update () {
		if(icyPuzzle == true && visible == false) {
			camera.SetActive(true);
			Cursor.lockState = CursorLockMode.Locked;
			visible = true;
		} else if (icyPuzzle == false && visible == true) {
			camera.SetActive(false);
			visible = false;
			Cursor.lockState = CursorLockMode.None;


		}
	}
}
