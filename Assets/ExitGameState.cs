﻿using UnityEngine;
using System.Collections;

public class ExitGameState : MonoBehaviour {

    private bool room1 = false;
    private bool room2 = false;
    private bool room3 = false;
    private bool room4 = false;

	public void setRoom1State(bool state)
    {
        room1 = state;
    }

    public bool getRoom1State()
    {
        return room1;
    }

    public void setRoom2State(bool state)
    {
        room2 = state;
    }

    public bool getRoom2State()
    {
        return room2;
    }

    public void setRoom3State(bool state)
    {
        room3 = state;
    }

    public bool getRoom3State()
    {
        return room3;
    }

    public void setRoom4State(bool state)
    {
        room4 = state;
    }

    public bool getRoom4State()
    {
        return room4;
    }
}
