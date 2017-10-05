//(HAVE VIDE_Data, VIDE_Assign, and MiniJSON_VIDE inside a 'Plugins' folder in the project root!) 

/* Delete this line and the one at the bottom 

#pragma strict
import VIDE_Data; //<--- Import to use easily call VD class

//This script handles player movement and interaction with other NPC game objects

//Reference to our diagUI script for quick access
var diagUI : JS_exampleUI;
var blue : Animator;

function Start()
{
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
}

function Update()
{
    //Only allow player to move and turn if there are no dialogs loaded
    if (!VD.isActive)
    {
        transform.Rotate(0, Input.GetAxis("Mouse X") * 5, 0);
        var move : float = Input.GetAxisRaw("Vertical");
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
function TryInteract()
{
    var rHit : RaycastHit;

    if (Physics.Raycast(transform.position, transform.forward, rHit, 2))
    {
        //In this example, we will try to interact with any collider the raycast finds

        //Lets grab the NPC's DialogueAssign script... if there's any
        var assigned : VIDE_Assign;
        if (rHit.collider.GetComponent.<VIDE_Assign>() != null)
            assigned = rHit.collider.GetComponent.<VIDE_Assign>();
        else return;
               

        if (!VD.isActive)
        {
            //... and use it to begin the conversation
            diagUI.Begin(assigned);
        }
        else
        {
            //If conversation already began, let's just progress through it
            diagUI.CallNext();
        }

    }
}

Delete this line and the one at the top */