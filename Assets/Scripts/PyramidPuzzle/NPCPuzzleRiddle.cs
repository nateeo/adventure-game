using UnityEngine;
using System.Collections;

/**
 * Script attached to an NPC if it contains a question for the player to answer.
 * It calls the NPCQuestion canvas to display the relevant contents to the player.
 */
public class NPCPuzzleRiddle : MonoBehaviour {

    public GameObject door;
    public GameObject canvas;
    public GameObject UIManager;
    public GameObject questionButton;
    public GameObject exitPuzzleState;
    public string question;
    public string answer;
    public int roomNumber;

    private int moveCount = 0;
    private bool hasAnswered = false;

    /**
     * Sets the question, answer, and the corresponding room number for the canvas
     */
    private void setCanvasFields()
    {
        // Makes a reference to the canvas
        questionButton.GetComponent<SubmitNPCQuestion>().setQuestion(question);
        questionButton.GetComponent<SubmitNPCQuestion>().setAnswer(answer);
        questionButton.GetComponent<SubmitNPCQuestion>().setRoomNumber(roomNumber);
    }
   
    /**
     * Triggered if the user approaches an NPC to answer a question
     */
    private void OnTriggerEnter(Collider other)
    {
        setCanvasFields();
        // Shows the canvas depending on the room number that corresponds to the NPC
        if (roomNumber == 1)
        {
            if (!exitPuzzleState.GetComponent<ExitGameState>().getRoom1State())
            {
                canvas.SetActive(true);
                UIManager.GetComponent<UIManager>().interfaceOpen();
            }
        }
        if (roomNumber == 2)
        {
            if (!exitPuzzleState.GetComponent<ExitGameState>().getRoom2State())
            {
                canvas.SetActive(true);
                UIManager.GetComponent<UIManager>().interfaceOpen();
            }
        }
        if (roomNumber == 3)
        {
            if (!exitPuzzleState.GetComponent<ExitGameState>().getRoom3State())
            {
                canvas.SetActive(true);
                UIManager.GetComponent<UIManager>().interfaceOpen();
            }
        }
        if (roomNumber == 4)
        {
            if (!exitPuzzleState.GetComponent<ExitGameState>().getRoom4State())
            {
                canvas.SetActive(true);
                UIManager.GetComponent<UIManager>().interfaceOpen();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        canvas.SetActive(false);
        UIManager.GetComponent<UIManager>().interfaceClosed();
    }
}
