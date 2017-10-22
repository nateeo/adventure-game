using UnityEngine;
using System.Collections;

public class InitialActive : MonoBehaviour {

	// Hides the game object initially
	void Start () {
        gameObject.SetActive(false);
	}
}
