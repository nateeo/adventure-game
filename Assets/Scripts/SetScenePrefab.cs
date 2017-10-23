using UnityEngine;
using System.Collections;

public class SetScenePrefab : MonoBehaviour {

	public int currentIndex;

	// Use this for initialization
	void Start () {
		PlayerPrefs.SetInt ("sceneIndex", currentIndex);
	}
}
