using UnityEngine;
using System.Collections;

/**
 * Used to show and hide the clues depending which is triggered by a collider
 * that is specified by the gameobject this script is attached to.
 */
public class ShowRoomClue : MonoBehaviour {

    public GameObject clue1;
    public GameObject clue2;
    
	void Start () {
        // Hide clues initially
        hideClues();
	}

    private void showClues()
    {
        clue1.SetActive(true);
        clue2.SetActive(true);
    }

    private void hideClues()
    {
        clue1.SetActive(false);
        clue2.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        showClues();
    }

    private void OnTriggerStay(Collider other)
    {
        showClues();
    }

    private void OnTriggerExit(Collider other)
    {
        hideClues();
    }
}
