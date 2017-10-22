using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SubmitNPCQuestion : MonoBehaviour {

    public GameObject UIManager;
    public GameObject exitPuzzleState;
    public GameObject passcodeScreen;
    public GameObject canvas;
    public GameObject door1;
    public GameObject door2;
    public GameObject door3;
    public GameObject door4;
    public Text answerInput;
    public Text questionText;
    public Text responseText;

    private string answer;
    private int roomNumber;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            onAnswerEnter();
        }
    }

    public void onAnswerEnter()
    {
        answerInput.text.ToLower();
        string userAnswer = answerInput.text.ToLower();
        if (userAnswer.Equals(answer.ToLower()))
        {
            if (roomNumber == 1)
            {
                exitPuzzleState.GetComponent<ExitGameState>().setRoom1State(true);
                door1.transform.Rotate(0, 80, 0);
            }
            if (roomNumber == 2)
            {
                exitPuzzleState.GetComponent<ExitGameState>().setRoom2State(true);
                door2.transform.Rotate(0, 80, 0);
            }
            if (roomNumber == 3)
            {
                exitPuzzleState.GetComponent<ExitGameState>().setRoom3State(true);
                door3.transform.Rotate(0, -80, 0);
            }
            if (roomNumber == 4)
            {
                exitPuzzleState.GetComponent<ExitGameState>().setRoom4State(true);
                door4.transform.Rotate(0, -80, 0);
            }
            canvas.SetActive(false);
            UIManager.GetComponent<UIManager>().interfaceClosed();
        }
        else
        {
            responseText.text = "Wrong, try again!";
            UIManager.GetComponent<UIManager>().interfaceOpen();
        }
    }

    public void setQuestion(string NPCQuestion)
    {
        questionText.text = NPCQuestion;
    }

    public void setAnswer(string NPCAnswer)
    {
        answer = NPCAnswer;
    }

    public void setRoomNumber(int NPCRoomNumber)
    {
        roomNumber = NPCRoomNumber;
    }
}
