using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// This class is used to clear the high scores that are saved in the Player prefabs.
public class ClearHighScores : MonoBehaviour {

	public Text firstName;
	public Text secondName;
	public Text thirdName;

	public Text firstStar;
	public Text secondStar;
	public Text thirdStar;

	// Checks if the score has been set in the Playerprefs if they have been delete and set
	// The texts to Noone and 0.
	public void clearScores() {
		if (PlayerPrefs.HasKey ("HighScore1")) {
			PlayerPrefs.DeleteKey ("HighScore1");
			firstName.text = "Noone";
		}

		if (PlayerPrefs.HasKey ("HighScore2")) {
			PlayerPrefs.DeleteKey ("HighScore2");
			secondName.text = "Noone";
		}
	
		if(PlayerPrefs.HasKey("HighScore3")){
			PlayerPrefs.DeleteKey ("HighScore3");
			thirdName.text = "Noone";
		}

		if (PlayerPrefs.HasKey ("HighScoreName1")) {
			PlayerPrefs.DeleteKey ("HighScoreName1");
			firstStar.text = "0";
		}

		if (PlayerPrefs.HasKey ("HighScoreName2")) {
			PlayerPrefs.DeleteKey ("HighScoreName2");
			secondStar.text = "0";
		}

		if (PlayerPrefs.HasKey ("HighScoreName3")) {
			PlayerPrefs.DeleteKey ("HighScoreName3");
			thirdStar.text = "0";
		}
	}
}
