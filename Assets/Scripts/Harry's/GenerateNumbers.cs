using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateNumbers : MonoBehaviour {

    public GameObject numberWall1;
    public GameObject numberWall2;
    public GameObject numberWall3;
    public GameObject numberWall4;
    private List<GameObject> walls;
    private string passcode;

	// Use this for initialization
	void Start () {
        walls = initializeWalls();

        List<int> numbers = generateNumbers(walls.Count);
        passcode = createPasscode(numbers);
        plotWalls(walls, numbers);
	}

    private List<GameObject> initializeWalls()
    {
        List<GameObject> tempWalls = new List<GameObject>();
        tempWalls.Add(numberWall1);
        tempWalls.Add(numberWall2);
        tempWalls.Add(numberWall3);
        tempWalls.Add(numberWall4);
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

    private List<int> generateNumbers(int numberOfWalls)
    {
        List<int> numbers = new List<int>();
        for (int i = 0; i < numberOfWalls; i++)
        {
            numbers.Add(Random.Range(0, 10));
        }
        return numbers;
    }

    private void plotWalls(List<GameObject> walls, List<int> numbers)
    {
        Debug.Log("HELLOOOO");
        // Plotting a single wall
        for (int i = 0; i < walls.Count; i++)
        {
            GameObject wall = walls[i];
            TextMesh t = wall.AddComponent<TextMesh>();
            t.text = numbers[i].ToString();
            //t.fontSize = 150;

            Debug.Log("KMS");

            t.transform.localEulerAngles += new Vector3(0, 90, 0);
            //t.transform.position = wall.transform.position;
            //t.transform.position = GameObject.Find("NumberWallRight" + (i + 1)).transform.position;
            Debug.Log(t.transform.position);
        }
    }

    private string createPasscode(List<int> numbers)
    {
        numbers.Sort();
        string passcode = "";
        for (int i = 0; i < numbers.Count; i++)
        {
            passcode += numbers[i].ToString();
        }
        return passcode;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.transform.position);
        Debug.Log(other.gameObject.name);
        gameObject.transform.Rotate(-180, 0, 0);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exiting");
        gameObject.transform.Rotate(180, 0, 0);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Entered");
    }
}
