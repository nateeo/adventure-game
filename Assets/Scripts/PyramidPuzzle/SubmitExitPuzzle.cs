using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SubmitExitPuzzle : MonoBehaviour {

    public GameObject door;
    public GameObject canvas;
    public GameObject UIManager;
    public GameObject submitButton;
    public GameObject exitPuzzle;
    public Text responseText;
    public Text answer1;
    public Text answer2;
    public Text answer3;
    public Text answer4;

    private List<string> answers = new List<string>();
    
	void Start () {
        generateAnswers();
	}

    private void Update()
    {
        // Checks whether the user presses enter to submit their question
        if (Input.GetKey(KeyCode.Return))
        {
            checkAnswer();
        }
    }

    /**
     * Generates the list of final answers for the puzzle
     */
    private void generateAnswers()
	{
		answers.Add("takelot");
		answers.Add("khayu");
		answers.Add("neheb");
        answers.Add("wazner");
    }

    /**
     * Checks whether or not the user inputs is equal to the answer
     */
    public void checkAnswer()
    {
        bool correct = true;
        if (!answer1.text.ToLower().Trim().Equals(answers[0].ToLower()))
        {
            correct = false;
        }
        if (!answer2.text.ToLower().Trim().Equals(answers[1].ToLower()))
        {
            correct = false;
        }
        if (!answer3.text.ToLower().Trim().Equals(answers[2].ToLower()))
        {
            correct = false;
        }
        if (!answer4.text.ToLower().Trim().Equals(answers[3].ToLower()))
        {
            correct = false;
        }
        // If it's clue, open the door for the player to exit the puzzle map
        if (correct)
        {
            canvas.SetActive(false);
            exitPuzzle.GetComponent<ExitGameState>().setOverallRoomState(true);
            UIManager.GetComponent<UIManager>().interfaceClosed();
            door.transform.Rotate(0, -80, 0);
        } else
        {
            responseText.text = "Wrong, try again!";
            UIManager.GetComponent<UIManager>().interfaceOpen();
        }
    }
}
