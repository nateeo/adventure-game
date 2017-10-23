using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExitPuzzle : MonoBehaviour
{
    public GameObject canvas;
    public GameObject exitPuzzle;
    public GameObject UIManager;
    public GameObject room1Clue1;
    public GameObject room1Clue2;
    public GameObject room2Clue1;
    public GameObject room2Clue2;
    public GameObject room3Clue1;
    public GameObject room3Clue2;
    public GameObject room4Clue1;
    public GameObject room4Clue2;
    public GameObject trueRoomClue;

    private List<string> trueClueList = new List<string>();
    private List<string> falseClueList = new List<string>();
    private List<List<GameObject>> clues = new List<List<GameObject>>();
    
    void Start()
    {
        initialiseClues();
        hideClues();

        int[] trueRooms = setTrueRooms();
        setClues(trueRooms);
    }

    /**
     * Adds the text clues to the over-arching list of clues.
     * Also initializes the list of true and false clues.
     */
    private void initialiseClues()
    {
        // Add the clues to a list for all the rooms
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

        // Adding a list of true and false clues
        trueClueList.Add("Neheb < Takelot");
        trueClueList.Add("Wazner < Khayu");
        trueClueList.Add("Neheb < Khayu");
        trueClueList.Add("Wazner < Takelot");

        falseClueList.Add("Takelot < Wazner");
        falseClueList.Add("Khayu < Wazner");
        falseClueList.Add("Takelot < Neheb");
        falseClueList.Add("Khayu < Neheb");
    }

    /**
     * Hides the created list of clues
     */
    private void hideClues()
    {
        for (int i = 0; i < clues.Count; i++)
        {
            for (int j = 0; j < clues[i].Count; j++)
            {
                clues[i][j].SetActive(false);
            }
        }
    }

    /**
     * Randomnly generates 2 true and 2 false rooms. Furthermore, sets the string name for
     * Room1's validity.
     * A true room is indicated by a value of one in which the rooms value corresponds to the
     * position in the array.
     */
    private int[] setTrueRooms()
    {
        int[] trueRooms = { 0, 0, 0, 0 };
        int setRooms = 0;

        // Loop until 2 unique rooms have been set to true
        while (setRooms < 2)
        {
            // A random number corresponding to a room number
            int random = Random.Range(0, 3);
            if (trueRooms[random] == 0)
            {
                trueRooms[random] = 1;
                setRooms++;
            }
        }

        // Create a string that shows whether Room1 is true or false
        string clue = "";
        if (trueRooms[0] == 0)
        {
            clue = "false";
        } else
        {
            clue = "true";
        }
        setTrueRoomClue(clue);

        return trueRooms;
    }

    /**
     * Sets the clue for Room1.
     */
    private void setTrueRoomClue(string answer)
    {
        trueRoomClue.GetComponent<TextMesh>().text = "Room 1 is " + answer + "!";
    }

    /**
     * Using the int[] list of true rooms, it sets the text of each room depending on
     * whether it is true or false
     */
    private void setClues(int[] trueRooms)
    {
        int trueCount = 0;
        int falseCount = 0;

        // Loop through all 4 
        for (int i = 0; i < trueRooms.Length; i++)
        {
            // Get the text component for the clues inside a room
            GameObject clue1 = clues[i][0];
            GameObject clue2 = clues[i][1];
            TextMesh t1 = clue1.GetComponent<TextMesh>();
            TextMesh t2 = clue2.GetComponent<TextMesh>();
            
            // Setting the text for the clues depending on whether the room is true or false
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
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only show the question if it hasn't been answered or wrongly answered
        if (!exitPuzzle.GetComponent<ExitGameState>().getOverallRoomState())
        {
            canvas.SetActive(true);
            UIManager.GetComponent<UIManager>().interfaceOpen();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Hide the question if the player moves away from it
        canvas.SetActive(false);
        UIManager.GetComponent<UIManager>().interfaceClosed();
    }
}