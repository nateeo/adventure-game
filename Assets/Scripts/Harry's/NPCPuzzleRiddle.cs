using UnityEngine;
using System.Collections;

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

    private void setCanvasFields()
    {
        // Set question and answer and roomNo on canvas
        questionButton.GetComponent<SubmitNPCQuestion>().setQuestion(question);
        questionButton.GetComponent<SubmitNPCQuestion>().setAnswer(answer);
        questionButton.GetComponent<SubmitNPCQuestion>().setRoomNumber(roomNumber);
    }
   
    private void OnTriggerEnter(Collider other)
    {
        setCanvasFields();
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
