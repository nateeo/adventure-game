using UnityEngine;
using System.Collections;

public class InitialSetActive : MonoBehaviour {

	public bool setActive;
	// Use this for initialization
	void Start () {
		gameObject.SetActive(setActive);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
