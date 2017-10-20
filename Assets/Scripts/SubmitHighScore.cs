using UnityEngine;
using System.Collections;

public class SubmitHighScore : MonoBehaviour {

	int score;
	string name;

	// Use this for initialization
	void Start () {
		int timeScore;
		if (PlayerPrefs.HasKey("TimeScore"))
		{
			timeScore = PlayerPrefs.GetInt("TimeScore");
		}
		else
		{
			Debug.Log("Non Existing Key");
			timeScore = 0;
		}

		int bonusScore;
		if (PlayerPrefs.HasKey("BonusScore"))
		{
			bonusScore = PlayerPrefs.GetInt("BonusScore");
		}
		else
		{
			Debug.Log("Non Existing Key");
			bonusScore = 0;
		}

		score = bonusScore + timeScore;
		name = "james";
	}
	
	// This function is called to submit a score to the high score menu.
	public void sumbitScore () {

		int highScore1 = fetchHighScore("HighScore1");
		int highScore2 = fetchHighScore("HighScore2");
		int highScore3 = fetchHighScore("HighScore3");

		string highScoreName1 = fetchHighScoreName("HighScoreName1");
		string highScoreName2 = fetchHighScoreName("HighScoreName2");
		string highScoreName3 = fetchHighScoreName("HighScoreName3");

		Debug.Log (highScore1);
		Debug.Log (highScore2);
		Debug.Log (highScore3);

		if (score > highScore1) {
			PlayerPrefs.SetInt ("HighScore1", score);
			PlayerPrefs.SetString ("HighScoreName1", name);
			PlayerPrefs.SetInt ("HighScore2", highScore1);
			PlayerPrefs.SetString ("HighScoreName2", highScoreName1);
			PlayerPrefs.SetInt ("HighScore3", highScore2);
			PlayerPrefs.SetString ("HighScoreName3", highScoreName2);
		} else if (score > highScore2) {
			PlayerPrefs.SetInt ("HighScore2", score);
			PlayerPrefs.SetString ("HighScoreName2", name);
			PlayerPrefs.SetInt ("HighScore3", highScore2);
			PlayerPrefs.SetString ("HighScoreName3", highScoreName2);
		} else if (score > highScore3) {
			PlayerPrefs.SetInt ("HighScore3", score);
			PlayerPrefs.SetString ("HighScoreName3", name);
		}
	}

	//TODO
	void clearScores() {

	}


	// A helper function that sets the first two inputs based on the values saved in the player
	// prefs.
	private int fetchHighScore(string scorekey) {
		int highScore;
		if (PlayerPrefs.HasKey (scorekey)) {
			highScore = PlayerPrefs.GetInt (scorekey);
		} else {
			Debug.Log("Non Existing " +scorekey);
			highScore = 0;
		}
		return highScore;
	}

	private string fetchHighScoreName(string namekey) {
		string highScoreName;
		if (PlayerPrefs.HasKey (namekey)) {
			highScoreName = PlayerPrefs.GetString (namekey);
		} else {
			Debug.Log("Non Existing " +namekey);
			highScoreName = "No Player yet";
		}
		return highScoreName;
	}
}
