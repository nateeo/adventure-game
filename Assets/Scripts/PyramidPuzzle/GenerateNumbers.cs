using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateNumbers : MonoBehaviour {

    public GameObject UIManager;
    public GameObject passcodeScreen;
    public GameObject passcodeButton;
    public GameObject number1;
    public GameObject number2;
    public GameObject number3;
    public GameObject number4;
    public GameObject number5;
    public GameObject number6;
    public GameObject number7;
    public GameObject number8;
    public GameObject number9;
    public GameObject number10;
    public GameObject number11;
    public GameObject number12;

    private List<GameObject> walls;
    private string passcode;
    
	void Start () {
        // Set and hide the walls
        walls = initializeWalls();
        hideWalls();

        // Generate the random numbers that will be used as the passcode
        List<int> numbers = generateNumbers(walls.Count);
        int[] isPassCode = chooseNumbers();
        passcode = createPasscode(numbers, isPassCode);
        
        // Plot the generated numbers to the walls
        plotWalls(walls, numbers, isPassCode);
        passcodeButton.GetComponent<SubmitPasscode>().setPasscode(passcode);
	}

    /**
     * Initialize the walls into a temporary list to work with
     */
    private List<GameObject> initializeWalls()
    {
        List<GameObject> tempWalls = new List<GameObject>();
        tempWalls.Add(number1);
        tempWalls.Add(number2);
        tempWalls.Add(number3);
        tempWalls.Add(number4);
        tempWalls.Add(number5);
        tempWalls.Add(number6);
        tempWalls.Add(number7);
        tempWalls.Add(number8);
        tempWalls.Add(number9);
        tempWalls.Add(number10);
        tempWalls.Add(number11);
        tempWalls.Add(number12);
        return tempWalls;
    }

    public void hideWalls()
    {
        for (int i = 0; i < walls.Count; i++)
        {
            walls[i].SetActive(false);
        }
    }

    public void showWalls()
    {
        for (int i = 0; i < walls.Count; i++)
        {
            walls[i].SetActive(true);
        }
    }

    public string getPasscode()
    {
        return passcode;
    }

    /**
     * Generates a random list of random numbers between 0 and 9.
     * The amount of walls that are generated depends on the numberOfWalls
     */
    private List<int> generateNumbers(int numberOfWalls)
    {
        List<int> numbers = new List<int>();
        for (int i = 0; i < numberOfWalls; i++)
        {
            numbers.Add(Random.Range(0, 10));
        }
        return numbers;
    }

    /**
     * Choose 4 random numbers from the list of generated numbers. 
     * A value of 1 indicates that the number has been chosen.
     */
    private int[] chooseNumbers()
    {
        int[] chosenNumbers = new int[12];

        int numbersChosen = 0;
        // Loop until 4 numbers have been chosen
        while (numbersChosen < 4)
        {
            int number = Random.Range(0, 9);
            if (chosenNumbers[number] == 0)
            {
                chosenNumbers[number] = 1;
                numbersChosen++;
            }
        }

        return chosenNumbers;
    }

    /**
     * Creates a string representation of the chosen passcode numbers
     */
    private string createPasscode(List<int> numbers, int[] isPassCode)
    {
        // Temporary list that will sort the 4 passcode numbers
        List<int> unsortedPasscode = new List<int>();

        for (int i = 0; i < numbers.Count; i++)
        {
            if (isPassCode[i] == 1)
            {
                unsortedPasscode.Add(numbers[i]);
            }
        }

        // Sort the numbers in ascending order and create a string representation
        unsortedPasscode.Sort();
        string passCode = "";
        for (int i = 0; i < 4; i++)
        {
            passCode += unsortedPasscode[i].ToString();
        }

        return passCode;
    }

    /**
     * Plots the number on the wall as text
     */
    private void plotWalls(List<GameObject> walls, List<int> numbers, int[] isPasscode)
    {
        for (int i = 0; i < walls.Count; i++)
        {
            // Get the text component for each wall
            GameObject wall = walls[i];
            TextMesh t = wall.GetComponent<TextMesh>();
            t.text = numbers[i].ToString();

            // Plot the text in grey if the number is in the passcode, white if not
            if (isPasscode[i] == 1)
            {
                t.color = Color.grey;
            } else
            {
                t.color = Color.white;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!passcodeButton.GetComponent<SubmitPasscode>().getAnswer())
        {
            passcodeScreen.SetActive(true);
            UIManager.GetComponent<UIManager>().interfaceOpen();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        passcodeScreen.SetActive(false);
        UIManager.GetComponent<UIManager>().interfaceClosed();
    }
}
