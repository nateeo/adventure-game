using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour {

	public void endGame() {
		SceneManager.LoadScene (0);
	}
}
