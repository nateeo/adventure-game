using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class submitDestinationScript : MonoBehaviour {

	public UIManager manager;
	public PlayerScore ps;
	public Text destinationInput;
	public Text responseText;

	
	public void onDestinationEnter(){
		string userInput = "";
		userInput = destinationInput.text;

		if(userInput.Trim().Equals("Khaloof") || userInput.Trim().Equals("khaloof")) 
		{
			manager.interfaceClosed ();
			ps.endSceneAndDisplayScore ();

		} else 
		{
			responseText.text = "Sorry, this is not a valid destination! Enter a destination...";
		}
	}
	
}
