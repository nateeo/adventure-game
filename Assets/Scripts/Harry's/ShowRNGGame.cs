using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowRNGGame : MonoBehaviour {

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

    // Use this for initialization
    void Start () {
        walls = initializeWalls();
	}

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

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < walls.Count; i++)
        {
            walls[i].SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < walls.Count; i++)
        {
            walls[i].SetActive(false);
        }
    }
}
