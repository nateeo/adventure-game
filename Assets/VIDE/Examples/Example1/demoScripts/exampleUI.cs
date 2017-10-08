using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using VIDE_Data; //<--- Import to use easily call VD class

public class exampleUI : MonoBehaviour
{
    //THIS IS ONLY A DEMO SCRIPT WITH A DEMO DESIGN THAT WORKS WELL FOR THIS CUSTOM PROJECT
    //Yours might require a different design, and you are absolutely free to create one.
    //Just make sure you read up on available methods, events, and variables in the Scripting API.

    /*
     *  This is script is only meant to be demonstrate various ways of handling data to create a Dialogue/UI Manager
     *  VIDE doesn't focus on the actual interface, but rather on the system and the data
     *  This script is basically handling the node data from nodeData in its own, customized way
     *  Creating a customized in-game Dialogue/UI manager is up to you
     *  Of course, you can absolutely use this script as a start point by adding, modifying, optimizing, or simplifying it to your needs.
     *  If you are experiencing strange behaviours or have any issues or questions, don't hesitate on contacting me at https://videdialogues.wordpress.com/contact/
     */

    //This script will handle everything related to dialogue interface
    //It will use a VD class to load dialogues and retrieve node data

    /*---------------------------------------------------------------------------------------------------------------*/


    //These are just references to UI components and objects in the scene
    public Text npcText;
    public Text npcName;
    public Text playerText;
    public GameObject itemPopUp;
    public GameObject uiContainer;
    public GameObject notIncluded;

    //We'll use these later
    bool dialoguePaused = false;
    bool animatingText = false;

    //Player/NPC Image component
    public Image NPCSprite;
    public Image PlayerSprite;

    //Demo variables
    public List<string> example_Items = new List<string>();
    public List<string> example_ItemInventory = new List<string>();

    //We'll be using this to store the current player dialogue options
    private List<Text> currentOptions = new List<Text>();

    IEnumerator npcTextAnimator;

    void Start()
    {
       notIncluded.SetActive(true);

       VD.LoadDialogues(); //Load all dialogues to memory so that we dont spend time doing so later
    }

    //Just so we know when we finished loading all dialogues, then we unsubscribe
    void OnLoadedAction()
    {
        Debug.Log("Finished loading all dialogues");
        VD.OnLoaded -= OnLoadedAction;
    }

    void OnDisable()
    {
        //If the script gets destroyed, let's make sure we force-end the dialogue to prevent errors
        EndDialogue(null);
    }

    //This begins the conversation (Called by examplePlayer script)
    public void Begin(VIDE_Assign diagToLoad)
    {
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
    void SpecialStartNodeOverrides(VIDE_Assign diagToLoad)
    {
        //Get the item from CrazyCap to trigger this one on Charlie
        if (diagToLoad.alias == "Charlie")
        {
            if (example_ItemInventory.Count > 0 && diagToLoad.overrideStartNode == -1)
                diagToLoad.overrideStartNode = 16;
        }
    }

    //Input related stuff (scroll through player choices and update highlight)
    void Update()
    {
        //Lets just store the Node Data variable for the sake of fewer words
        var data = VD.nodeData;

        if (VD.isActive) //Only if
        {
            //Scroll through Player dialogue options
            if (!data.pausedAction && data.isPlayer)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    if (data.commentIndex < currentOptions.Count - 1)
                        data.commentIndex++;
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    if (data.commentIndex > 0)
                        data.commentIndex--;
                }

                //Color the Player options. Blue for the selected one
                for (int i = 0; i < currentOptions.Count; i++)
                {
                    currentOptions[i].color = Color.white;
                    if (i == data.commentIndex) currentOptions[i].color = Color.yellow;
                }
            }
        }

        if (Input.anyKeyDown && notIncluded.activeSelf)
            notIncluded.SetActive(false);
    }

    //examplePlayer.cs calls this one to move forward in the conversation
    public void CallNext()
    {
        //Let's not go forward if text is currently being animated, but let's speed it up.
        if (animatingText) { CutTextAnim(); ; return; }

        if (!dialoguePaused) //Only if
        {
            //Sometimes, we might want to check the ExtraVariables before moving forward
            //We might want to modify the dialogue or perhaps go to another node
            //If so, we will return and will not call Next()
            if (PreCheckExtraVariables()) return;

            VD.Next(); //We call the next node and populate nodeData with new data
            return;
        }

        //This will just disable the item popup if it is enabled
        if (itemPopUp.activeSelf)
        {
            dialoguePaused = false;
            itemPopUp.SetActive(false);
        }
    }       

    //Another way to handle Action Nodes is to listen to the OnActionNode event, which sends the ID of the action node
    void ActionHandler(int actionNodeID)
    {
        Debug.Log("ACTION TRIGGERED: " + actionNodeID.ToString());
    }

    //We listen to OnNodeChange to update our UI with each new nodeData
    //This should happen right after calling VD.Next()
    void NodeChangeAction(VD.NodeData data)
    {
        //Reset some variables
        npcText.text = "";
        npcText.transform.parent.gameObject.SetActive(false);
        playerText.transform.parent.gameObject.SetActive(false);
        PlayerSprite.sprite = null;
        NPCSprite.sprite = null;

        //Look for dynamic text change in extraData
        PostCheckExtraVariables(data);

        //If this new Node is a Player Node, set the player choices offered by the node
        if (data.isPlayer)
        {
            //Set node sprite if there's any, otherwise try to use default sprite
            if (data.sprite != null)
                PlayerSprite.sprite = data.sprite;
            else if (VD.assigned.defaultPlayerSprite != null)
                PlayerSprite.sprite = VD.assigned.defaultPlayerSprite;

            SetOptions(data.comments);
            playerText.transform.parent.gameObject.SetActive(true);

        }
        else  //If it's an NPC Node, let's just update NPC's text and sprite
        {
            //Set node sprite if there's any, otherwise try to use default sprite
            if (data.sprite != null)
            {
                //For NPC sprite, we'll first check if there's any "sprite" key
                //Such key is being used to apply the sprite only when at a certain comment index
                //Check CrazyCap dialogue for reference
                if (data.extraVars.ContainsKey("sprite"))
                {
                    if (data.commentIndex == (int) data.extraVars["sprite"])
                        NPCSprite.sprite = data.sprite;
                    else
                        NPCSprite.sprite = VD.assigned.defaultNPCSprite; //If not there yet, set default dialogue sprite
                }
                else
                {
                    NPCSprite.sprite = data.sprite;
                }
            }
            else if (VD.assigned.defaultNPCSprite != null)
                NPCSprite.sprite = VD.assigned.defaultNPCSprite;

            npcTextAnimator = AnimateText(data);
            StartCoroutine(npcTextAnimator);

            //If it has a tag, show it, otherwise show the dialogueName
            if (data.tag.Length > 0)
                npcName.text = data.tag;
            else
                npcName.text = VD.assigned.alias;

            npcText.transform.parent.gameObject.SetActive(true);
        }
    }


    //When this returns true, it means that we did something that alters the progression of the dialogue
    //And we don't want to call Next() this time
    bool PreCheckExtraVariables()
    {
        var data = VD.nodeData;
        //Check for extra variables
        //This one finds a key named "item" which has the value of the item thats gonna be given
        //If there's an 'item' key, then we will assume there's also an 'itemLine' key and use it
        if (!data.isPlayer)
        {
            if (data.extraVars.ContainsKey("item") && !data.dirty)
            {
                if (data.commentIndex == (int)data.extraVars["itemLine"])
                {
                    if (data.extraVars.ContainsKey("item++")) //If we have this key, we use it to increment the value of 'item' by 'item++'
                    {
                        Dictionary<string, object> newVars = data.extraVars; //Clone the current extraVars content
                        int newItem = (int)newVars["item"]; //Retrieve the value we want to change
                        newItem += (int)data.extraVars["item++"]; //Change it as we desire
                        newVars["item"] = newItem; //Set it back   
                        VD.SetExtraVariables(25, newVars); //Send newVars through UpdateExtraVariable method
                    }

                    //If it's CrazyCap, check his stock before continuing
                    //If out of stock, change override start node
                    if (VD.assigned.alias == "CrazyCap")
                        if ((int)data.extraVars["item"] + 1 >= example_Items.Count)
                            VD.assigned.overrideStartNode = 28;


                    if (!example_ItemInventory.Contains(example_Items[(int)data.extraVars["item"]]))
                    {
                        GiveItem((int)data.extraVars["item"]);
                        return true;
                    }
                }
            }
        } else
        {
            if (data.extraVars.ContainsKey("outCondition"))
            {
                if (data.extraVars.ContainsKey("condInfo"))
                {
                    int[] nodeIDs = VD.ToIntArray((string)data.extraVars["outCondition"]);
                    if (VD.assigned.interactionCount < nodeIDs.Length)
                        VD.SetNode(nodeIDs[VD.assigned.interactionCount]);
                    else
                        VD.SetNode(nodeIDs[nodeIDs.Length - 1]);
                    return true;
                }
            }
        }
        return false;
    }

    //Check to see if there's Extra Variables and if so, we do stuff
    void PostCheckExtraVariables(VD.NodeData data)
    {
        //Don't conduct extra variable actions if we are waiting on a paused action
        if (data.pausedAction) return;

        if (!data.isPlayer) //For player nodes
        {
            //Replaces [NAME]
            if (data.extraVars.ContainsKey("nameLookUp"))
                nameLookUp(data);

            //Checks for extraData that concerns font size (CrazyCap node 2)
            if (data.extraData[data.commentIndex].Contains("fs"))
            {
                string[] fontSize = data.extraData[data.commentIndex].Split(","[0]);
                int fSize = 14;
                int.TryParse(fontSize[1], out fSize);
                npcText.fontSize = fSize;
            }
            else
            {
                npcText.fontSize = 14;
            }
        }
        return;
    }

    //Adds item to demo inventory, shows item popup, and pauses dialogue
    void GiveItem(int itemIndex)
    {
        example_ItemInventory.Add(example_Items[itemIndex]);
        itemPopUp.SetActive(true);
        string text = "You've got a <color=yellow>" + example_Items[itemIndex] + "</color>!";
        itemPopUp.transform.GetChild(0).GetComponent<Text>().text = text;
        dialoguePaused = true;
    }

    //This uses the returned string[] from nodeData.comments to create the UIs for each comment
    //It first cleans, then it instantiates new options
    //This is for demo only, you shouldn´t instantiate/destroy so constantly
    public void SetOptions(string[] opts)
    {
        //Destroy the current options
        foreach (UnityEngine.UI.Text op in currentOptions)
            Destroy(op.gameObject);

        //Clean the variable
        currentOptions = new List<UnityEngine.UI.Text>();

        //Create the options
        for (int i = 0; i < opts.Length; i++)
        {
            GameObject newOp = Instantiate(playerText.gameObject, playerText.transform.position, Quaternion.identity) as GameObject;
            newOp.SetActive(true);
            newOp.transform.SetParent(playerText.transform.parent, true);
            newOp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20 - (20 * i));
            newOp.GetComponent<UnityEngine.UI.Text>().text = opts[i];
            currentOptions.Add(newOp.GetComponent<UnityEngine.UI.Text>());
        }
    }

    //This will replace any "[NAME]" with the name of the gameobject holding the VIDE_Assign
    //Will also replace [WEAPON] with a different variable
    void nameLookUp(VD.NodeData data)
    {
        if (data.comments[data.commentIndex].Contains("[NAME]"))
        data.comments[data.commentIndex] = data.comments[data.commentIndex].Replace("[NAME]", VD.assigned.gameObject.name);

        if (data.comments[data.commentIndex].Contains("[WEAPON]"))
        data.comments[data.commentIndex] = data.comments[data.commentIndex].Replace("[WEAPON]", example_ItemInventory[0].ToLower());
    }

    //Very simple text animation usin StringBuilder
    public IEnumerator AnimateText(VD.NodeData data)
    {
        animatingText = true;
        string text = data.comments[data.commentIndex];

        if (!data.isPlayer)
        {
            StringBuilder builder = new StringBuilder();
            int charIndex = 0;
            while (npcText.text != text)
            {
                builder.Append(text[charIndex]);
                charIndex++;
                npcText.text = builder.ToString();
                yield return new WaitForSeconds(0.02f);
            }
        }

        npcText.text = data.comments[data.commentIndex]; //Now just copy full text		
        animatingText = false;
    }

    void CutTextAnim()
    {
        StopCoroutine(npcTextAnimator);
        npcText.text = VD.nodeData.comments[VD.nodeData.commentIndex]; //Now just copy full text		
        animatingText = false;
    }

    //Unsuscribe from everything, disable UI, and end dialogue
    void EndDialogue(VD.NodeData data)
    {
        CheckTasks();
        VD.OnActionNode -= ActionHandler;
        VD.OnNodeChange -= NodeChangeAction;
        VD.OnEnd -= EndDialogue;
        uiContainer.SetActive(false);
        VD.EndDialogue();
    }

    void CheckTasks()
    {
        if (example_ItemInventory.Count == 5)
            QuestChartDemo.SetQuest(2, false);

        QuestChartDemo.CheckTaskCompletion(VD.nodeData);
    }

    //Example method called by an Action Node
    public void ActionGiveItem(int item)
    {
        //Do something here
    }

    //This method is not necessary when using the Full version of the plugin
    //as Action Nodes have predefined actions that allow you to easily modify the start point in the VIDE Editor.
    public void SetOverrideStartNode(int newNode)
    {
        if (VD.isActive)
        {
            VD.assigned.overrideStartNode = newNode;
        }
    }

    //This method is not necessary when using the Full version of the plugin
    //as Action Nodes have predefined actions that allow you to easily modify go to another node
    public void GoToNode(int newNode)
    {
        if (VD.isActive)
        {
            VD.SetNode(newNode);
        }
    }
}
