using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON_VIDE;

public class VIDE_Assign : MonoBehaviour
{
    /*
     * This script component should be attached to every game object you will be interacting with.
     * When interacting with the NPC or object, you will have to call the BeginDialogue() method on
     * the DialogueData component and pass this script.
     * It will safely load the assigned script and keep track of the amount of times you've interacted with it.
     * It will also allow you to set a start point override and to modify the assigned dialogue.
     */

    public List<string> diags = new List<string>();
    public int assignedIndex = 0;
    public int assignedID = 0;
    public string assignedDialogue = "";

    public int interactionCount = 0;
    public string alias = "";

    public int overrideStartNode = -1;

    public Sprite defaultNPCSprite;
    public Sprite defaultPlayerSprite;

    /// <summary>
    /// Returns the name of the currently assigned dialogue.
    /// </summary>
    /// <returns></returns>
    public string GetAssigned()
    {
        return diags[assignedIndex];
    }

    /// <summary>
    /// Assigns a new dialogue to these component.
    /// </summary>
    /// <param name="Dialogue name"></param>
    /// <returns></returns>
    public bool AssignNew(string newFile)
    {
        loadFiles();

        if (!diags.Contains(newFile))
        {
            Debug.LogError("Dialogue not found! Make sure the name is correct and has no extension");
            return false;
        }

        assignedIndex = diags.IndexOf(newFile);
        assignedDialogue = diags[assignedIndex];

        return true;
    }

    private void loadFiles()
    {
        TextAsset[] files = Resources.LoadAll<TextAsset>("Dialogues");
        diags = new List<string>();
        assignedIndex = 0;

        if (files.Length < 1) return;

        foreach (TextAsset f in files)
        {
            diags.Add(f.name);
        }

        diags.Sort();

    }



}

