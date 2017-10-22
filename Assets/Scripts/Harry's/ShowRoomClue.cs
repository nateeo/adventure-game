using UnityEngine;
using System.Collections;

public class ShowRoomClue : MonoBehaviour {

    public GameObject clue1;
    public GameObject clue2;
    
	void Start () {
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
