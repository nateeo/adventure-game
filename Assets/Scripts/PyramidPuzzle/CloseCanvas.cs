using UnityEngine;
using System.Collections;

public class CloseCanvas : MonoBehaviour {

    public GameObject canvas;
    public GameObject UIManager;

    /**
     * Closes the question screen if the "X" button is clicked
     */
	public void closeCanvas()
    {
        canvas.SetActive(false);
        UIManager.GetComponent<UIManager>().interfaceClosed();
    }
}
