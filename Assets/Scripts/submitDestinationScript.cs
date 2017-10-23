using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class submitDestinationScript : MonoBehaviour {

	public GameObject UIManager;
	public Text destinationInput;
	public Text responseText;

	
	public void onDestinationEnter(){
		string userInput = "";
		userInput = destinationInput.text;

		if(userInput.Equals("Khaloof") || userInput.Equals("khaloof")) 
		{
			responseText.text = "Lift off in 10 seconds...";
				// They can leave the planet
		} else 
		{
			responseText.text = "Sorry, this is not a valid destination! Enter a destination...";
		}
	}
	
}
