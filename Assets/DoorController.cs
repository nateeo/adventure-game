using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text;
using System.Linq;

//Use this class for controlling the open and closing of doors
public class DoorController : MonoBehaviour
{
    // fields for door scripts
    public PuzzleDoors doorA;
    public PuzzleDoors doorB;
    public PuzzleDoors doorC;
    public PuzzleDoors doorD;
    public PuzzleDoors doorE;
    public PuzzleDoors doorF;
    public PuzzleDoors doorG;
    List<PuzzleDoors> doors = new List<PuzzleDoors>();

    //fields for buttons
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;
    public Button button6;
    public Button button7;
    List<Button> buttons = new List<Button>();

    //field for the text displaying the buttons pressed
    public Text indicator;


    List<int> mappings = new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7 });
    List<int> pressedList = new List<int>();


    // Use this for initialization
    void Start()
    {
        indicator.text = "";
        mappings.Shuffle();
        //add to the button list.
        buttons.Add(button1);
        buttons.Add(button2);
        buttons.Add(button3);
        buttons.Add(button4);
        buttons.Add(button5);
        buttons.Add(button6);
        buttons.Add(button7);

        //add to the doors list.
        doors.Add(doorA);
        doors.Add(doorB);
        doors.Add(doorC);
        doors.Add(doorD);
        doors.Add(doorE);
        doors.Add(doorF);
        doors.Add(doorG);
    }

    //Call this function when a button is being clicked
    public void onClick()
    {
        var buttonClicked = EventSystem.current.currentSelectedGameObject;
        string nameOfButton;
        if (buttonClicked != null)
        {
            nameOfButton = buttonClicked.name;
            
        }
        else
        {
            //Unexpected behaviour here. A button should be clicked.
            Debug.Log("Error: button not clicked?");
            return;
        }

        //Reset button pressed, clear all buttons pressed, close all doors.
        if (nameOfButton.Equals("Reset"))
        {
            //First, clear all the numbers pressed, unhighlight all buttons.
            pressedList.Clear();
            updateText();
            buttons.ForEach(b => b.GetComponent<Image>().color = Color.white);
            doors.ForEach(d => d.closeDoor());
        }
        else
        {
            string number = nameOfButton.Substring(nameOfButton.Length - 1);
            int numberPressed = int.Parse(number);

            if (pressedList.Contains(numberPressed)) //Contains a number pressed already.
            {
                return;
            }

            if (pressedList.Count == 3) //Pressed three buttons already
            {
                return;
            }
            
            else //otherwise append this button to the pressed list.
            {
                buttons[numberPressed-1].GetComponent<Image>().color = Color.red;
                pressedList.Add(numberPressed);
                updateText();
                if (pressedList.Count == 3)//If now three buttons are pressed, open the doors.
                {
                    openDoors();
                } 
            }
        }
    }

    //Private helper method for updating the text indicator of what has been pressed.
    private void updateText()
    {
        StringBuilder sb = new StringBuilder();
        foreach (int pressed in pressedList)
        {
            sb.Append(pressed);
            sb.Append(" ");
        }
        indicator.text = sb.ToString();
    }

    //Pirrivate helper method for opening doors.
    //This method should be called when three buttons are pressed.
    private void openDoors()
    {
        //Lambda expression to map the buttons pressed to which doors it corresponds in the mappings list
        //Then call open door on each of these indexes.
        List<int> indexesOfDoors = pressedList.Select(p => mappings.IndexOf(p)).ToList();
        indexesOfDoors.ForEach(i => doors[i].openDoor());
    }
}

//This is an extension class used to shuffle a list. 
static class MyExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

