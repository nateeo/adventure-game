using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadMineScene : MonoBehaviour {

	void OnTriggerEnter(Collider collider) {
		SceneManager.LoadScene (5);
	}
}
