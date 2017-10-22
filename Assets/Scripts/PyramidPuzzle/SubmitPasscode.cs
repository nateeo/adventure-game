using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SubmitPasscode : MonoBehaviour {

    public GameObject UIManager;
    public GameObject passcodeScreen;
    public GameObject door;
    public Text responseText;
    public Text passcodeInput1;
    public Text passcodeInput2;
    public Text passcodeInput3;
    public Text passcodeInput4;

    private string passcode;
    private bool hasAnswered = false;

    private void Update()
    {
        // Checks to see whether the user has submitted using the enter button
        if (Input.GetKey(KeyCode.Return))
        {
            onPassCodeEnter();
        }
    }

    /**
     * Called to check whether the user input matches the passcode answer
     */
    public void onPassCodeEnter()
    {
        // The user input will be a combination of the 4 input fields
        string passcodeInput = "";
        passcodeInput += passcodeInput1.text;
        passcodeInput += passcodeInput2.text;
        passcodeInput += passcodeInput3.text;
        passcodeInput += passcodeInput4.text;

        // If the correct answer is given, the door opens leading them to the extra clue
        if (passcodeInput.Equals(passcode))
        {
            door.transform.Rotate(0, 80, 0);
            passcodeScreen.SetActive(false);
            hasAnswered = true;
            UIManager.GetComponent<UIManager>().interfaceClosed();
        } else
        {
            responseText.text = "Wrong, try again!";
        }
    }

    public void setPasscode(string realPasscode)
    {
        passcode = realPasscode;
    }

    public bool getAnswer()
    {
        return hasAnswered;
    }
}
