using UnityEngine;
using System.Collections;

public class OpenDoor : MonoBehaviour {

    private bool hasOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasOpened)
        {
            gameObject.transform.Rotate(0, 90, 0);
        }
    }
}
