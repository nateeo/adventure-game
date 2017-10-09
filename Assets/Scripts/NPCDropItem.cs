using UnityEngine;
using System.Collections;

public class NPCDropItem : MonoBehaviour
{

    public GameObject item;
    public int keyPressTrigger;
    private int keyPressed = 0;
    
    void Start()
    {
        item.SetActive(false);
    }

    public void Update()
    {
        // Check if F has been pressed
        if (Input.GetKeyDown(KeyCode.F) == true)
        {
            Debug.Log(keyPressed);
            keyPressed++;
            // Check if the conversation between the user and NPC is finished
            if (keyPressed >= keyPressTrigger)
            {
                // Display the item and reset the keycount
                showItem();
                resetKeyCount();
            }
        }
    }

    public void showItem()
    {
        item.SetActive(true);
        item.AddComponent<PickUpItem>();
    }

    private void resetKeyCount()
    {
        keyPressed = 0;
    }
}