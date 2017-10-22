using UnityEngine;
using System.Collections;

public class CloseCanvas : MonoBehaviour {

    public GameObject canvas;

	public void closeCanvas()
    {
        canvas.SetActive(false);
    }
}
