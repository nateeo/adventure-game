using UnityEngine;
using System.Collections;

public class CloseCanvas : MonoBehaviour {

    public GameObject canvas;
    public GameObject UIManager;

	public void closeCanvas()
    {
        canvas.SetActive(false);
        UIManager.GetComponent<UIManager>().interfaceClosed();
    }
}
