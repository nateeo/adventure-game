using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadHighScoreName : MonoBehaviour {

	public Text name;
	public int playerRank;

	// Use this for initialization
	void Start () {
		string highScoreName;
		string namekey = "HighScoreName" + playerRank;
		if (PlayerPrefs.HasKey (namekey)) {
			highScoreName = PlayerPrefs.GetString (namekey);
		} else {
			Debug.Log("Non Existing " +namekey);
			highScoreName = "No Player yet";
		}
		name.text = highScoreName;
	}
}
