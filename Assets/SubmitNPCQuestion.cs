using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SubmitNPCQuestion : MonoBehaviour {

    public GameObject exitPuzzleState;
    public GameObject passcodeScreen;
    public GameObject canvas;
    public GameObject door1;
    public GameObject door2;
    public GameObject door3;
    public GameObject door4;
    public Text answerInput;
    public Text questionText;
    string answer;
    int roomNumber;

    private void Start()
    {

    }

    public void onAnswerEnter()
    {
        Debug.Log("Answer: " + answer);
        Debug.Log("Your answer: " + answerInput);
        answerInput.text.ToLower();
        string userAnswer = answerInput.text.ToLower();
        if (userAnswer.Equals(answer.ToLower()))
        {
            Debug.Log("Correct");
            if (roomNumber == 1)
            {
                exitPuzzleState.GetComponent<ExitGameState>().setRoom1State(true);
                door1.transform.Rotate(0, -80, 0);
            }
            if (roomNumber == 2)
            {
                exitPuzzleState.GetComponent<ExitGameState>().setRoom2State(true);
                door2.transform.Rotate(0, -80, 0);
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
        }
        else
        {
            Debug.Log("Wrong");
        }
    }

    public void setQuestion(string NPCQuestion)
    {
        Debug.Log("Question");
        questionText.text = NPCQuestion;
    }

    public void setAnswer(string NPCAnswer)
    {
        Debug.Log("Answer");
        answer = NPCAnswer;
    }

    public void setRoomNumber(int NPCRoomNumber)
    {
        roomNumber = NPCRoomNumber;
    }
}
