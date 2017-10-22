using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExitPuzzle : MonoBehaviour
{
    public GameObject canvas;
    public GameObject room1Clue1;
    public GameObject room1Clue2;
    public GameObject room2Clue1;
    public GameObject room2Clue2;
    public GameObject room3Clue1;
    public GameObject room3Clue2;
    public GameObject room4Clue1;
    public GameObject room4Clue2;

    private List<string> trueClueList = new List<string>();
    private List<string> falseClueList = new List<string>();
    private List<List<GameObject>> clues = new List<List<GameObject>>();

    // Use this for initialization
    void Start()
    {
        initialiseClues();
        hideClues();

        int[] trueRooms = setTrueRooms();
        setClues(trueRooms);
    }

    private void initialiseClues()
    {
        List<GameObject> room1 = new List<GameObject>();
        room1.Add(room1Clue1);
        room1.Add(room1Clue2);
        clues.Add(room1);

        List<GameObject> room2 = new List<GameObject>();
        room2.Add(room2Clue1);
        room2.Add(room2Clue2);
        clues.Add(room2);

        List<GameObject> room3 = new List<GameObject>();
        room3.Add(room3Clue1);
        room3.Add(room3Clue2);
        clues.Add(room3);

        List<GameObject> room4 = new List<GameObject>();
        room4.Add(room4Clue1);
        room4.Add(room4Clue2);
        clues.Add(room4);

        trueClueList.Add("Tutankhamen < Takelot");
        trueClueList.Add("Shebitku < Apepi");
        trueClueList.Add("Tutankhamen < Shebitku");
        trueClueList.Add("Takelot < Shebitku");

        falseClueList.Add("Takelot < Tutankhamen");
        falseClueList.Add("Shebitku < Takelot");
        falseClueList.Add("Apepi < Shebitku");
        falseClueList.Add("Shebitku < Tutankhamen");
    }

    private void hideClues()
    {
        for (int i = 0; i < clues.Count; i++)
        {
            for (int j = 0; j < clues[i].Count; j++)
            {
                Debug.Log("Hiding" + clues[i][j].name);
                clues[i][j].SetActive(false);
            }
        }
    }

    private int[] setTrueRooms()
    {
        int[] trueRooms = { 0, 0, 0, 0 };
        int setRooms = 0;
        while (setRooms < 2)
        {
            int random = Random.Range(0, 3);
            if (trueRooms[random] == 0)
            {
                trueRooms[random] = 1;
                setRooms++;
            }
        }
        return trueRooms;
    }

    private void setClues(int[] trueRooms)
    {
        int trueCount = 0;
        int falseCount = 0;

        for (int i = 0; i < trueRooms.Length; i++)
        {
            GameObject clue1 = clues[i][0];
            GameObject clue2 = clues[i][1];
            Debug.Log(clue1.name);
            Debug.Log(clue2.name);
            TextMesh t1 = clue1.GetComponent<TextMesh>();
            TextMesh t2 = clue2.GetComponent<TextMesh>();
            if (trueRooms[i] == 1)
            {
                t1.text = trueClueList[trueCount];
                trueCount++;
                t2.text = trueClueList[trueCount];
                trueCount++;
            } else
            {
                t1.text = falseClueList[falseCount];
                falseCount++;
                t2.text = falseClueList[falseCount];
                falseCount++;
            }
            Debug.Log(t1.text + trueRooms[i]);
            Debug.Log(t2.text + trueRooms[i]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        canvas.SetActive(true);
    }
}