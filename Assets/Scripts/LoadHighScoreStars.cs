using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadHighScoreStars : MonoBehaviour {

	public Text name;
	public int playerRank;

	// Use this for initialization
	void Start () {
		int highScore;
		string scorekey = "HighScore" + playerRank;
		if (PlayerPrefs.HasKey (scorekey)) {
			highScore = PlayerPrefs.GetInt (scorekey);
		} else {
			Debug.Log("Non Existing " +scorekey);
			highScore = 0;
		}
		name.text = highScore.ToString();
	}
}
