using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class loadPlanet3Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F)) {
			SceneManager.LoadScene (5);
		}
	}
}
