using UnityEngine;
using System.Collections;

public class ResetAccumulatedScore : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		PlayerPrefs.SetInt ("AccumulatedScore", 0);
	}
}
