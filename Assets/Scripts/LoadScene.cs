using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {

	public void LoadByIndex(int sceneIndex) {
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		SceneManager.LoadScene (sceneIndex);
	}
		
}
