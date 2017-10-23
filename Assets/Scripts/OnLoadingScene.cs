using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class OnLoadingScene : MonoBehaviour {
	public GameObject fullStar0;
	public GameObject fullStar1;
	public GameObject fullStar2;
	public GameObject fullStar3;
	public GameObject fullStar4;
	public GameObject fullStar5;
	public Text time1;
	public Text time2;
	public Text time3;
	private List<GameObject> listOfStars = new List<GameObject>();

	public Text timeText;
	public Text bonusText;


	// Use this for initialization
	void Start () {
		listOfStars.Add(fullStar0);
		listOfStars.Add(fullStar1);
		listOfStars.Add(fullStar2);
		listOfStars.Add(fullStar3);
		listOfStars.Add(fullStar4);
		listOfStars.Add(fullStar5);

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
			Debug.Log ("What is the bonnus Score "+bonusScore);
		}
		else
		{
			Debug.Log("Non Existing Key");
			bonusScore = 0;
		}

		float time;
		if (PlayerPrefs.HasKey("Time"))
		{
			time = PlayerPrefs.GetFloat("Time");
		}
		else
		{
			Debug.Log("Non Existing Key");
			time = 0.0f;
		}

		int bonus;
		if (PlayerPrefs.HasKey("Bonus"))
		{
			bonus = PlayerPrefs.GetInt("Bonus");
		}
		else
		{
			Debug.Log("Non Existing Key");
			bonus = 0;
		}

		int maxBonus;
		if (PlayerPrefs.HasKey("MaxBonus"))
		{
			maxBonus = PlayerPrefs.GetInt("MaxBonus");
		}
		else
		{
			Debug.Log("Non Existing Key");
			maxBonus = 0;
		}

		if (PlayerPrefs.HasKey ("TimeFor1Star")) {
			time1.text = PlayerPrefs.GetString ("TimeFor1Star");
		}

		if (PlayerPrefs.HasKey ("TimeFor2Star")) {
			time2.text = PlayerPrefs.GetString ("TimeFor2Star");
		}

		if (PlayerPrefs.HasKey ("TimeFor3Star")) {
			time3.text = PlayerPrefs.GetString ("TimeFor3Star");
		}

		Debug.Log("TIME" + timeScore + "BONUS" + bonusScore);

		changeStar(timeScore, bonusScore);
		changeValues(time, bonus, maxBonus);

		Debug.Log ("total score" + (timeScore + bonusScore));

	}

	//private helper method for changing number of stars (score).
	//use only 0-3 stars for both integers.
	private void changeStar(int firstRow, int secondRow)
	{
		if (firstRow < 0 || firstRow > 3 || secondRow < 0 || secondRow > 3)
		{
			throw new System.ArgumentException("the number of stars for both rows must be between 0-3.");
		}
		else
		{
			for (int i = 0; i < 3; i++)
			{
				if (i < firstRow)
				{
					listOfStars[i].SetActive(true);
				}
				else
				{
					listOfStars[i].SetActive(false);
				}
			}

			for (int i = 3; i < 6; i++)
			{
				if (i < secondRow + 3)
				{
					listOfStars[i].SetActive(true);
				}
				else
				{
					listOfStars[i].SetActive(false);
				}
			}
		}
	}

	//Private helper method for changing the values displayed on the score screen.
	private void changeValues(float timeInSec, int bonus, int maxBonus)
	{
		int minutes = ((int)timeInSec / 60);
		int seconds = (int)(timeInSec % 60);

		string minToDisplay = minutes.ToString("00");
		string secToDisplay = seconds.ToString("00");

		timeText.text = minToDisplay + ":" + secToDisplay;
		bonusText.text = bonus + " / " + maxBonus;
	}

	// Update is called once per frame
	void Update () {

	}
}
