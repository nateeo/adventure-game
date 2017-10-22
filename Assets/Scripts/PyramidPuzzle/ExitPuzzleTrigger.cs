using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Shows and hides the walls depending on whether or not the player
 * collides with the trigger associated with the gameobject this script
 * is attached to
 */
public class ExitPuzzleTrigger : MonoBehaviour {

    public GameObject roomLabel1;
    public GameObject roomLabel2;
    public GameObject roomLabel3;
    public GameObject roomLabel4;

    private List<GameObject> roomText;
    
    void Start () {
        roomText = initializeLabels();
        hideWalls();
	}

    /**
     * Creates a temporary list of labels for the walls
     */
    private List<GameObject> initializeLabels()
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
