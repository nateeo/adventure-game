using UnityEngine;
using System.Collections;

/**
 * Opens to door if a collision has occurred
 */
public class OpenDoor : MonoBehaviour {

    private bool hasOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        // Only opens the door once when the question has been answered correctly
        if (!hasOpened)
        {
            gameObject.transform.Rotate(0, 90, 0);
        }
    }
}
