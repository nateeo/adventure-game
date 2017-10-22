using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SubmitExitPuzzle : MonoBehaviour {

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
        if (Input.GetKey(KeyCode.Return))
        {
            checkAnswer();
        }
    }

    private void generateAnswers()
    {
        answers.Add("apepi");
        answers.Add("shebitku");
        answers.Add("takelot");
        answers.Add("tutankhamen");
    }

    public void checkAnswer()
    {
        bool correct = true;
        if (!answer1.text.ToLower().Equals(answers[0].ToLower()))
        {
            correct = false;
        }
        if (!answer2.text.ToLower().Equals(answers[1].ToLower()))
        {
            correct = false;
        }
        if (!answer3.text.ToLower().Equals(answers[2].ToLower()))
        {
            correct = false;
        }
        if (!answer4.text.ToLower().Equals(answers[3].ToLower()))
        {
            correct = false;
        }

        if (correct)
        {
            canvas.SetActive(false);
            exitPuzzle.GetComponent<ExitGameState>().setOverallRoomState(true);
            UIManager.GetComponent<UIManager>().interfaceClosed();
        } else
        {
            responseText.text = "Wrong, try again!";
            UIManager.GetComponent<UIManager>().interfaceOpen();
        }
    }
}
