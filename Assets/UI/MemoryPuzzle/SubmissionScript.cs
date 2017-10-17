using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//This script will be used when the submission button is clicked.
//It is attached on the status (Text) gameObject.
//Author: Victor
public class SubmissionScript : MonoBehaviour {
    public Text statusText;
    [SerializeField] Transform slots;
    private string CorrectSequence = "DFBACE"; //This is the correct sequence to enter!

    public void onClick()
    {
        string sequence = getSequenceEntered();
        if (sequence.Contains("0"))//sequence has empty slots.
        {
            sequenceIncomplete();
        } else if (!sequence.Equals(CorrectSequence)) //sequence not correct.
        {
            sequenceIncorrect();
        } else //sequence is correct. 
        {
            sequenceCorrect();
        }
    }

    //This function gets the sequence of letters entered in the slots.
    //If there are slots that are empty, the string "0" will be used to fill it. 
    private string getSequenceEntered()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        foreach(Transform slotTransform in slots)
        {
            GameObject item = slotTransform.GetComponent<Slot>().item;
            if (item)
            {
                builder.Append(item.name);
            }
            else {
                builder.Append("0");
            }
        }
        return builder.ToString();
    }

    //This function is called when there are empty slots.
    private void sequenceIncomplete()
    {
        statusText.text = "All slots must be filled.";
    }

    //This function is called when incorrect sequence is entered.
    private void sequenceIncorrect()
    {
        statusText.text = "Incorrect sequence.";
    }

    //This function is called when there are empty slots.
    private void sequenceCorrect()
    {
        statusText.text = "Correct!";
    }
}
