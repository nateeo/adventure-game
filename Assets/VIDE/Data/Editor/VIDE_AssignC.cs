using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using MiniJSON_VIDE;

[CanEditMultipleObjects]
[CustomEditor(typeof(VIDE_Assign))]
public class VIDE_AssignC : Editor
{
    /*
     * Custom Inspector for the VIDE_Assign component
     */
    VIDE_Assign d;
    bool loadup = false;

    static string path = "";
    List<string> fullPaths = new List<string>();

    private void openVIDE_Editor(string idx)
    {
        if (d != null)
            loadFiles();

        VIDE_Editor editor = EditorWindow.GetWindow<VIDE_Editor>();
        editor.Init(idx, true);
    }

    void OnEnable()
    {
        loadup = true;

        path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
        path = Directory.GetParent(path).ToString();
        path = Directory.GetParent(path).ToString();
        path = Directory.GetParent(path).ToString();

        loadFiles();
    }

    bool HasUniqueID(int id, string[] saveNames, int currentDiag)
    {
        //Retrieve all IDs
        foreach (string s in saveNames)
        {
            if (s == saveNames[currentDiag]) continue;

            if (File.Exists(Application.dataPath + "/../" + s))
            {
                Dictionary<string, object> dict = SerializeHelper.ReadFromFile(s) as Dictionary<string, object>;
                if (dict.ContainsKey("dID"))
                    if (id == ((int)((long)dict["dID"])))
                        return false;
            }
        }
        return true;
    }

    int AssignDialogueID(string[] saveNames)
    {
        List<int> ids = new List<int>();
        int newID = Random.Range(0, 99999);

        //Retrieve all IDs
        foreach (string s in saveNames)
        {
            if (File.Exists(Application.dataPath + "/../" + s))
            {
                Dictionary<string, object> dict = SerializeHelper.ReadFromFile(s) as Dictionary<string, object>;
                if (dict.ContainsKey("dID"))
                    ids.Add((int)((long)dict["dID"]));
            }
        }

        //Make sure ID is unique
        while (ids.Contains(newID))
        {
            newID = Random.Range(0, 99999);
        }

        return newID;
    }

    public class SerializeHelper
    {
        static string fileDataPath = Application.dataPath + "/../";
        public static void WriteToFile(object data, string filename)
        {
            string outString = DiagJson.Serialize(data);
            File.WriteAllText(fileDataPath + filename, outString);
        }
        public static object ReadFromFile(string filename)
        {
            string jsonString = File.ReadAllText(fileDataPath + filename);
            return DiagJson.Deserialize(jsonString);
        }
    }

    public override void OnInspectorGUI()
    {

        d = (VIDE_Assign)target;
        Color defColor = GUI.color;
        GUI.color = Color.yellow;

        if (loadup)
        {
            loadFiles();
            loadup = false;
        }

        //Create a button to open up the VIDE Editor and load the currently assigned dialogue
        if (GUILayout.Button("Open VIDE Editor"))
        {
            openVIDE_Editor(d.assignedDialogue);
        }

        GUI.color = defColor;

        //Refresh dialogue list
        if (Event.current.type == EventType.MouseDown)
        {
            if (d != null)
                loadFiles();
        }

        GUILayout.BeginHorizontal();

        GUILayout.Label("Assigned dialogue:");
        if (d.diags.Count > 0)
        {
            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(d, "Changed dialogue index");
            d.assignedIndex = EditorGUILayout.Popup(d.assignedIndex, d.diags.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                int theID = 0;
                int currentName = -1;

                /* Get file location based on name */
                for (int i = 0; i < d.diags.Count; i++)
                {
                    if (fullPaths[i].Contains(d.diags[d.assignedIndex] + ".json"))
                        currentName = i;
                }

                if (currentName == -1)
                {
                    return;
                }

                if (File.Exists(Application.dataPath + "/../" + fullPaths[currentName]))
                {
                    Dictionary<string, object> dict = SerializeHelper.ReadFromFile(fullPaths[currentName]) as Dictionary<string, object>;
                    if (dict.ContainsKey("dID"))
                    {
                        theID = ((int)((long)dict["dID"]));

                    }
                    else Debug.LogError("Could not read dialogue ID!");
                }

                if (!HasUniqueID(theID, fullPaths.ToArray(), currentName))
                {
                    theID = AssignDialogueID(fullPaths.ToArray());
                    Dictionary<string, object> dict = SerializeHelper.ReadFromFile(fullPaths[currentName]) as Dictionary<string, object>;
                    if (dict.ContainsKey("dID"))
                    {
                        dict["dID"] = theID;
                    }
                    SerializeHelper.WriteToFile(dict as Dictionary<string, object>, fullPaths[currentName]);
                }

                d.assignedID = theID;
                d.assignedDialogue = d.diags[d.assignedIndex];


                foreach (var transform in Selection.transforms)
                {
                    VIDE_Assign scr = transform.GetComponent<VIDE_Assign>();
                    scr.assignedIndex = d.assignedIndex;
                    scr.assignedDialogue = d.assignedDialogue;
                    scr.assignedID = d.assignedID;
                }

            }
        }
        else
        {
            GUILayout.Label("No saved Dialogues!");

        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Alias: ");

        Undo.RecordObject(d, "Changed custom name");
        EditorGUI.BeginChangeCheck();
        d.alias = EditorGUILayout.TextField(d.alias);
        if (EditorGUI.EndChangeCheck())
        {
            foreach (var transform in Selection.transforms)
            {
                VIDE_Assign scr = transform.GetComponent<VIDE_Assign>();
                scr.alias = d.alias;
            }
        }

        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("Override Start Node: ");
        Undo.RecordObject(d, "Changed override start node");
        EditorGUI.BeginChangeCheck();
        d.overrideStartNode = EditorGUILayout.IntField(d.overrideStartNode);
        if (EditorGUI.EndChangeCheck())
        {
            foreach (var transform in Selection.transforms)
            {
                VIDE_Assign scr = transform.GetComponent<VIDE_Assign>();
                scr.overrideStartNode = d.overrideStartNode;
            }
        }
        GUILayout.EndHorizontal();

        EditorGUI.BeginChangeCheck();
        d.defaultPlayerSprite = (Sprite)EditorGUILayout.ObjectField("Def. Player Sprite: ", d.defaultPlayerSprite, typeof(Sprite), false);
        if (EditorGUI.EndChangeCheck())
        {
            foreach (var transform in Selection.transforms)
            {
                VIDE_Assign scr = transform.GetComponent<VIDE_Assign>();
                scr.defaultPlayerSprite = d.defaultPlayerSprite;
            }
        }

        EditorGUI.BeginChangeCheck();
        d.defaultNPCSprite = (Sprite)EditorGUILayout.ObjectField("Def. NPC Sprite: ", d.defaultNPCSprite, typeof(Sprite), false);
        if (EditorGUI.EndChangeCheck())
        {
            foreach (var transform in Selection.transforms)
            {
                VIDE_Assign scr = transform.GetComponent<VIDE_Assign>();
                scr.defaultNPCSprite = d.defaultNPCSprite;
            }
        }
        GUILayout.Label("Interaction Count: " + d.interactionCount.ToString());
        /*GUILayout.Label(d.assignedID.ToString());
        GUILayout.Label(d.assignedIndex.ToString());
        GUILayout.Label(d.assignedDialogue.ToString());*/
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    //Refresh dialogue list
    public void OnFocus()
    {
        if (d != null)
            loadFiles();
    }

    //Refresh dialogue list
    public void loadFiles()
    {
        AssetDatabase.Refresh();
        d = (VIDE_Assign)target;

        TextAsset[] files = Resources.LoadAll<TextAsset>("Dialogues");
        d.diags = new List<string>();
        fullPaths = new List<string>();

        if (files.Length < 1) return;

        foreach (TextAsset f in files)
        {
            d.diags.Add(f.name);
            fullPaths.Add(AssetDatabase.GetAssetPath(f));
        }

        d.diags.Sort();

        //Lets make sure we still have the right file
        IDCheck();
        Repaint();

    }

    void IDCheck()
    {
        int theID = 0;
        List<int> theIDs = new List<int>();
        if (d.assignedIndex == -1)
        {
            if (d.assignedDialogue != "")
            {
                d.assignedIndex = d.diags.IndexOf(d.assignedDialogue);
            }
            else
            {
                return;
            }
        }

        if (!d.diags.Contains(d.assignedDialogue))
        {
            Debug.LogError("'" + d.assignedDialogue + "' dialogue not found!");
            return;
        }


        if (d.assignedIndex >= d.diags.Count)
        {
            for (int i = 0; i < d.diags.Count; i++)
            {
                if (d.diags[i] == d.assignedDialogue)
                    d.assignedIndex = i;
            }
        }

        int currentName = -1;

        /* Get file location based on name */
        for (int i = 0; i < d.diags.Count; i++)
        {
            if (fullPaths[i].Contains(d.diags[d.assignedIndex] + ".json"))
                currentName = i;
        }

        if (currentName == -1)
        {
            return;
        }

        if (File.Exists(Application.dataPath + "/../" + fullPaths[currentName]))
        {
            Dictionary<string, object> dict = SerializeHelper.ReadFromFile(fullPaths[currentName]) as Dictionary<string, object>;
            if (dict.ContainsKey("dID"))
            {
                theID = ((int)((long)dict["dID"]));
            }
            else { Debug.LogError("Could not read dialogue ID!"); return; }
        }

        if (theID != d.assignedID)
        {

            foreach (string s in d.diags)
            {
                for (int i = 0; i < d.diags.Count; i++)
                {
                    if (fullPaths[i].Contains(d.diags[d.diags.IndexOf(s)] + ".json"))
                        currentName = i;
                }

                if (File.Exists(Application.dataPath + "/../" + fullPaths[currentName]))
                {
                    Dictionary<string, object> dict = SerializeHelper.ReadFromFile(fullPaths[currentName]) as Dictionary<string, object>;
                    if (dict.ContainsKey("dID"))
                        theIDs.Add((int)((long)dict["dID"]));
                }
            }
            var theRealID_Index = theIDs.IndexOf(d.assignedID);

            d.assignedIndex = theRealID_Index;

            if (d.assignedIndex != -1)
                d.assignedDialogue = d.diags[d.assignedIndex];
        }
    }

}
