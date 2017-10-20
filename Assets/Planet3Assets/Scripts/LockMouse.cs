using UnityEngine;
using System.Collections;

public class LockMouse : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
