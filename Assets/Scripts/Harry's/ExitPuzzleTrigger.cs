using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExitPuzzleTrigger : MonoBehaviour {

    public GameObject roomLabel1;
    public GameObject roomLabel2;
    public GameObject roomLabel3;
    public GameObject roomLabel4;

    private List<GameObject> roomText;
    
    void Start () {
        roomText = initializeWalls();
        hideWalls();
	}

    private List<GameObject> initializeWalls()
    {
        List<GameObject> roomLabels = new List<GameObject>();
        roomLabels.Add(roomLabel1);
        roomLabels.Add(roomLabel2);
        roomLabels.Add(roomLabel3);
        roomLabels.Add(roomLabel4);
        return roomLabels;
    }

    private void showWalls()
    {
        for (int i = 0; i < roomText.Count; i++)
        {
            roomText[i].SetActive(true);
        }
    }

    private void hideWalls()
    {
        for (int i = 0; i < roomText.Count; i++)
        {
            roomText[i].SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        showWalls();
    }

    void OnTriggerExit(Collider other)
    {
        hideWalls();
    }
}
