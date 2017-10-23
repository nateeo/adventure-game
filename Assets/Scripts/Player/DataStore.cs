using UnityEngine;
using System.Collections;


// Temporary storage for dialogue / interaction flags
using System.Collections.Generic;


public class DataStore : MonoBehaviour {

	Dictionary<string, string> dictionary;

	// Use this for initialization
	void Start () {
		dictionary = new Dictionary<string, string>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void add(string key, string value) {
		dictionary.Add (key, value);
	}

	public string get(string key) {
		if (dictionary.ContainsKey (key)) {
			return dictionary [key];
		}
		return null;
	}
}
