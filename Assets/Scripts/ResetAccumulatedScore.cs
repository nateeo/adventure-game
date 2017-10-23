using UnityEngine;
using System.Collections;

public class ResetAccumulatedScore : MonoBehaviour {

	// Use this for initialization
	void Start () {
		PlayerPrefs.SetInt ("AccumulatedScore", 0);
	}
}
