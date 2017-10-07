using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class OnLoadingScene : MonoBehaviour {
    public GameObject fullStar0;
    public GameObject fullStar1;
    public GameObject fullStar2;
    public GameObject fullStar3;
    public GameObject fullStar4;
    public GameObject fullStar5;
    private List<GameObject> listOfStars = new List<GameObject>();


    // Use this for initialization
    void Start () {
        listOfStars.Add(fullStar0);
        listOfStars.Add(fullStar1);
        listOfStars.Add(fullStar2);
        listOfStars.Add(fullStar3);
        listOfStars.Add(fullStar4);
        listOfStars.Add(fullStar5);

        int timeScore;
        int bonusScore;

        if (PlayerPrefs.HasKey("TimeScore"))
        {
            timeScore = PlayerPrefs.GetInt("TimeScore");
            bonusScore = PlayerPrefs.GetInt("BonusScore");
        }
        else
        {
            Debug.Log("Non Existing Key");
            timeScore = 0;
        }

        if (PlayerPrefs.HasKey("BonusScore"))
        {
            timeScore = PlayerPrefs.GetInt("TimeScore");
            bonusScore = PlayerPrefs.GetInt("BonusScore");
        }
        else
        {
            Debug.Log("Non Existing Key");
            bonusScore = 0;
        }

        Debug.Log("TIME" + timeScore + "BONUS" + bonusScore);

        changeStar(timeScore, bonusScore);

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

    // Update is called once per frame
    void Update () {

    }
}
