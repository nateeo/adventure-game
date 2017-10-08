//(HAVE VIDE_Data, VIDE_Assign, and MiniJSON_VIDE inside a 'Plugins' folder in the project root!) 

/* Delete this line and the one at the bottom

#pragma strict
import System.Collections.Generic;
import System.Linq;
import System.Text;
import UnityEngine.UI;
import VIDE_Data; //<--- Import to use easily call VD class

//THIS IS ONLY A DEMO SCRIPT WITH A DEMO DESIGN THAT WORKS WELL FOR THIS CUSTOM PROJECT
//Yours might require a different design, and you are absolutely free to create one.
//Just make sure you read up on available methods, events, and variables in the Scripting API.


//This is script is only meant to be demonstrate various ways of handling data to create a Dialogue/UI Manager
//VIDE doesn't focus on the actual interface, but rather on the system and the data
//This script is basically handling the node data from nodeData in its own, customized way
//Creating a customized in-game Dialogue/UI manager is up to you
//Of course, you can absolutely use this script as a start point by adding, modifying, optimizing, or simplifying it to your needs.
//If you are experiencing strange behaviours or have any issues or questions, don't hesitate on contacting me at https://videdialogues.wordpress.com/contact/
 

//This script will handle everything related to dialogue interface
//It will use a VD class to load dialogues and retrieve node data



//These are just references to UI components and objects in the scene
private var npcText: Text;
private var npcName: Text;
private var playerText: Text;
private var itemPopUp: GameObject;
private var uiContainer: GameObject;
private var notIncluded: GameObject;

//We'll use these later
private var dialoguePaused: boolean = false;
private var animatingText: boolean = false;

//Player/NPC Image component
private var NPCSprite: Image;
private var PlayerSprite: Image;

//Demo variables
private var example_Items: List. < String > = new List. < String > ();
private var example_ItemInventory: List. < String > = new List. < String > ();

//We'll be using this to store the current player dialogue options
private var currentOptions: List. < Text > = new List. < Text > ();

function Start() {
    LoadUIReferences();
    VD.LoadDialogues(); //Load all dialogues to memory so that we dont spend time doing so later
}

function LoadUIReferences(){
    npcText =  GameObject.Find("NPCText").GetComponent.<Text>();
    npcName =  GameObject.Find("NPCLabel").GetComponent.<Text>();
    playerText =  GameObject.Find("playerComment").GetComponent.<Text>();
    itemPopUp =  GameObject.Find("itemContainer");
    uiContainer =  GameObject.Find("TextContainer");
    NPCSprite =  GameObject.Find("NPCImage").GetComponent.<Image>();
    PlayerSprite =  GameObject.Find("PlayerImage").GetComponent.<Image>();
    GameObject.Find("Player").GetComponent.<JS_demoPlayer>().diagUI = this;
    GameObject.Find("Player").GetComponent.<JS_demoPlayer>().blue = GameObject.Find("Player").transform.Find("Blue").GetComponent.<Animator>();
    var items = ["Knife", "Sword", "Mystical Rocket-Launcher", "Glorious shotgun", "Teddy Bear"];
    example_Items.AddRange(new List.<String> (items));
    GameObject.Find("itemContainer").SetActive(false);
    GameObject.Find("playerComment").SetActive(false);
    GameObject.Find("NPCText").transform.parent.gameObject.SetActive(false);
    GameObject.Find("TextContainer").SetActive(false);
    notIncluded = GameObject.Find("Help").transform.GetChild(1).gameObject;    
    notIncluded.SetActive(true);
}

//Just so we know when we finished loading all dialogues, then we unsubscribe
private function OnLoadedAction() {
    Debug.Log("Finished loading all dialogues");
    VD.OnLoaded -= OnLoadedAction;
}

function OnDisable() {
    //If the script gets destroyed, let's make sure we force-end the dialogue to prevent errors
    EndDialogue(null);
}

//This begins the conversation (Called by examplePlayer script)
public function Begin(diagToLoad: VIDE_Assign) {
    //Let's clean the NPC text variables
    npcText.text = "";
    npcName.text = "";

    //First step is to call BeginDialogue, passing the required VIDE_Assign component 
    //This will store the first Node data in VD.nodeData
    //But before we do so, let's subscribe to certain events that will allow us to easily
    //Handle the node-changes
    VD.OnActionNode += ActionHandler;
    VD.OnNodeChange += NodeChangeAction;
    VD.OnEnd += EndDialogue;

    SpecialStartNodeOverrides(diagToLoad); //This one checks for special cases when overrideStartNode could change right before starting a conversation

    VD.BeginDialogue(diagToLoad); //Begins conversation, will call the first OnNodeChange
    uiContainer.SetActive(true);
}

//Demo on yet another way to modify the flow of the conversation
private function SpecialStartNodeOverrides(diagToLoad: VIDE_Assign) {
    //Get the item from CrazyCap to trigger this one on Charlie
    if (diagToLoad.alias == "Charlie") {
        if (example_ItemInventory.Count > 0 && diagToLoad.overrideStartNode == -1)
            diagToLoad.overrideStartNode = 16;
    }
}

//Input related stuff (scroll through player choices and update highlight)
function Update() {
    //Lets just store the Node Data variable for the sake of fewer words
    var data = VD.nodeData;

    if (VD.isActive) //Only if
    {
        //Scroll through Player dialogue options
        if (!data.pausedAction && data.isPlayer) {
            if (Input.GetKeyDown(KeyCode.S)) {
                if (data.commentIndex < currentOptions.Count - 1)
                    data.commentIndex++;
            }
            if (Input.GetKeyDown(KeyCode.W)) {
                if (data.commentIndex > 0)
                    data.commentIndex--;
            }

            //Color the Player options. Blue for the selected one
            for (var i = 0; i < currentOptions.Count; i++) {
                currentOptions[i].color = Color.white;
                if (i == data.commentIndex) currentOptions[i].color = Color.yellow;
            }
        }
    }

    if (Input.anyKeyDown && notIncluded.activeSelf)
        notIncluded.SetActive(false);
}

//examplePlayer.cs calls this one to move forward in the conversation
public function CallNext() {
    //Let's not go forward if text is currently being animated, but let's speed it up.
    if (animatingText) {
        CutTextAnim();;
        return;
    }

    if (!dialoguePaused) //Only if
    {
        //We check for current extraData before moving forward to do special actions that affect the flow of the conversation
        //ExtraDataLookUp returns true if an action requires to skip VD.Next()
        //For example, it will be true when we receive an item
        if (ExtraVariablesLookUp(VD.nodeData, true)) return;

        VD.Next(); //We call the next node and populate nodeData with new data
        return;
    }

    //This will just disable the item popup if it is enabled
    if (itemPopUp.activeSelf) {
        dialoguePaused = false;
        itemPopUp.SetActive(false);
    }
}

//Another way to handle Action Nodes is to listen to the OnActionNode event, which sends the ID of the action node
private function ActionHandler(actionNodeID: int) {
    Debug.Log("ACTION TRIGGERED: " + actionNodeID.ToString());
}

//We listen to OnNodeChange to update our UI with each new nodeData
//This should happen right after calling VD.Next()
private function NodeChangeAction(data: VD.NodeData) {
    //Reset some variables
    npcText.text = "";
    npcText.transform.parent.gameObject.SetActive(false);
    playerText.transform.parent.gameObject.SetActive(false);
    PlayerSprite.sprite = null;
    NPCSprite.sprite = null;

    //Look for dynamic text change in extraData
    ExtraVariablesLookUp(data, false);

    //If this new Node is a Player Node, set the player choices offered by the node
    if (data.isPlayer) {
        //Set node sprite if there's any, otherwise try to use default sprite
        if (data.sprite != null)
            PlayerSprite.sprite = data.sprite;
        else if (VD.assigned.defaultPlayerSprite != null)
            PlayerSprite.sprite = VD.assigned.defaultPlayerSprite;

        SetOptions(data.comments);
        playerText.transform.parent.gameObject.SetActive(true);

    } else //If it's an NPC Node, let's just update NPC's text and sprite
    {
        //Set node sprite if there's any, otherwise try to use default sprite
        if (data.sprite != null) {
            //For NPC sprite, we'll first check if there's any "sprite" key
            //Such key is being used to apply the sprite only when at a certain comment index
            //Check CrazyCap dialogue for reference
            if (data.extraVars.ContainsKey("sprite")) {
                if (data.commentIndex == data.extraVars["sprite"])
                    NPCSprite.sprite = data.sprite;
                else
                    NPCSprite.sprite = VD.assigned.defaultNPCSprite; //If not there yet, set default dialogue sprite
            } else {
                NPCSprite.sprite = data.sprite;
            }
        } else if (VD.assigned.defaultNPCSprite != null)
            NPCSprite.sprite = VD.assigned.defaultNPCSprite;

        StartCoroutine("AnimateText", data);

        //If it has a tag, show it, otherwise show the dialogueName
        if (data.tag.Length > 0)
            npcName.text = data.tag;
        else
            npcName.text = VD.assigned.alias;

        npcText.transform.parent.gameObject.SetActive(true);
    }
}

//Check to see if there's Extra Variables and if so, we do stuff
private function ExtraVariablesLookUp(data: VD.NodeData, PreCall: boolean): boolean {
    //Don't conduct extra variable actions if we are waiting on a paused action
    if (data.pausedAction) return false;

    if (!data.isPlayer) //For player nodes
    {
        //Check for extra variables
        //This one finds a key named "item" which has the value of the item thats gonna be given
        //If there's an 'item' key, then we will assume there's also an 'itemLine' key and use it
        if (PreCall) //Checks that happen right before calling the next node
        {
            if (data.extraVars.ContainsKey("item") && !data.dirty)
                if (data.commentIndex == data.extraVars["itemLine"]) {
                    if (data.extraVars.ContainsKey("item++")) //If we have this key, we use it to increment the value of 'item' by 'item++'
                    {
                        var newVars: Dictionary. < String, Object > = data.extraVars; //Clone the current extraVars content
                        //var newItem : int = (newVars["item"]); //Retrieve the value we want to change
                        var newItem: int = VD.GetInt("item"); //Retrieve the value we want to change
                        var evItem: int = VD.GetInt("item++");
                        newItem += evItem; //Change it as we desire
                        newVars["item"] = newItem; //Set it back   
                        VD.SetExtraVariables(25, newVars); //Send newVars through UpdateExtraVariable method
                    }

                    //If it's CrazyCap, check his stock before continuing
                    //If out of stock, change override start node
                    if (VD.assigned.alias == "CrazyCap") {
                        var evItem2: int = VD.GetInt("item");
                        if (evItem2 + 1 >= example_Items.Count)
                            VD.assigned.overrideStartNode = 28;

                    }


                    if (!example_ItemInventory.Contains(example_Items[VD.GetInt("item")])) {
                        GiveItem(VD.GetInt("item"));
                        return true;
                    }
                }
        }

        //Replaces [NAME]
        if (data.extraVars.ContainsKey("nameLookUp"))
            nameLookUp(data);

        //Checks for extraData that concerns font size (CrazyCap node 2)
        if (data.extraData[data.commentIndex].Contains("fs")) {
            var fontSize: String[] = data.extraData[data.commentIndex].Split("," [0]);
            var fSize: int = 14;
            int.TryParse(fontSize[1], fSize);
            npcText.fontSize = fSize;
        } else {
            npcText.fontSize = 14;
        }

    }
    return false;
}

//Adds item to demo inventory, shows item popup, and pauses dialogue
private function GiveItem(itemIndex: int) {
    example_ItemInventory.Add(example_Items[itemIndex]);
    itemPopUp.SetActive(true);
    var text: String = "You've got a <color=yellow>" + example_Items[itemIndex] + "</color>!";
    itemPopUp.transform.GetChild(0).GetComponent. < Text > ().text = text;
    dialoguePaused = true;
}

//This uses the returned String[] from nodeData.comments to create the UIs for each comment
//It first cleans, then it instantiates new options
//This is for demo only, you shouldn´t instantiate/destroy so constantly public
private function SetOptions(opts: String[]) {
    //Destroy the current options
    for (var op: UnityEngine.UI.Text in currentOptions)
        Destroy(op.gameObject);

    //Clean the variable
    currentOptions = new List. < UnityEngine.UI.Text > ();

    //Create the options
    for (var i: int = 0; i < opts.Length; i++) {
        var newOp: GameObject = Instantiate(playerText.gameObject, playerText.transform.position, Quaternion.identity) as GameObject;
        newOp.SetActive(true);
        newOp.transform.SetParent(playerText.transform.parent, true);
        newOp.GetComponent. < RectTransform > ().anchoredPosition = new Vector2(0, 20 - (20 * i));
        newOp.GetComponent. < UnityEngine.UI.Text > ().text = opts[i];
        currentOptions.Add(newOp.GetComponent. < UnityEngine.UI.Text > ());
    }
}

//This will replace any "[NAME]" with the name of the gameobject holding the VIDE_Assign
//Will also replace [WEAPON] with a different variable
private function nameLookUp(data: VD.NodeData) {
    if (data.comments[data.commentIndex].Contains("[NAME]"))
        data.comments[data.commentIndex] = data.comments[data.commentIndex].Replace("[NAME]", VD.assigned.gameObject.name);

    if (data.comments[data.commentIndex].Contains("[WEAPON]"))
        data.comments[data.commentIndex] = data.comments[data.commentIndex].Replace("[WEAPON]", example_ItemInventory[0].ToLower());
}

//Very simple text animation usin StringBuilder
public function AnimateText(data: VD.NodeData): IEnumerator {
    animatingText = true;
    var text: String = data.comments[data.commentIndex];

    if (!data.isPlayer) {
        var builder: StringBuilder = new StringBuilder();
        var charIndex: int = 0;
        while (npcText.text != text) {
            builder.Append(text[charIndex]);
            charIndex++;
            npcText.text = builder.ToString();
            yield WaitForSeconds(0.02);
        }
    }

    npcText.text = data.comments[data.commentIndex]; //Now just copy full text       
    animatingText = false;
}

private function CutTextAnim() {
    StopCoroutine("AnimateText");
    npcText.text = VD.nodeData.comments[VD.nodeData.commentIndex]; //Now just copy full text     
    animatingText = false;
}

//Unsuscribe from everything, disable UI, and end dialogue
private function EndDialogue(data: VD.NodeData) {
    VD.OnActionNode -= ActionHandler;
    VD.OnNodeChange -= NodeChangeAction;
    VD.OnEnd -= EndDialogue;
    uiContainer.SetActive(false);
    VD.EndDialogue();
}

//Example method called by an Action Node
public function ActionGiveItem(item: int) {
    //Do something here
}

//This method is not necessary when using the Full version of the plugin
//as Action Nodes have predefined actions that allow you to easily modify the start point in the VIDE Editor.
public function SetOverrideStartNode(newNode: int) {
    if (VD.isActive) {
        VD.assigned.overrideStartNode = newNode;
    }
}

//This method is not necessary when using the Full version of the plugin
//as Action Nodes have predefined actions that allow you to easily modify go to another node
public function GoToNode(newNode: int) {
    if (VD.isActive) {
        VD.SetNode(newNode);
    }
}

Delete this line and the one at the top */