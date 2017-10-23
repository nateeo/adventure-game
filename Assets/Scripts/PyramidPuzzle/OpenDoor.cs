using UnityEngine;

/**
 * Opens to door if a collision has occurred
 */
public class OpenDoor : MonoBehaviour {

    public GameObject gameState;
    private bool hasOpened = false;
    
    private void OnTriggerEnter(Collider other)
    {
        bool answer = gameState.GetComponent<SubmitPasscode>().getAnswer();
        // Only opens the door once when the question has been answered correctly
        if (!hasOpened && answer)
        {
            gameObject.transform.Rotate(0, 90, 0);
        }
        // Make sure it is only opened once
        if (!hasOpened)
        {
            hasOpened = true;
        }
    }
}
