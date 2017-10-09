using UnityEngine;
using System.Collections;

public class quitGame : MonoBehaviour {

	//code to quit out of game using the exit button
	public void exitGame() {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit ();
		#endif
	}
}
	