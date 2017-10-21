using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExitPuzzle : MonoBehaviour
{

    public GameObject trueClue1;
    public GameObject trueClue2;
    public GameObject trueClue3;
    public GameObject trueClue4;
    public GameObject falseClue1;
    public GameObject falseClue2;
    public GameObject falseClue3;
    public GameObject falseClue4;
    public GameObject Plugman1;

    private List<GameObject> trueClues = new List<GameObject>();
    private List<GameObject> falseClues = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        initialiseClues();
        hideClues();
    }

    private void initialiseClues()
    {
        trueClues.Add(trueClue1);
        trueClues.Add(trueClue2);
        trueClues.Add(trueClue3);
        trueClues.Add(trueClue4);
        falseClues.Add(falseClue1);
        falseClues.Add(falseClue2);
        falseClues.Add(falseClue3);
        falseClues.Add(falseClue4);
    }

    private void hideClues()
    {
        for (int i = 0; i < trueClues.Count; i++)
        {
            trueClues[i].SetActive(false);
            falseClues[i].SetActive(false);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Entered");
        Plugman1.transform.position = trueClue1.transform.position;

    }
}