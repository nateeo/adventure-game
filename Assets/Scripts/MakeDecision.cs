using UnityEngine;
using System.Collections;

// This class is used to decide which text to display after a decision has made. 
public class MakeDecision : MonoBehaviour {

	// Decision text that will be referenced for setting visibility.
	public GameObject decisionText;
	public GameObject decisionCanvas;
	public GameObject cutsceneCanvas;

	public void displayText() {
		cutsceneCanvas.SetActive(false);
		decisionText.SetActive(true);
		decisionCanvas.SetActive(true);
	}
}
