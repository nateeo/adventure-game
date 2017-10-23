using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class persistedCanvas : MonoBehaviour {

	public GameObject childGameObject;

	// Use this for initialization
	void Start () {
	
	}

	void Awake() {
		//DontDestroyOnLoad (transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (SceneManager.GetActiveScene ().buildIndex == 0) {
			Destroy (gameObject);
		} else if (SceneManager.GetActiveScene ().buildIndex == 2) {
			childGameObject.SetActive (false);
		} else {
			childGameObject.SetActive (true);
		}
	}
}
