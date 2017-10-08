using UnityEngine;
using System.Collections;
using VIDE_Data;

public class examplePlayer : MonoBehaviour
{
    //This script handles player movement and interaction with other NPC game objects

    //Reference to our diagUI script for quick access
    public exampleUI diagUI;
    public QuestChartDemo questUI;
    public Animator blue;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //Only allow player to move and turn if there are no dialogs loaded
        if (!VD.isActive)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * 5, 0);
            float move = Input.GetAxisRaw("Vertical");
            transform.position += transform.forward * 7 * move * Time.deltaTime;
            blue.SetFloat("speed", move);
        }
        //Interact with NPCs when hitting spacebar
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
        //Hide/Show cursor
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.visible = !Cursor.visible;
            if (Cursor.visible)
                Cursor.lockState = CursorLockMode.None;
                else
                Cursor.lockState = CursorLockMode.Locked;
        }
    }

    //Casts a ray to see if we hit an NPC and, if so, we interact
    void TryInteract()
    {
        RaycastHit rHit;

        if (Physics.Raycast(transform.position, transform.forward, out rHit, 2))
        {
            //In this example, we will try to interact with any collider the raycast finds

            //Lets grab the NPC's DialogueAssign script... if there's any
            VIDE_Assign assigned;
            if (rHit.collider.GetComponent<VIDE_Assign>() != null)
                assigned = rHit.collider.GetComponent<VIDE_Assign>();
            else return;
                   

            if (!VD.isActive)
            {
                //... and use it to begin the conversation
                if (assigned.alias == "NonDialogue")
                {
                    questUI.CallQuestChart();
                }
                else
                {
                    diagUI.Begin(assigned);
                }
            }
            else
            {
                //If conversation already began, let's just progress through it
                if (assigned.alias == "NonDialogue")
                {
                    questUI.CallQuestChart();
                }
                else
                {
                    diagUI.CallNext();
                }
            }

        }
    }
}
