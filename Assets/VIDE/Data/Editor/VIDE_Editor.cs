using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using MiniJSON_VIDE;
using System.Reflection;
using bf = System.Reflection.BindingFlags;
using System.Text.RegularExpressions;
using System.Linq;

public class VIDE_Editor : EditorWindow
{

    //This script will draw the VIDE Editor window and all of its content
    //It comunicates with VIDE_EditorDB to store the data

    //Blacklist for namespaces. 
    //For Action Nodes: add here the namespaces of the scripts you don't wish to see fetched in the list.
    //Any namespace CONTAINING any of the below strings will be discarded from the search.
    public string[] namespaceBlackList = new string[]{
        "UnityEngine",
        //TMP       
    };

    VIDE_EditorDB db;
    GameObject dbObj;
    Color defaultColor;
    Color32[] colors;

    VIDE_EditorDB.Comment draggedCom;
    VIDE_EditorDB.ActionNode draggedAction;

    Rect canvas = new Rect(-4000, -4000, 20000, 20000);
    bool lerpFocusTime = false;
    Vector2 goalScrollPos;
    Vector2 scrollArea;
    Vector2 dragStart;
    Vector2 assignScroll;
    int gotofocus = 0;
    int gridSize = 10;
    Rect fWin = new Rect();
    Rect startDiag;

    bool showSettings;
    float focusTimer;

    string newsHeadline;
    string newsHeadlineLink;

    bool searchingForDialogue = false;
    string searchWord;
    Vector2 searchScrollView;

    WWW news;
    List<string> saveNames = new List<string>() { };
    List<string> saveNamesFull = new List<string>() { };

    int areYouSureIndex = 0;
    Texture2D lineIcon;
    Texture2D newNodeIcon;
    Texture2D newNodeIcon3;
    Texture2D twitIcon;
    Texture2D visON;
    Texture2D visOFF;
    int dragNewNode = 0;
    object copiedNode = null;
    Rect dragNewNodeRect = new Rect(20, 20, 100, 40);

    bool draggingLine = false;
    bool dragnWindows = false;
    bool repaintLines = false;
    bool editEnabled = true;
    bool newFile = false;
    bool overwritePopup = false;
    bool deletePopup = false;
    bool needSave = false;
    bool playerReady = false;
    bool areYouSure = false;
    bool showError = false;
    bool hasID = false;
    string newFileName = "My Dialogue";
    string errorMsg = "";
    string lastTextFocus;
    Vector2 dragSlide;


    //Add VIDE Editor to Window...
    [MenuItem("Window/VIDE Editor (Lite)")]
    static void ShowEditor()
    {
        VIDE_Editor editor = EditorWindow.GetWindow<VIDE_Editor>();
        editor.Init("", false);
    }

    void OnEnable()
    {
        VIDE_EditorDB.videRoot = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
        VIDE_EditorDB.videRoot = Directory.GetParent(VIDE_EditorDB.videRoot).ToString();
        VIDE_EditorDB.videRoot = Directory.GetParent(VIDE_EditorDB.videRoot).ToString();
        VIDE_EditorDB.videRoot = Directory.GetParent(VIDE_EditorDB.videRoot).ToString();
        dbObj = (GameObject)AssetDatabase.LoadAssetAtPath(VIDE_EditorDB.videRoot + "/Data/Editor/db.prefab", typeof(GameObject));
        db = dbObj.GetComponent<VIDE_EditorDB>();

        lineIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(VIDE_EditorDB.videRoot + "/Data/lineIcon.png", typeof(Texture2D));
        newNodeIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(VIDE_EditorDB.videRoot + "/Data/newNode.png", typeof(Texture2D));
        newNodeIcon3 = (Texture2D)AssetDatabase.LoadAssetAtPath(VIDE_EditorDB.videRoot + "/Data/newNode2.png", typeof(Texture2D));
        twitIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(VIDE_EditorDB.videRoot + "/Data/twit.jpg", typeof(Texture2D));
        visON = (Texture2D)AssetDatabase.LoadAssetAtPath(VIDE_EditorDB.videRoot + "/Data/visON.png", typeof(Texture2D));
        visOFF = (Texture2D)AssetDatabase.LoadAssetAtPath(VIDE_EditorDB.videRoot + "/Data/visOFF.png", typeof(Texture2D));
        gridTex = (Texture2D)AssetDatabase.LoadAssetAtPath(VIDE_EditorDB.videRoot + "/Data/backTex.jpg", typeof(Texture2D));
        gridTex.SetPixel(0, 0, new Color(0.2f,0.2f,0.2f,1));
        gridTex.Apply();

        Load(true);
    }

    //Save progress if autosave is on
    void OnLostFocus()
    {
        dragnWindows = false;
        Repaint();
        repaintLines = true;

        if (db.autosave)
        {
            Save();
            AssetDatabase.Refresh();
            saveEditorSettings(db.currentDiag);
        }
    }

    //Set all start variables
    public void Init(string dName, bool loadFromIndex)
    {
        VIDE_EditorDB.videRoot = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));

        VIDE_EditorDB.videRoot = Directory.GetParent(VIDE_EditorDB.videRoot).ToString();
        VIDE_EditorDB.videRoot = Directory.GetParent(VIDE_EditorDB.videRoot).ToString();
        VIDE_EditorDB.videRoot = Directory.GetParent(VIDE_EditorDB.videRoot).ToString();

#if UNITY_5_0
        EditorWindow.GetWindow<VIDE_Editor>().title = "VIDE Editor";
#else
        Texture2D icon = (Texture2D)AssetDatabase.LoadAssetAtPath(VIDE_EditorDB.videRoot + "/Data/assignIcon.png", typeof(Texture2D));
        GUIContent titleContent = new GUIContent(" VIDE Editor", icon);
        EditorWindow.GetWindow<VIDE_Editor>().titleContent = titleContent;
#endif


        dbObj = (GameObject)AssetDatabase.LoadAssetAtPath(VIDE_EditorDB.videRoot + "/Data/Editor/db.prefab", typeof(GameObject));
        db = dbObj.GetComponent<VIDE_EditorDB>();
        startDiag = new Rect(20f, 50f, 300f, 50f);

        CheckNews();

        VIDE_Editor editor = EditorWindow.GetWindow<VIDE_Editor>();
        editor.position = new Rect(50f, 50f, 1027f, 768);

        scrollArea = new Vector2(4000, 4000);

        //Update diag list
        TextAsset[] files = Resources.LoadAll<TextAsset>("Dialogues");
        saveNames = new List<string>();
        saveNamesFull = new List<string>();
        foreach (TextAsset f in files)
        {
            saveNames.Add(f.name);
            saveNamesFull.Add(AssetDatabase.GetAssetPath(f));
        }
        saveNames.Sort();

        //Get correct index of sent diag
        int theIndex = 0;
        for (int i = 0; i < saveNames.Count; i++)
        {
            if (saveNames[i] == dName)
                theIndex = i;
        }

        //Listen to localization events

        if (loadFromIndex)
        {
            db.fileIndex = theIndex;
            loadFiles(theIndex);
            saveEditorSettings(db.currentDiag);
            Load(true);
        }
        else
        {
            loadEditorSettings();
            loadFiles(db.currentDiag);
            Load(true);
        }



        CenterAll(false, db.startID, true);
    }

    void CheckNews()
    {
        newsHeadline = "Checking...";
        news = new WWW("http://involutionsaga.com/data/VIDE/news.txt");
    }

    void Update()
    {
        if (news != null)
            if (news.isDone)
            {
                if (news.text.Length > 10)
                {
                    string[] st = news.text.Split(","[0]);
                    newsHeadline = st[0];
                    if (st.Length > 1)
                        newsHeadlineLink = st[1];
                    news = null;

                }
                else
                {
                    newsHeadlineLink = string.Empty;
                    newsHeadline = "Could not connect";
                    news = null;
                }
            }

        if (lerpFocusTime)
        {
            float timer = ((Time.realtimeSinceStartup - focusTimer) / 10);
            scrollArea = Vector2.Lerp(scrollArea, goalScrollPos, timer);
            Repaint();
            if (timer > 0.2f) lerpFocusTime = false;

            if (Vector2.Distance(new Vector2(scrollArea.x, scrollArea.y), new Vector2(goalScrollPos.x, goalScrollPos.y)) < 0.1f) lerpFocusTime = false;
        }
    }

    public class SerializeHelper
    {
        static string fileDataPath = Application.dataPath + "/../";
        static string SettingsDataPath = Application.dataPath + "/../" + VIDE_EditorDB.videRoot + "/Resources/";

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
        public static void WriteSettings(object data, string filename)
        {
            string outString = DiagJson.Serialize(data);
            File.WriteAllText(SettingsDataPath + filename, outString);
        }
        public static object ReadSettings(string filename)
        {
            string jsonString = File.ReadAllText(SettingsDataPath + filename);
            return DiagJson.Deserialize(jsonString);
        }
    }

    #region Main Methods

    public void addComment(VIDE_EditorDB.DialogueNode id)
    {
        Undo.RecordObject(db, "Added Comment");
        id.comment.Add(new VIDE_EditorDB.Comment(id));

    }

    public void addSet(Vector2 rPos, int cSize, int id, string pTag, bool endC)
    {
        db.playerDiags.Add(new VIDE_EditorDB.DialogueNode(rPos, cSize, id, pTag, endC));
    }

    public void removeSet(VIDE_EditorDB.DialogueNode id)
    {
        Undo.RecordObject(db, "Removed Set");


        db.playerDiags.Remove(id);

        for (int i = 0; i < db.playerDiags.Count; i++)
        {
            for (int ii = 0; ii < db.playerDiags[i].comment.Count; ii++)
            {
                if (db.playerDiags[i].comment[ii].outNode == id)
                {
                    db.playerDiags[i].comment[ii].outNode = null;
                }
            }

        }

        for (int i = 0; i < db.actionNodes.Count; i++)
        {
            if (db.actionNodes[i].outPlayer == id)
            {
                db.actionNodes[i].outPlayer = null;
            }
        }
    }

    public void removeComment(VIDE_EditorDB.Comment idx)
    {
        Undo.RecordObject(db, "Removed Comment");

        idx.inputSet.comment.Remove(idx);

    }

    public void removeAction(VIDE_EditorDB.ActionNode id)
    {
        Undo.RecordObject(db, "Added Action");
        db.actionNodes.Remove(id);

        for (int i = 0; i < db.playerDiags.Count; i++)
        {
            for (int ii = 0; ii < db.playerDiags[i].comment.Count; ii++)
            {
                if (db.playerDiags[i].comment[ii].outAction == id)
                {
                    db.playerDiags[i].comment[ii].outAction = null;
                }
            }
        }

        for (int i = 0; i < db.actionNodes.Count; i++)
        {
            if (db.actionNodes[i].outAction == id)
            {
                db.actionNodes[i].outAction = null;
            }
        }
    }

    //This will break the node connections
    public void breakConnection(int type, VIDE_EditorDB.Comment commID, VIDE_EditorDB.ActionNode aID)
    {
        Undo.RecordObject(db, "Broke Connection");

        //Type 0 = VIDE_EditorDB.DialogueNode
        //Type 1 = VIDE_EditorDB.ActionNode

        if (type == 0)
        {
            commID.outNode = null;
            commID.outAction = null;
        }

        if (type == 1)
        {
            aID.outPlayer = null;
            aID.outAction = null;
        }

    }

    //Connect DialogueNode to others
    //Create node if released on empty space
    public void TryConnectToDialogueNode(Vector2 mPos, VIDE_EditorDB.Comment commID)
    {
        if (commID == null) return;

        Undo.RecordObject(db, "Connected Node");

        for (int i = 0; i < db.playerDiags.Count; i++)
        {
            if (db.playerDiags[i].rect.Contains(mPos))
            {
                commID.outNode = db.playerDiags[i];
                Repaint();
                return;
            }
        }
        for (int i = 0; i < db.actionNodes.Count; i++)
        {
            if (db.actionNodes[i].rect.Contains(mPos))
            {
                commID.outAction = db.actionNodes[i];
                Repaint();
                return;
            }
        }

        int id = setUniqueID();
        db.playerDiags.Add(new VIDE_EditorDB.DialogueNode(new Rect(mPos.x - 150, mPos.y - 200, 0, 0), id));
        commID.outNode = db.playerDiags[db.playerDiags.Count - 1];

        repaintLines = true;
        Repaint();
        GUIUtility.hotControl = 0;
    }

    //Connect Action node to Dialogue Node/Action node
    //Create Action node if released on empty space
    public void TryConnectAction(Vector2 mPos, VIDE_EditorDB.ActionNode aID)
    {
        if (aID == null) return;
        Undo.RecordObject(db, "Connected Node");

        for (int i = 0; i < db.playerDiags.Count; i++)
        {
            if (db.playerDiags[i].rect.Contains(mPos))
            {
                aID.outPlayer = db.playerDiags[i];
                Repaint();
                return;
            }
        }
        for (int i = 0; i < db.actionNodes.Count; i++)
        {
            if (db.actionNodes[i].rect.Contains(mPos))
            {
                if (db.actionNodes[i] == aID) { return; }

                aID.outAction = db.actionNodes[i];
                Repaint();
                return;
            }
        }
        int id = setUniqueID();
        db.actionNodes.Add(new VIDE_EditorDB.ActionNode(new Rect(mPos.x - 150, mPos.y - 200, 0, 0), id));
        aID.outAction = db.actionNodes[db.actionNodes.Count - 1];
        repaintLines = true;
        Repaint();
        GUIUtility.hotControl = 0;
    }

    //Sets a unique ID for the node
    public int setUniqueID()
    {
        int tempID = 0;
        while (!searchIDs(tempID))
        {
            tempID++;
        }
        return tempID;
    }

    //Searches for a unique ID
    public bool searchIDs(int id)
    {
        for (int i = 0; i < db.playerDiags.Count; i++)
        {
            if (db.playerDiags[i].ID == id) return false;
        }
        for (int i = 0; i < db.actionNodes.Count; i++)
        {
            if (db.actionNodes[i].ID == id) return false;
        }
        return true;
    }

    bool HasUniqueID(int id)
    {
        //Retrieve all IDs
        foreach (string s in saveNames)
        {
            if (s == saveNames[db.currentDiag]) continue;

            int currentName = -1;

            /* Get file location based on name */
            for (int i = 0; i < saveNames.Count; i++)
            {
                if (saveNamesFull[i].Contains(saveNames[saveNames.IndexOf(s)] + ".json"))
                    currentName = i;
            }

            if (currentName == -1)
            {
                return true;
            }

            if (File.Exists(Application.dataPath + "/../" + saveNamesFull[currentName]))
            {
                Dictionary<string, object> dict = SerializeHelper.ReadFromFile(saveNamesFull[currentName]) as Dictionary<string, object>;
                if (dict.ContainsKey("dID"))
                    if (id == ((int)((long)dict["dID"])))
                        return false;
            }
        }
        return true;
    }

    int AssignDialogueID()
    {
        List<int> ids = new List<int>();
        int newID = Random.Range(0, 99999);

        //Retrieve all IDs
        foreach (string s in saveNames)
        {
            int currentName = -1;

            /* Get file location based on name */
            for (int i = 0; i < saveNames.Count; i++)
            {
                if (saveNamesFull[i].Contains(saveNames[saveNames.IndexOf(s)] + ".json"))
                    currentName = i;
            }

            if (currentName == -1)
            {
                return 0;
            }
            if (File.Exists(Application.dataPath + "/../" + saveNamesFull[currentName]))
            {
                Dictionary<string, object> dict = SerializeHelper.ReadFromFile(saveNamesFull[currentName]) as Dictionary<string, object>;
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

    //Try create a new dialogue file
    public bool tryCreate(string fName)
    {

        if (saveNames.Contains(fName))
        {
            return false;
        }
        else
        {
            Undo.RecordObject(db, "Created Dialogue");
            saveNames.Add(fName);
            saveNamesFull.Add(VIDE_EditorDB.videRoot + "/Resources/Dialogues/" + fName + ".json");

            saveNames.Sort();
            db.currentDiag = saveNames.IndexOf(fName);
            db.startID = 0;
            db.autosave = true;
            return true;
        }
    }

    //Deletes dialogue
    public void DeleteDiag()
    {
        Undo.RecordObject(db, "Deleted Dialogue");
        File.Delete(Application.dataPath + "/../" + VIDE_EditorDB.videRoot + "/Resources/Dialogues/" + saveNames[db.currentDiag] + ".json");
        File.Delete(Application.dataPath + "/../" + VIDE_EditorDB.videRoot + "/Resources/Dialogues/" + saveNames[db.currentDiag] + ".json.meta");
        if (File.Exists(Application.dataPath + "/../" + VIDE_EditorDB.videRoot + "/Resources/Localized/" + "LOC_" + saveNames[db.currentDiag] + ".json"))
        {
            File.Delete(Application.dataPath + "/../" + VIDE_EditorDB.videRoot + "/Resources/Localized/" + "LOC_" + saveNames[db.currentDiag] + ".json");
            File.Delete(Application.dataPath + "/../" + VIDE_EditorDB.videRoot + "/Resources/Localized/" + "LOC_" + saveNames[db.currentDiag] + ".json.meta");
        }
        AssetDatabase.Refresh();
        loadFiles(0);
        Load(true);
    }

    public Rect IDExists(bool resetOnMax)
    {
        int highestID = 0;
        foreach (VIDE_EditorDB.DialogueNode c in db.playerDiags)
        {
            if (c.ID > highestID) { highestID = c.ID; }
        }
        foreach (VIDE_EditorDB.ActionNode c in db.actionNodes)
        {
            if (c.ID > highestID) { highestID = c.ID; }
        }

        for (int i = 0; i < 99999; i++)
        {
            if (db.curFocusID > highestID)
            {
                if (resetOnMax)
                    db.curFocusID = 0;
                else
                    db.curFocusID = highestID;
            }

            foreach (VIDE_EditorDB.DialogueNode c in db.playerDiags)
            {
                if (c.ID == db.curFocusID) { return c.rect; }
            }
            foreach (VIDE_EditorDB.ActionNode c in db.actionNodes)
            {
                if (c.ID == db.curFocusID) { return c.rect; }
            }
            db.curFocusID++;
        }
        return new Rect(0, 0, 0, 0);
    }

    //Centers nodes
    public void CenterAll(bool increment, int specific, bool instantly)
    {
        Rect nodePos = new Rect();
        if (specific > -1)
        {
            db.curFocusID = specific;
            nodePos = IDExists(false);
        }

        if (increment)
        {
            db.curFocusID++;
            nodePos = IDExists(true);
        }

        if (instantly)
        {
            scrollArea = new Vector2(Mathf.Abs(canvas.x) + nodePos.x - (position.width / 2) + nodePos.width / 2, Mathf.Abs(canvas.y) + nodePos.y - (position.height / 2) + nodePos.height / 2 + 50);
            return;
        }

        focusTimer = Time.realtimeSinceStartup;
        goalScrollPos = new Vector2(Mathf.Abs(canvas.x) + nodePos.x - (position.width / 2) + nodePos.width / 2, Mathf.Abs(canvas.y) + nodePos.y - (position.height / 2) + nodePos.height / 2 + 50);
        lerpFocusTime = true;
    }
    #endregion

    #region File Handling

    //This will save the current data base status
    public void Save()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        GUIUtility.keyboardControl = 0;


        if (saveNames.Count < 1)
            return;

        if (db.currentDiag >= saveNames.Count)
        {
            Debug.LogError("Dialogue file not found! Loading default.");
            db.currentDiag = 0;
        }

        int theID = -1;
        int currentName = -1;

        /* Get file location based on name */
        for (int i = 0; i < saveNames.Count; i++)
        {
            if (saveNamesFull[i].Contains(saveNames[db.currentDiag] + ".json"))
                currentName = i;
        }
        if (currentName == -1)
        {
            return;
        }

        if (File.Exists(Application.dataPath + "/../" + saveNamesFull[currentName]))
        {
            Dictionary<string, object> dictl = SerializeHelper.ReadFromFile(saveNamesFull[currentName]) as Dictionary<string, object>;
            if (dictl.ContainsKey("dID"))
            {
                theID = ((int)((long)dictl["dID"]));

            }
        }

        if (theID == -1)
        {
            dict.Add("dID", AssignDialogueID());
        }
        else
        {
            if (!HasUniqueID(theID))
                dict.Add("dID", AssignDialogueID());
            else
                dict.Add("dID", theID);
        }

        dict.Add("playerDiags", db.playerDiags.Count);
        dict.Add("actionNodes", db.actionNodes.Count);
        dict.Add("startPoint", db.startID);
        dict.Add("loadTag", db.loadTag);
        dict.Add("previewPanning", db.previewPanning);
        dict.Add("autosave", db.autosave);

        dict.Add("showSettings", showSettings);

        for (int i = 0; i < db.playerDiags.Count; i++)
        {
            dict.Add("pd_isp_" + i.ToString(), db.playerDiags[i].isPlayer);
            dict.Add("pd_rect_" + i.ToString(), new int[] { (int)db.playerDiags[i].rect.x, (int)db.playerDiags[i].rect.y });
            dict.Add("pd_comSize_" + i.ToString(), db.playerDiags[i].comment.Count);
            dict.Add("pd_ID_" + i.ToString(), db.playerDiags[i].ID);

            dict.Add("pd_pTag_" + i.ToString(), db.playerDiags[i].playerTag);

            if (db.playerDiags[i].sprite != null)
                dict.Add("pd_sprite_" + i.ToString(), AssetDatabase.GetAssetPath(db.playerDiags[i].sprite));
            else
                dict.Add("pd_sprite_" + i.ToString(), string.Empty);


            dict.Add("pd_expand_" + i.ToString(), db.playerDiags[i].expand);
            dict.Add("pd_vars" + i.ToString(), db.playerDiags[i].vars.Count);

            for (int v = 0; v < db.playerDiags[i].vars.Count; v++)
            {
                dict.Add("pd_var_" + i.ToString() + "_" + v.ToString(), db.playerDiags[i].vars[v]);
                dict.Add("pd_varKey_" + i.ToString() + "_" + v.ToString(), db.playerDiags[i].varKeys[v]);
            }

            for (int ii = 0; ii < db.playerDiags[i].comment.Count; ii++)
            {
                dict.Add("pd_" + i.ToString() + "_com_" + ii.ToString() + "iSet", db.playerDiags.FindIndex(idx => idx == db.playerDiags[i].comment[ii].inputSet));
                dict.Add("pd_" + i.ToString() + "_com_" + ii.ToString() + "oAns", db.playerDiags.FindIndex(idx => idx == db.playerDiags[i].comment[ii].outNode));
                dict.Add("pd_" + i.ToString() + "_com_" + ii.ToString() + "oAct", db.actionNodes.FindIndex(idx => idx == db.playerDiags[i].comment[ii].outAction));
                dict.Add("pd_" + i.ToString() + "_com_" + ii.ToString() + "text", db.playerDiags[i].comment[ii].text);
                dict.Add("pd_" + i.ToString() + "_com_" + ii.ToString() + "extraD", db.playerDiags[i].comment[ii].extraData);
                dict.Add("pd_" + i.ToString() + "_com_" + ii.ToString() + "showmore", db.playerDiags[i].comment[ii].showmore);
                dict.Add("pd_" + i.ToString() + "_com_" + ii.ToString() + "visible", db.playerDiags[i].comment[ii].visible);
            }
        }

        for (int i = 0; i < db.actionNodes.Count; i++)
        {
            dict.Add("ac_rect_" + i.ToString(), new int[] { (int)db.actionNodes[i].rect.x, (int)db.actionNodes[i].rect.y });
            dict.Add("ac_ID_" + i.ToString(), db.actionNodes[i].ID);
            dict.Add("ac_pause_" + i.ToString(), db.actionNodes[i].pauseHere);

            dict.Add("ac_goName_" + i.ToString(), db.actionNodes[i].gameObjectName);
            dict.Add("ac_nIndex_" + i.ToString(), db.actionNodes[i].nameIndex);

            dict.Add("ac_optsCount_" + i.ToString(), db.actionNodes[i].opts.Length);
            for (int ii = 0; ii < db.actionNodes[i].opts.Length; ii++)
                dict.Add("ac_opts_" + ii.ToString() + "_" + i.ToString(), db.actionNodes[i].opts[ii]);

            dict.Add("ac_namesCount_" + i.ToString(), db.actionNodes[i].nameOpts.Count);
            for (int ii = 0; ii < db.actionNodes[i].nameOpts.Count; ii++)
                dict.Add("ac_names_" + ii.ToString() + "_" + i.ToString(), db.actionNodes[i].nameOpts[ii]);

            List<string> keyList = new List<string>(db.actionNodes[i].methods.Keys);
            dict.Add("ac_methCount_" + i.ToString(), keyList.Count);

            for (int ii = 0; ii < db.actionNodes[i].methods.Count; ii++)
            {
                dict.Add("ac_meth_key_" + i.ToString() + "_" + ii.ToString(), keyList[ii]);
                dict.Add("ac_meth_val_" + i.ToString() + "_" + ii.ToString(), db.actionNodes[i].methods[keyList[ii]]);
            }

            dict.Add("ac_meth_" + i.ToString(), db.actionNodes[i].methodName);
            dict.Add("ac_paramT_" + i.ToString(), db.actionNodes[i].paramType);
            dict.Add("ac_methIndex_" + i.ToString(), db.actionNodes[i].methodIndex);

            dict.Add("ac_pString_" + i.ToString(), db.actionNodes[i].param_string);
            dict.Add("ac_pBool_" + i.ToString(), db.actionNodes[i].param_bool);
            dict.Add("ac_pInt_" + i.ToString(), db.actionNodes[i].param_int);
            dict.Add("ac_pFloat_" + i.ToString(), db.actionNodes[i].param_float);

            dict.Add("ac_oSet_" + i.ToString(), db.playerDiags.FindIndex(idx => idx == db.actionNodes[i].outPlayer));
            dict.Add("ac_oAct_" + i.ToString(), db.actionNodes.FindIndex(idx => idx == db.actionNodes[i].outAction));


        }

        needSave = false;
        SerializeHelper.WriteToFile(dict as Dictionary<string, object>, saveNamesFull[currentName]);

    }

    public void saveEditorSettings(int cd)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("db.currentDiagEdited", cd);
        SerializeHelper.WriteSettings(dict as Dictionary<string, object>, "EditorSettings" + ".json");
    }

    public void loadEditorSettings()
    {
        if (!File.Exists(Application.dataPath + "/../" + VIDE_EditorDB.videRoot + "/Resources/" + "EditorSettings" + ".json"))
            return;

        Dictionary<string, object> dict = SerializeHelper.ReadSettings("EditorSettings" + ".json") as Dictionary<string, object>;
        if (dict.ContainsKey("db.currentDiagEdited"))
        {
            db.currentDiag = (int)((long)dict["db.currentDiagEdited"]);
            db.fileIndex = db.currentDiag;
        }
        else
        {
            db.currentDiag = 0;
            db.fileIndex = 0;
        }

    }

    //Loads from dialogues
    public void Load(bool clear)
    {
        if (clear)
        {
            db.playerDiags = new List<VIDE_EditorDB.DialogueNode>();
            db.actionNodes = new List<VIDE_EditorDB.ActionNode>();
        }

        if (saveNames.Count < 1)
            return;

        if (db.currentDiag >= saveNames.Count)
        {
            Debug.LogError("Dialogue file not found! Loading default.");
            db.currentDiag = 0;
        }

        if (db.currentDiag < 0) db.currentDiag = 0;
        int currentName = -1;

        /* Get file location based on name */
        for (int i = 0; i < saveNames.Count; i++)
        {
            if (saveNamesFull[i].Contains(saveNames[db.currentDiag] + ".json"))
                currentName = i;
        }

        if (currentName == -1)
        {
            return;
        }

        //if (!File.Exists(Application.dataPath + "/../" + VIDE_EditorDB.videRoot + "/Resources/Dialogues/" + saveNames[db.currentDiag] + ".json"))
        if (!File.Exists(Application.dataPath + "/../" + saveNamesFull[currentName]))
        {
            return;
        }

        Sprite[] sprites = Resources.LoadAll<Sprite>("");
        AudioClip[] audios = Resources.LoadAll<AudioClip>("");
        List<string> spriteNames = new List<string>();
        List<string> audioNames = new List<string>();
        foreach (Sprite t in sprites)
            spriteNames.Add(t.name);
        foreach (AudioClip t in audios)
            audioNames.Add(t.name);

        Dictionary<string, object> dict = SerializeHelper.ReadFromFile(saveNamesFull[currentName]) as Dictionary<string, object>;

        int pDiags = (int)((long)dict["playerDiags"]);
        int nDiags = 0;
        if (dict.ContainsKey("npcDiags"))
            nDiags = (int)((long)dict["npcDiags"]);

        if (dict.ContainsKey("dID"))
            db.currentID = ((int)((long)dict["dID"]));

        int aDiags = 0;
        if (dict.ContainsKey("actionNodes")) aDiags = (int)((long)dict["actionNodes"]);

        db.startID = (int)((long)dict["startPoint"]);
        if (dict.ContainsKey("loadTag"))
            db.loadTag = (string)dict["loadTag"];

        if (dict.ContainsKey("previewPanning"))
            db.previewPanning = (bool)dict["previewPanning"];

        if (dict.ContainsKey("autosave"))
            db.autosave = (bool)dict["autosave"];

        if (dict.ContainsKey("locEdit"))
            db.locEdit = (bool)dict["locEdit"];


        if (dict.ContainsKey("showSettings"))
        {
            showSettings = (bool)dict["showSettings"];
            startDiag.height = 10;
        }

        //Create first...
        for (int i = 0; i < pDiags; i++)
        {
            string tagt = "";

            if (dict.ContainsKey("pd_pTag_" + i.ToString()))
                tagt = (string)dict["pd_pTag_" + i.ToString()];

            string k = "pd_rect_" + i.ToString();
            List<object> rect = (List<object>)(dict[k]);
            addSet(new Vector2((float)((long)rect[0]), (float)((long)rect[1])),
                (int)((long)dict["pd_comSize_" + i.ToString()]),
                (int)((long)dict["pd_ID_" + i.ToString()]),
                tagt,
                false
                );


            if (dict.ContainsKey("pd_isp_" + i.ToString()))
                db.playerDiags[db.playerDiags.Count - 1].isPlayer = (bool)dict["pd_isp_" + i.ToString()];
            else
                db.playerDiags[db.playerDiags.Count - 1].isPlayer = true;

            if (dict.ContainsKey("pd_sprite_" + i.ToString()))
            {
                string name = Path.GetFileNameWithoutExtension((string)dict["pd_sprite_" + i.ToString()]);
                if (spriteNames.Contains(name))
                    db.playerDiags[db.playerDiags.Count - 1].sprite = sprites[spriteNames.IndexOf(name)];
                else if (name != string.Empty)
                    Debug.LogError("'" + name + "' not found in any Resources folder!");
            }

            if (dict.ContainsKey("pd_expand_" + i.ToString()))
                db.playerDiags[db.playerDiags.Count - 1].expand = (bool)dict["pd_expand_" + i.ToString()];

            if (dict.ContainsKey("pd_vars" + i.ToString()))
            {
                for (int v = 0; v < (int)(long)dict["pd_vars" + i.ToString()]; v++)
                {
                    db.playerDiags[db.playerDiags.Count - 1].vars.Add((string)dict["pd_var_" + i.ToString() + "_" + v.ToString()]);
                    db.playerDiags[db.playerDiags.Count - 1].varKeys.Add((string)dict["pd_varKey_" + i.ToString() + "_" + v.ToString()]);
                }
            }

        }

        int npcIndexStart = db.playerDiags.Count;

        for (int i = 0; i < nDiags; i++)
        {
            string k = "nd_rect_" + i.ToString();
            List<object> rect = (List<object>)(dict[k]);

            string tagt = "";

            if (dict.ContainsKey("nd_tag_" + i.ToString()))
                tagt = (string)dict["nd_tag_" + i.ToString()];

            db.playerDiags.Add(new VIDE_EditorDB.DialogueNode());
            var npc = db.playerDiags[db.playerDiags.Count - 1];
            var v2 = new Vector2(((long)rect[0]), (long)(rect[1]));
            npc.rect = new Rect(v2.x, v2.y, 300, 50);
            npc.ID = (int)((long)dict["nd_ID_" + i.ToString()]);
            npc.playerTag = tagt;

            npc.isPlayer = false;

            List<string> texts = new List<string>();

            string text = (string)dict["nd_text_" + i.ToString()];


            if (text.Contains("<br>"))
            {
                string[] splitText = Regex.Split(text, "<br>");
                texts = new List<string>();
                foreach (string s in splitText)
                {
                    texts.Add(s.Trim());
                }
            }
            else
            {
                texts.Add(text);
            }

            foreach (string s in texts)
            {
                npc.comment.Add(new VIDE_EditorDB.Comment());
                npc.comment[npc.comment.Count - 1].text = s;
            }

            if (dict.ContainsKey("nd_sprite_" + i.ToString()))
            {
                string name = Path.GetFileNameWithoutExtension((string)dict["nd_sprite_" + i.ToString()]);

                if (spriteNames.Contains(name))
                    npc.sprite = sprites[spriteNames.IndexOf(name)];
                else if (name != string.Empty)
                    Debug.LogError("'" + name + "' not found in any Resources folder!");
            }

            if (dict.ContainsKey("nd_expand_" + i.ToString()))
                npc.expand = (bool)dict["nd_expand_" + i.ToString()];

            if (dict.ContainsKey("nd_vars" + i.ToString()))
            {
                for (int v = 0; v < (int)(long)dict["nd_vars" + i.ToString()]; v++)
                {
                    npc.vars.Add((string)dict["nd_var_" + i.ToString() + "_" + v.ToString()]);
                    npc.varKeys.Add((string)dict["nd_varKey_" + i.ToString() + "_" + v.ToString()]);
                }
            }
        }

        for (int i = 0; i < aDiags; i++)
        {
            string k = "ac_rect_" + i.ToString();
            List<object> rect = (List<object>)(dict[k]);
            float pFloat;
            var pfl = dict["ac_pFloat_" + i.ToString()];
            if (pfl.GetType() == typeof(System.Double))
                pFloat = System.Convert.ToSingle(pfl);
            else
                pFloat = (float)(long)pfl;


            db.actionNodes.Add(new VIDE_EditorDB.ActionNode(
                new Vector2((float)((long)rect[0]), (float)((long)rect[1])),
                (int)((long)dict["ac_ID_" + i.ToString()]),
                (string)dict["ac_meth_" + i.ToString()],
                (string)dict["ac_goName_" + i.ToString()],
                (bool)dict["ac_pause_" + i.ToString()],
                (bool)dict["ac_pBool_" + i.ToString()],
                (string)dict["ac_pString_" + i.ToString()],
                (int)((long)dict["ac_pInt_" + i.ToString()]),
                pFloat
                ));

            db.actionNodes[db.actionNodes.Count - 1].nameIndex = (int)((long)dict["ac_nIndex_" + i.ToString()]);

            List<string> opts = new List<string>();
            List<string> nameOpts = new List<string>();

            for (int ii = 0; ii < (int)((long)dict["ac_optsCount_" + i.ToString()]); ii++)
                opts.Add((string)dict["ac_opts_" + ii.ToString() + "_" + i.ToString()]);

            for (int ii = 0; ii < (int)((long)dict["ac_namesCount_" + i.ToString()]); ii++)
                nameOpts.Add((string)dict["ac_names_" + ii.ToString() + "_" + i.ToString()]);

            db.actionNodes[db.actionNodes.Count - 1].opts = opts.ToArray();
            db.actionNodes[db.actionNodes.Count - 1].nameOpts = nameOpts;

            int dc = (int)((long)dict["ac_methCount_" + i.ToString()]);

            for (int ii = 0; ii < dc; ii++)
            {
                db.actionNodes[db.actionNodes.Count - 1].methods.Add(
                    (string)dict["ac_meth_key_" + i.ToString() + "_" + ii.ToString()],
                    (string)dict["ac_meth_val_" + i.ToString() + "_" + ii.ToString()]
                    );
            }


        }

        //Connect now...
        for (int i = 0; i < db.playerDiags.Count - nDiags; i++)
        {
            for (int ii = 0; ii < db.playerDiags[i].comment.Count; ii++)
            {
                if (dict.ContainsKey("pd_" + i.ToString() + "_com_" + ii.ToString() + "text"))
                    db.playerDiags[i].comment[ii].text = (string)dict["pd_" + i.ToString() + "_com_" + ii.ToString() + "text"];

                if (dict.ContainsKey("pd_" + i.ToString() + "_com_" + ii.ToString() + "visible"))
                    db.playerDiags[i].comment[ii].visible = (bool)dict["pd_" + i.ToString() + "_com_" + ii.ToString() + "visible"];

                if (dict.ContainsKey("pd_" + i.ToString() + "_com_" + ii.ToString() + "showmore"))
                    db.playerDiags[i].comment[ii].showmore = (bool)dict["pd_" + i.ToString() + "_com_" + ii.ToString() + "showmore"];

                if (dict.ContainsKey("pd_" + i.ToString() + "_com_" + ii.ToString() + "extraD"))
                    db.playerDiags[i].comment[ii].extraData = (string)dict["pd_" + i.ToString() + "_com_" + ii.ToString() + "extraD"];

                int index = (int)((long)dict["pd_" + i.ToString() + "_com_" + ii.ToString() + "iSet"]);

                if (index != -1)
                    db.playerDiags[i].comment[ii].inputSet = db.playerDiags[index];

                index = (int)((long)dict["pd_" + i.ToString() + "_com_" + ii.ToString() + "oAns"]);

                if (index != -1)
                {
                    if (nDiags > 0)
                    {
                        db.playerDiags[i].comment[ii].outNode = db.playerDiags[(db.playerDiags.Count - nDiags) + index];
                    }
                    else
                    {
                        db.playerDiags[i].comment[ii].outNode = db.playerDiags[index];
                    }
                }

                index = -1;
                if (dict.ContainsKey("pd_" + i.ToString() + "_com_" + ii.ToString() + "oAct"))
                    index = (int)((long)dict["pd_" + i.ToString() + "_com_" + ii.ToString() + "oAct"]);

                if (index != -1)
                    db.playerDiags[i].comment[ii].outAction = db.actionNodes[index];
            }
        }
        for (int i = npcIndexStart; i < db.playerDiags.Count; i++)
        {
            int x = i - npcIndexStart;
            int index = -1;
            if (dict.ContainsKey("nd_oSet_" + x.ToString()))
                index = (int)((long)dict["nd_oSet_" + x.ToString()]);

            if (index != -1)
                db.playerDiags[i].comment[0].outNode = db.playerDiags[index];

            if (dict.ContainsKey("nd_oAct_" + x.ToString()))
            {
                index = -1;
                index = (int)((long)dict["nd_oAct_" + x.ToString()]);
                if (index != -1)
                    db.playerDiags[i].comment[0].outAction = db.actionNodes[index];
            }
        }
        for (int i = 0; i < db.actionNodes.Count; i++)
        {
            db.actionNodes[i].paramType = (int)((long)dict["ac_paramT_" + i.ToString()]);
            db.actionNodes[i].methodIndex = (int)((long)dict["ac_methIndex_" + i.ToString()]);

            int index = -1;
            index = (int)((long)dict["ac_oSet_" + i.ToString()]);

            if (index != -1)
                db.actionNodes[i].outPlayer = db.playerDiags[index];

            if (dict.ContainsKey("ac_oAct_" + i.ToString()))
            {
                index = -1;
                index = (int)((long)dict["ac_oAct_" + i.ToString()]);
                if (index != -1)
                    db.actionNodes[i].outAction = db.actionNodes[index];
            }
        }

        repaintLines = true;
        Repaint();
    }

    //Refreshes file list
    public void loadFiles(int focused)
    {
        TextAsset[] files = Resources.LoadAll<TextAsset>("Dialogues");
        saveNames = new List<string>();
        saveNamesFull = new List<string>();
        db.currentDiag = focused;
        foreach (TextAsset f in files)
        {
            saveNames.Add(f.name);
            saveNamesFull.Add(AssetDatabase.GetAssetPath(f));
        }
        saveNames.Sort();
    }

    #endregion

    //TOOLBAR
    void DrawToolbar()
    {

        //Current Dialogue

        GUI.enabled = editEnabled;
        GUIStyle titleSt = new GUIStyle(GUI.skin.GetStyle("Label"));
        titleSt.fontStyle = FontStyle.Bold;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Editing: ", EditorStyles.label, GUILayout.Width(55));
        int t_file = db.fileIndex;
        if (GUILayout.Button(">", EditorStyles.toolbarButton))
        {
            searchingForDialogue = !searchingForDialogue;
            searchWord = "";
            Repaint();
            //return;
        }

        if (Event.current.keyCode == KeyCode.Escape && Event.current.type == EventType.keyUp)
        {
            if (searchingForDialogue)
            {
                searchingForDialogue = false;
                searchWord = "";
                Repaint();
            }
        }

        if (Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.keyUp)
        {
            if (searchingForDialogue && searchWord != "")
            {
                for (int i = 0; i < saveNames.Count; i++)
                {
                    if (saveNames[i].ToLower().Contains(searchWord.ToLower()))
                    {
                        Undo.RecordObject(db, "Loaded dialogue");
                        db.fileIndex = i;
                        if (playerReady)
                        {
                            Save();
                        }
                        searchingForDialogue = false;
                        db.currentDiag = db.fileIndex;
                        saveEditorSettings(db.currentDiag);
                        Load(true);
                        CenterAll(false, db.startID, true);
                        Repaint();
                        break;
                        //return;
                    }
                }
            }
        }

        if (saveNames.Count > 0)
        {
            GUIStyle gs = new GUIStyle(EditorStyles.toolbarPopup);
            gs.fontStyle = FontStyle.Bold;

            if (searchingForDialogue)
            {
                GUI.SetNextControlName("filterSearch");
                searchWord = GUILayout.TextField(searchWord, GUILayout.Width(150));
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                int fileIndexTEMP = EditorGUILayout.Popup(db.fileIndex, saveNames.ToArray(), gs, GUILayout.Width(150));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(db, "Changed Dialogue");
                    db.fileIndex = fileIndexTEMP;

                    if (t_file != db.fileIndex)
                    {
                        if (playerReady)
                        {
                            Save();
                        }
                        db.currentDiag = db.fileIndex;
                        saveEditorSettings(db.currentDiag);
                        Load(true);
                        CenterAll(false, db.startID, true);
                        return;
                    }
                }
            }

        }

        if (searchingForDialogue) GUI.enabled = false;

        GUI.color = new Color(0.8f, 0.9f, 0.95f, 1);

        //Add new
        if (GUILayout.Button("Add new dialogue", EditorStyles.toolbarButton))
        {
            editEnabled = false;
            newFile = true;
            GUI.FocusWindow(99998);
        }

        //Delete
        if (saveNames.Count > 0)
        {
            if (GUILayout.Button("Delete current", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                editEnabled = false;
                deletePopup = true;
            }
            GUIStyle bb = new GUIStyle(GUI.skin.label);
            bb.fontStyle = FontStyle.Bold;
            bb.normal.textColor = Color.red;

            if (showError)
                GUI.enabled = false;

            GUI.color = defaultColor;


            if (needSave) GUI.color = Color.yellow;
            if (GUILayout.Button("SAVE", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                Save();
                AssetDatabase.Refresh();

                needSave = false;
                newFileName = "My Dialogue";
                editEnabled = true;
                overwritePopup = false;
                newFile = false;
                errorMsg = "";
                saveEditorSettings(db.currentDiag);
                //editEnabled = false;
                //overwritePopup = true;
            }
            GUI.color = defaultColor;
            //autosaveON = GUILayout.Toggle(autosaveON, "Autosave");

            GUILayout.Label("Autosave", EditorStyles.miniLabel);
            EditorGUI.BeginChangeCheck();
            bool auto = GUILayout.Toggle(db.autosave, "");
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(db, "Set Autosave");
                db.autosave = auto;
                needSave = true;
            }
            GUI.enabled = true;

            if (!hasID) { GUI.color = Color.red; }
            GUILayout.Label("Start Node ID: ", EditorStyles.miniLabel);

            EditorGUI.BeginChangeCheck();
            int sid = EditorGUILayout.IntField(db.startID, EditorStyles.toolbarTextField, GUILayout.Width(50));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(db, "Set Start ID");
                db.startID = sid;
                needSave = true;
            }

            GUI.color = defaultColor;
            GUILayout.Label("Load Tag: ", EditorStyles.miniLabel);
            EditorGUI.BeginChangeCheck();
            string lt = EditorGUILayout.TextField(db.loadTag, EditorStyles.toolbarTextField, GUILayout.Width(50));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(db, "Set Load Tag");
                db.loadTag = lt;
                needSave = true;

            }
            GUI.color = defaultColor;
        }

        GUILayout.EndHorizontal();
        GUI.enabled = true;

        GUILayout.FlexibleSpace();
        GUIStyle s = new GUIStyle(EditorStyles.toolbarButton);
        //s.normal.textColor = new Color(0.4f,0.9f,0.2f,1);
        s.alignment = TextAnchor.MiddleRight;
        s.fontStyle = FontStyle.Bold;
        if (GUILayout.Button(newsHeadline, s, GUILayout.MinWidth(30)))
        {
            if (newsHeadlineLink.Length > 0)
            {
                Application.OpenURL(newsHeadlineLink);
                EditorGUIUtility.ExitGUI();
            }
        }
        GUI.color = defaultColor;

    
    }

    void DrawToolbar2()
    {
        //Current Dialogue
        if (saveNames.Count > 0)
            GUI.enabled = true;

        GUI.enabled = editEnabled;
        GUI.enabled = !searchingForDialogue;
        GUIStyle titleSt = new GUIStyle(GUI.skin.GetStyle("Label"));
        titleSt.fontStyle = FontStyle.Bold;

        if (saveNames.Count > 0)
        {

            GUILayout.Label("Add nodes: ", EditorStyles.label);
            GUILayout.BeginHorizontal();
            GUI.color = Color.cyan;

            // ADD NEW BUTTONS
            Rect lr;

            if (dragNewNode == 1)
                GUILayout.Box("", EditorStyles.toolbarButton, GUILayout.Width(50), GUILayout.Height(20));
            else
                GUILayout.Box(newNodeIcon, EditorStyles.toolbarButton, GUILayout.Width(50), GUILayout.Height(20));
            lr = GUILayoutUtility.GetLastRect();
            if (editEnabled && lr.Contains(Event.current.mousePosition) && Event.current.type == EventType.mouseDown)
            {
                dragNewNode = 1;
            }

            if (dragNewNode == 2)
                GUILayout.Box("", EditorStyles.toolbarButton, GUILayout.Width(50), GUILayout.Height(20));
            else
                GUILayout.Box(newNodeIcon3, EditorStyles.toolbarButton, GUILayout.Width(50), GUILayout.Height(20));
            lr = GUILayoutUtility.GetLastRect();
            if (editEnabled && lr.Contains(Event.current.mousePosition) && Event.current.type == EventType.mouseDown)
            {
                dragNewNode = 2;
            }

            GUILayout.EndHorizontal();

            GUI.color = defaultColor;

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Focus Next ", EditorStyles.toolbarButton))
            {
                CenterAll(true, -1, false);
                Repaint();
            }
            if (GUILayout.Button("Go to:", EditorStyles.toolbarButton))
            {
                CenterAll(false, gotofocus, false);
                Repaint();
            }
            gotofocus = EditorGUILayout.IntField(gotofocus, GUILayout.Width(30));
            GUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            bool pp = GUILayout.Toggle(db.previewPanning, "Perf. panning");
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(db, "Changed preview panning");
                db.previewPanning = pp;
                needSave = true;

            }

        }
        GUILayout.FlexibleSpace();

        GUI.color = Color.yellow;
        if (GUILayout.Button("Get VIDE PRO!", EditorStyles.toolbarButton, GUILayout.Width(120)))
        {
            Application.OpenURL("https://assetstore.unity.com/packages/tools/ai/vide-dialogues-pro-69932");
            EditorGUIUtility.ExitGUI();
        }
        GUI.color = Color.white;

        GUI.enabled = true; 

        GUILayout.Label("VIDE 1.1.1 (Lite)", EditorStyles.miniLabel);
        if (GUILayout.Button(twitIcon, EditorStyles.toolbarButton, GUILayout.Width(20)))
        {
            Application.OpenURL("https://twitter.com/VIDEDialogues");
            EditorGUIUtility.ExitGUI();
        }

    }

    void SearchDialogue()
    {
        GUILayout.Space(10);
        searchScrollView = GUILayout.BeginScrollView(searchScrollView, GUILayout.Height(position.height - 50));
        for (int i = 0; i < saveNames.Count; i++)
        {
            if (saveNames[i].ToLower().Contains(searchWord.ToLower()))
                if (GUILayout.Button(saveNames[i], EditorStyles.toolbarButton))
                {
                    db.fileIndex = i;
                    if (playerReady)
                    {
                        Save();
                    }
                    searchingForDialogue = false;
                    db.currentDiag = db.fileIndex;
                    saveEditorSettings(db.currentDiag);
                    Load(true);
                    CenterAll(false, db.startID, true);
                    return;
                }
        }
        GUILayout.EndScrollView();


    }

    private static string UppercaseString(string inputString)
    {
        return inputString.ToUpper();
    }

    //Here's where we actually draw everything
    void OnGUI()
    {
        Event e = Event.current;
        //Set colors we'll be using later
        colors = new Color32[]{new Color32(255,255,255,255),
            new Color32(180,180,180,255),
            new Color32(142,172,180,255),
            new Color32(84,110,137,255),
            new Color32(198,143,137,255)
        };

        defaultColor = GUI.color;

        if (searchingForDialogue)
        {
            GUI.FocusControl("filterSearch");
        }

        if (e.keyCode == KeyCode.Return && e.type == EventType.keyUp && GUIUtility.keyboardControl == 0)
        {
            if (!searchingForDialogue)
            {
                searchingForDialogue = true;
                searchWord = "";
                Repaint();
                return;
            }
        }

        GUIStyle sty = EditorStyles.toolbar;
        sty.fixedHeight = 18;
        GUILayout.BeginHorizontal(sty);
        DrawToolbar();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(sty);
        DrawToolbar2();
        GUILayout.EndHorizontal();

        //GUILayout.Label("Bounds: " + canvas.ToString());
        //GUILayout.Label("ScrollArea: " + scrollArea.ToString());
        //GUILayout.Label("lerpTime: " + lerpFocusTime.ToString());
        //GUILayout.Label("Hot control: " + GUIUtility.hotControl);
        //GUILayout.Label("Keyboard Control: " + GUIUtility.keyboardControl);
        //GUILayout.Label("Focused control: " + GUI.GetNameOfFocusedControl());
        //GUILayout.Label("RealTimeSinceStartup: " + Time.realtimeSinceStartup.ToString());
        //GUILayout.Label("Focusing: " + lerpFocusTime.ToString() + " Time: " + focusTimer.ToString());
        //GUILayout.Label("videroot: " + Application.dataPath + "/../" + VIDE_EditorDB.videRoot + "/Resources/");
        //GUILayout.Label(Application.systemLanguage.ToString());



        if (searchingForDialogue)
        {
            SearchDialogue();
            return;
        }

        scrollArea = GUI.BeginScrollView(new Rect(0, 37, position.width, position.height - 37), scrollArea, canvas, GUIStyle.none, GUIStyle.none);

        DrawGrid();

        defaultColor = GUI.color;


        //handle input events
        if (editEnabled)
        {

            if (!dragnWindows)
            {
                if (e.type == EventType.MouseUp && GUIUtility.hotControl == 0 && e.button == 1)
                {
                    startDiag.x = e.mousePosition.x - 150;
                    startDiag.y = e.mousePosition.y - 25;
                    GUIUtility.keyboardControl = 0;
                    Repaint();
                }
            }

            if (e.type == EventType.MouseDown)
            {
                if (GUIUtility.hotControl != 0) GUIUtility.hotControl = 0;
                if (lerpFocusTime)
                    lerpFocusTime = false;
            }

            if (position.Contains(GUIUtility.GUIToScreenPoint(e.mousePosition)))
            {
                if (e.type == EventType.MouseDrag && e.button == 0 && dragNewNode == 0) //Drag all around
                {
                    if (GUIUtility.hotControl == 0)
                    {
                        dragnWindows = true;
                        if (e.delta.x < 200 && e.delta.y < 200)
                        {
                            scrollArea = new Vector2(scrollArea.x -= e.delta.x, scrollArea.y -= e.delta.y);
                            Repaint();
                        }
                    }
                }
            }
            else
            {
                if (dragnWindows) // Stop dragging windows
                {
                    dragnWindows = false;
                    Repaint();
                    repaintLines = true;
                }
            }

            if (e.type == EventType.MouseUp)
            {
                if (draggingLine) //Connect node detection
                {
                    TryConnectToDialogueNode(e.mousePosition, draggedCom);
                    TryConnectAction(e.mousePosition, draggedAction);
                    needSave = true;
                    Repaint();
                    GUIUtility.hotControl = 0;
                    repaintLines = true;
                }
                if (dragnWindows) // Stop dragging windows
                {
                    dragnWindows = false;
                    Repaint();
                    repaintLines = true;
                }
                draggingLine = false;
            }
        }
        //Draw connection line
        if (editEnabled)
        {
            if (draggingLine)
            {
                DrawNodeLine3(dragStart, e.mousePosition);
                Repaint();
            }
        }

        //Draw all connected lines
        if (e.type == EventType.Repaint)
        {
            if (dragnWindows)
            {
                if (!db.previewPanning)
                    DrawLines();
            }
            else
            {
                DrawLines();
            }
        }

        //Here we'll draw all of the windows
        BeginWindows();

        int setID = 0;
        int ansID = 0;
        int acID = 0;
        GUI.enabled = editEnabled;
        GUIStyle st = new GUIStyle(GUI.skin.window);

        st.fontStyle = FontStyle.Bold;
        st.fontSize = 12;
        st.richText = true;
        st.wordWrap = true;

        if (!newFile && !deletePopup && !overwritePopup)
        {
            if (db.playerDiags.Count > 0)
            {
                for (; setID < db.playerDiags.Count; setID++)
                {
                    if (!CheckInsideWindow(db.playerDiags[setID].rect)) continue;

                    if (db.playerDiags[setID].isPlayer)
                        GUI.color = new Color32(180, 160, 160, 255);
                    else
                        GUI.color = new Color32(160, 160, 180, 255);


                    if (!dragnWindows)
                    {
                        db.playerDiags[setID].rect = GUILayout.Window(setID, db.playerDiags[setID].rect, DrawNodeWindow, "Dialogue Node - <color=white>ID: " + db.playerDiags[setID].ID.ToString() + "</color>", st, GUILayout.Height(40));
                        Rect rawrect = db.playerDiags[setID].rect;
                        rawrect.x = Mathf.Floor(rawrect.x / gridSize) * gridSize;
                        rawrect.y = Mathf.Floor(rawrect.y / gridSize) * gridSize;
                        db.playerDiags[setID].rect = new Rect(rawrect.x, rawrect.y, db.playerDiags[setID].rect.width, db.playerDiags[setID].rect.height);
                    }
                    else
                    {
                        if (db.previewPanning)
                        {
                            db.playerDiags[setID].rect = GUILayout.Window(setID, db.playerDiags[setID].rect, DrawEmptyWindow, "Dialogue Node - <color=white>ID: " + db.playerDiags[setID].ID.ToString() + "</color>", st, GUILayout.Height(40));
                        }
                        else
                        {
                            db.playerDiags[setID].rect = GUILayout.Window(setID, db.playerDiags[setID].rect, DrawNodeWindow, "Dialogue Node - <color=white>ID: " + db.playerDiags[setID].ID.ToString() + "</color>", st, GUILayout.Height(40));
                        }
                    }
                }
            }
            GUI.color = defaultColor;
            if (db.actionNodes.Count > 0)
            {
                for (; acID < db.actionNodes.Count; acID++)
                {
                    if (!CheckInsideWindow(db.actionNodes[acID].rect)) continue;

                    GUI.color = new Color32(160, 180, 160, 255);


                    if (!dragnWindows)
                    {
                        db.actionNodes[acID].rect = GUILayout.Window(acID + setID + ansID, db.actionNodes[acID].rect, DrawActionWindow, "Action Node - <color=white>ID: " + db.actionNodes[acID].ID.ToString() + "</color>", st, GUILayout.Height(40), GUILayout.Width(200));
                        Rect rawrect = db.actionNodes[acID].rect;
                        rawrect.x = Mathf.Floor(rawrect.x / gridSize) * gridSize;
                        rawrect.y = Mathf.Floor(rawrect.y / gridSize) * gridSize;
                        db.actionNodes[acID].rect = new Rect(rawrect.x, rawrect.y, db.actionNodes[acID].rect.width, db.actionNodes[acID].rect.height);
                    }
                    else
                    {
                        if (db.previewPanning)
                        {
                            db.actionNodes[acID].rect = GUILayout.Window(acID + setID + ansID, db.actionNodes[acID].rect, DrawEmptyWindow, "Action Node - <color=white>ID: " + db.actionNodes[acID].ID.ToString() + "</color>", st, GUILayout.Height(40), GUILayout.Width(200));
                        }
                        else
                        {
                            db.actionNodes[acID].rect = GUILayout.Window(acID + setID + ansID, db.actionNodes[acID].rect, DrawActionWindow, "Action Node - <color=white>ID: " + db.actionNodes[acID].ID.ToString() + "</color>", st, GUILayout.Height(40), GUILayout.Width(200));
                        }

                    }
                }

            }
        }


        //Here we check for errors in the node structure

        playerReady = true;
        hasID = false;
        for (int i = 0; i < db.playerDiags.Count; i++)
        {
            if (db.startID == db.playerDiags[i].ID)
            {
                hasID = true;
            }
        }
        for (int i = 0; i < db.actionNodes.Count; i++)
        {
            if (db.startID == db.actionNodes[i].ID)
            {
                hasID = true;
            }
        }

        if (e.type == EventType.Layout)
        {
            showError = false;
            if (!playerReady)
                showError = true;
        }
        GUI.color = colors[0];
        GUI.SetNextControlName("startD");

        GUI.enabled = true;
        if (newFile)
        {
            fWin = new Rect(canvas.x + scrollArea.x + position.width / 4, canvas.y + scrollArea.y + position.height / 4, position.width / 2, 0);
            fWin = GUILayout.Window(99998, fWin, DrawNewFileWindow, "New Dialogue:");
            GUI.FocusWindow(99998);
        }
        if (overwritePopup)
        {
            fWin = new Rect(canvas.x + scrollArea.x + position.width / 4, canvas.y + scrollArea.y + position.height / 4, position.width / 2, 0);
            fWin = GUILayout.Window(99997, fWin, DrawOverwriteWindow, "Overwrite?");
            GUI.FocusWindow(99997);
        }
        if (deletePopup)
        {
            fWin = new Rect(canvas.x + scrollArea.x + position.width / 4, canvas.y + scrollArea.y + position.height / 4, position.width / 2, 0);
            fWin = GUILayout.Window(99996, fWin, DrawDeleteWindow, "Are you sure?");
            GUI.FocusWindow(99996);
        }
        EndWindows();

        if (e.button == 0 && e.type == EventType.MouseDown)
        {
            areYouSure = false;
            GUIUtility.keyboardControl = 0;
            Repaint();
        }
        GUI.EndScrollView();

        if (editEnabled)
            if (e.type == EventType.MouseUp)
            {
                if (dragNewNode > 0) // Stop dragging windows
                {
                    addNewNode(e.mousePosition, dragNewNode);
                    dragNewNode = 0;
                    Repaint();
                }
            }

        //Draws dragged node
        if (dragNewNode > 0)
        {
            dragNewNodeRect.x = e.mousePosition.x - 50;
            dragNewNodeRect.y = e.mousePosition.y - 20;
            dragNewNodeRect.width = 100;
            dragNewNodeRect.height = 40;
            if (dragNewNode == 1)
                GUI.DrawTexture(dragNewNodeRect, newNodeIcon, ScaleMode.StretchToFill);
            if (dragNewNode == 2)
                GUI.DrawTexture(dragNewNodeRect, newNodeIcon3, ScaleMode.StretchToFill);
            Repaint();
        }

    }

    Texture2D gridTex;

    void DrawGrid()
    {
        Color dCol = Handles.color;
        Color gridColor = new Color(0.3f, 0.3f, 0.3f, 1);

        GUI.DrawTexture(canvas, gridTex, ScaleMode.StretchToFill);

        int wlines = Mathf.RoundToInt(canvas.width / gridSize);
        int hlines = Mathf.RoundToInt(canvas.height / gridSize);
        Handles.color = gridColor;

        if (db.previewPanning) return;


        for (int i = 0; i < wlines; i++)
        {
            if (i % 4 == 0)
                Handles.DrawLine(new Vector3(canvas.x, canvas.y + i * gridSize, 0), new Vector3(canvas.x + canvas.width, canvas.y + i * gridSize, 0));
        }
        for (int i = 0; i < hlines; i++)
        {
            if (i % 4 == 0)
                Handles.DrawLine(new Vector3(canvas.x + i * gridSize, canvas.y, 0), new Vector3(canvas.x + i * gridSize, canvas.height, 0));
        }
        Handles.color = dCol;
    }

    bool CheckInsideWindow(Rect rr)
    {
        Rect viewport = canvas;
        viewport.x += scrollArea.x;
        viewport.y += scrollArea.y;
        viewport.width = position.width;
        viewport.height = position.height;

        Rect r = rr;


        if (viewport.Contains(new Vector2(r.x, r.y)))
        {
            return true;
        }
        if (viewport.Contains(new Vector2(r.x + r.width, r.y + r.height)))
        {
            return true;
        }
        if (viewport.Contains(new Vector2(r.x + r.width, r.y)))
        {
            return true;
        }
        if (viewport.Contains(new Vector2(r.x, r.y + r.height)))
        {
            return true;
        }
        return false;
    }

    void DrawEmptyWindow(int id)
    {
        GUI.color = Color.clear;
        GUILayout.Box("", GUILayout.Width(200), GUILayout.Height(50));
        GUI.color = Color.white;

    }

    void DrawNodeWindow(int id)
    {
        if (id >= db.playerDiags.Count)
            return;

        Event e = Event.current;

        GUI.enabled = editEnabled;
        bool dontDrag = false;
        if (e.type == EventType.MouseUp)
        {
            draggingLine = false;
            dontDrag = true;
        }

        GUI.color = new Color(0, 0, 0, 0.2f);
        GUILayout.BeginVertical(GUI.skin.box);
        GUI.color = defaultColor;

        GUILayout.BeginHorizontal();
        GUI.color = colors[1];
        string delText = "Delete Node";
        if (areYouSureIndex == id)
            if (areYouSure) { delText = "Sure?"; GUI.color = new Color32(176, 128, 54, 255); }
        if (GUILayout.Button(delText, GUILayout.Width(80)))
        {
            if (areYouSureIndex != id) areYouSure = false;
            if (!areYouSure)
            {
                areYouSure = true;
                areYouSureIndex = id;
            }
            else
            {
                areYouSure = false;
                areYouSureIndex = 0;
                removeSet(db.playerDiags[id]);
                needSave = true;
                return;
            }
        }
        if (e.type == EventType.MouseDown)
        {
            areYouSure = false;
            Repaint();
        }
        GUI.color = defaultColor;
        if (GUILayout.Button("Add comment", GUILayout.Width(140)))
        {
            areYouSure = false;
            addComment(db.playerDiags[id]);
            needSave = true;
        }

        string isp = "Is Player";
        if (!db.playerDiags[id].isPlayer) isp = "Is NPC";

        if (db.playerDiags[id].isPlayer) GUI.color = new Color32(180, 160, 160, 255);
        else GUI.color = new Color32(160, 160, 180, 255);

        if (GUILayout.Button(isp))
        {
            Undo.RecordObject(db, "Set Node type");
            db.playerDiags[id].isPlayer = !db.playerDiags[id].isPlayer;
            if (!db.playerDiags[id].isPlayer)
            {
                for (int i = 0; i < db.playerDiags[id].comment.Count; i++)
                {
                    if (i == 0) continue;
                    db.playerDiags[id].comment[i].outNode = null;
                    db.playerDiags[id].comment[i].outAction = null;

                }
            }
        }
        GUILayout.EndHorizontal();
        GUI.color = defaultColor;

        GUILayout.EndVertical();

        for (int i = 0; i < db.playerDiags[id].comment.Count; i++)
        {
            if (db.playerDiags[id].comment.Count > 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label((i).ToString() + ". ", GUILayout.Width(20));
                if (i == 0) GUILayout.Space(24);
                if (i != 0)
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        areYouSure = false;
                        removeComment(db.playerDiags[id].comment[i]);
                        needSave = true;
                        return;
                    }
                GUIStyle stf = new GUIStyle(GUI.skin.textField);
                GUIStyle exD = new GUIStyle(GUI.skin.textField);
                exD.wordWrap = false;
                stf.wordWrap = true;
                EditorGUI.BeginChangeCheck();
                string testText = EditorGUILayout.TextArea(db.playerDiags[id].comment[i].text, stf, GUILayout.Width(200));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(db, "Edited Player comment");
                    db.playerDiags[id].comment[i].text = testText;
                    needSave = true;
                }

                string showmore = "+";
                if (db.playerDiags[id].comment[i].showmore) showmore = "-";

                if (GUILayout.Button(showmore, GUILayout.Width(20)))
                {
                    Undo.RecordObject(db, "Show Audio and Sprites");
                    db.playerDiags[id].comment[i].showmore = !db.playerDiags[id].comment[i].showmore;
                    Repaint();
                    needSave = true;
                }

                bool drawConnectors = true;

                if (!db.playerDiags[id].isPlayer)
                {
                    if (i > 0)
                        drawConnectors = false;
                    else
                        drawConnectors = true;
                }

                if (drawConnectors)
                {
                    if (db.playerDiags[id].comment[i].outNode == null && db.playerDiags[id].comment[i].outAction == null)
                    {
                        Rect lr;
                        GUI.color = Color.green;
                        if (GUILayout.RepeatButton("O", GUILayout.Width(30)))
                        {
                            areYouSure = false;
                            lr = GUILayoutUtility.GetLastRect();
                            lr = new Rect(lr.x + db.playerDiags[id].rect.x + 30, lr.y + db.playerDiags[id].rect.y + 7, 0, 0);
                            if (!draggingLine && !dontDrag)
                            {
                                draggedCom = db.playerDiags[id].comment[i];
                                draggedAction = null;
                                dragStart = new Vector2(lr.x, lr.y);
                                draggingLine = true;
                                needSave = true;
                            }
                        }
                        GUI.color = defaultColor;
                    }
                    else
                    {

                        GUI.color = defaultColor;
                        if (GUILayout.Button("x", GUILayout.Width(30)))
                        {
                            areYouSure = false;
                            breakConnection(0, db.playerDiags[id].comment[i], null);
                            needSave = true;
                        }
                        if (e.type == EventType.Repaint)
                        {
                            db.playerDiags[id].comment[i].outRect = GUILayoutUtility.GetLastRect();
                        }
                    }
                }


                GUILayout.EndHorizontal();

                if (db.playerDiags[id].comment[i].showmore)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(26);
                    GUIStyle visStyle = new GUIStyle(GUI.skin.button);
                    if (db.playerDiags[id].comment[i].visible)
                        visStyle.normal.background = visON;
                    else
                        visStyle.normal.background = visOFF;

                    visStyle.fixedHeight = 0;
                    visStyle.fixedWidth = 0;
                    visStyle.padding = new RectOffset(0, 0, 0, 0);
                    visStyle.margin = new RectOffset(0, 0, 0, 0);

                    if (GUILayout.Button("", visStyle, GUILayout.Width(18), GUILayout.Height(18)))
                    {
                        Undo.RecordObject(db, "Set Comment visibility");
                        db.playerDiags[id].comment[i].visible = !db.playerDiags[id].comment[i].visible;
                        needSave = true;
                    }

                    EditorGUI.BeginChangeCheck();
                    string exd = EditorGUILayout.TextArea(db.playerDiags[id].comment[i].extraData, exD, GUILayout.Width(100));
                    GUI.color = Color.white;
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(db, "Edited Player extra data");
                        db.playerDiags[id].comment[i].extraData = exd;
                        needSave = true;
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUI.color = Color.white;
                    GUILayout.Box(" ", GUILayout.ExpandWidth(true), GUILayout.Height(2));
                    GUI.color = defaultColor;
                }

            }
        }
        GUI.color = new Color(0, 0, 0, 0.2f);
        GUILayout.BeginVertical(GUI.skin.box);
        GUI.color = defaultColor;

        GUIStyle stf2 = new GUIStyle(GUI.skin.textField);
        stf2.wordWrap = true;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Tag: ", GUILayout.Width(30));

        EditorGUI.BeginChangeCheck();
        string pt = EditorGUILayout.TextField(db.playerDiags[id].playerTag, stf2, GUILayout.Width(80));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(db, "Set player Tag");
            db.playerDiags[id].playerTag = pt;
            needSave = true;
        }

        GUI.color = new Color(0.7f, 0.8f, 0.4f, 1);
        if (db.playerDiags[id].sprite != null || db.playerDiags[id].vars.Count > 0)
        {
            Color c = new Color(0.1f, 0.4f, 0.8f, 1);
            GUI.color = c;
        }

        GUILayout.FlexibleSpace();
        string exText = (db.playerDiags[id].expand) ? "-" : "+";
        if (GUILayout.Button(exText, GUILayout.Width(60)))
        {
            Undo.RecordObject(db, "Set Expand");
            db.playerDiags[id].expand = !db.playerDiags[id].expand;
            needSave = true;
        }

        GUILayout.EndHorizontal();
        GUI.color = defaultColor;

        /* Expand stuff */

        if (db.playerDiags[id].expand)
        {
            GUIStyle st = new GUIStyle(GUI.skin.label);
            Vector2 coff = st.contentOffset;
            coff.y -= 5;
            st.contentOffset = coff;
            GUILayout.Label("__________________________________________", st);
            coff.y += 7;
            st.contentOffset = coff;
            st.fontStyle = FontStyle.Bold;

            EditorGUI.BeginChangeCheck();
            Sprite sp = (Sprite)EditorGUILayout.ObjectField("Node Sprite: ", db.playerDiags[id].sprite, typeof(Sprite), false);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(db, "Set Sprite");
                db.playerDiags[id].sprite = sp;
                needSave = true;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Extra Variables: ", st);
            if (GUILayout.Button("Add"))
            {
                Undo.RecordObject(db, "Add Extra Variable");
                db.playerDiags[id].vars.Add(string.Empty);
                db.playerDiags[id].varKeys.Add("Key" + db.playerDiags[id].vars.Count.ToString());
                needSave = true;
            }

            GUILayout.EndHorizontal();

            for (int i = 0; i < db.playerDiags[id].vars.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(i.ToString() + ". ", GUILayout.Width(20));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Undo.RecordObject(db, "Removed Extra Variable");
                    db.playerDiags[id].vars.RemoveAt(i);
                    db.playerDiags[id].varKeys.RemoveAt(i);
                    needSave = true;
                    break;
                }

                EditorGUI.BeginChangeCheck();
                string key = EditorGUILayout.TextField(db.playerDiags[id].varKeys[i], GUILayout.Width(80));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(db, "Set key");
                    db.playerDiags[id].varKeys[i] = key;
                    needSave = true;

                }

                EditorGUI.BeginChangeCheck();
                string val = EditorGUILayout.TextField(db.playerDiags[id].vars[i]);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(db, "Set value");
                    db.playerDiags[id].vars[i] = val;
                    needSave = true;

                }

                GUILayout.EndHorizontal();

            }

        }

        GUILayout.EndVertical();

        if (e.button == 0 && e.type == EventType.MouseDown)
        {
            areYouSure = false;
            Repaint();
        }
        if (e.commandName == "UndoRedoPerformed")
            Repaint();

        if (e.type == EventType.MouseDrag)
        {
            Undo.RecordObject(db, "Dragged node");
        }

        if (!lerpFocusTime && e.button == 0)
        {
            if (position.Contains(GUIUtility.GUIToScreenPoint(e.mousePosition)))
                GUI.DragWindow();
        }

        if (!lerpFocusTime && e.button == 1 && e.type == EventType.MouseDown)
        {
            dragNewNode = 1;
            copiedNode = db.playerDiags[id];
        }

        if (db.playerDiags[id].rectRaw != db.playerDiags[id].rect)
            needSave = true;

        db.playerDiags[id].rectRaw = db.playerDiags[id].rect;


    }

    void DragOtherWindows(VIDE_EditorDB.DialogueNode com, Vector2 off)
    {
        for (int i = 0; i < db.playerDiags.Count; i++)
        {
            if (db.playerDiags[i] == com) continue;
            Rect r = db.playerDiags[i].rect;
            r.x += off.x;
            r.y += off.y;
            db.playerDiags[i].rect = r;
        }
    }


    void DrawActionWindow(int id)
    {
        GUI.enabled = editEnabled;
        bool dontDrag = false;

        int aID = id - (db.playerDiags.Count);
        if (aID < 0)
            aID = 0;

        if (aID >= db.actionNodes.Count)
            return;

        if (Event.current.type == EventType.MouseUp)
        {
            draggingLine = false;
            dontDrag = true;
        }

        GUI.color = new Color(0, 0, 0, 0.2f);
        GUILayout.BeginVertical(GUI.skin.box);
        GUI.color = defaultColor;

        GUILayout.BeginHorizontal();

        GUI.color = new Color32(230, 230, 230, 255);
        string delText = "Delete Node";
        if (areYouSureIndex == id)
            if (areYouSure) { delText = "Sure?"; GUI.color = new Color32(176, 128, 54, 255); }
        if (GUILayout.Button(delText))
        {
            if (areYouSureIndex != id) areYouSure = false;
            if (!areYouSure)
            {
                areYouSure = true;
                areYouSureIndex = id;
            }
            else
            {
                areYouSure = false;
                areYouSureIndex = 0;
                removeAction(db.actionNodes[aID]);
                needSave = true;
                return;
            }
        }
        if (Event.current.type == EventType.MouseDown)
        {
            areYouSure = false;
            Repaint();
        }

        GUI.color = defaultColor;

        GUIStyle stf = new GUIStyle(GUI.skin.textField);
        stf.wordWrap = true;

        if (db.actionNodes[aID].outPlayer == null && db.actionNodes[aID].outAction == null)
        {
            Rect lr;
            GUI.color = Color.green;
            if (GUILayout.RepeatButton("O", GUILayout.Width(30)))
            {
                areYouSure = false;
                lr = GUILayoutUtility.GetLastRect();
                lr = new Rect(lr.x + db.actionNodes[aID].rect.x + 30, lr.y + db.actionNodes[aID].rect.y + 7, 0, 0);
                if (!draggingLine && !dontDrag)
                {
                    draggedCom = null;
                    draggedAction = db.actionNodes[aID];
                    dragStart = new Vector2(lr.x, lr.y);
                    draggingLine = true;
                    needSave = true;
                }
            }
            GUI.color = Color.white;

        }
        else
        {
            if (GUILayout.Button("x", GUILayout.Width(30)))
            {
                areYouSure = false;
                breakConnection(1, null, db.actionNodes[aID]);
                needSave = true;
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

            GUI.color = new Color(0, 0, 0, 0.2f);
            GUILayout.BeginVertical(GUI.skin.box);

            GUI.color = defaultColor;

            GUI.color = new Color32(170, 200, 170, 255);

            if (GUILayout.Button("Reset and fetch"))
            {
                Undo.RecordObject(db, "Reset and fetch");

                var objects = Resources.FindObjectsOfTypeAll<GameObject>();
                db.actionNodes[aID].nameOpts.Clear();

                int c = 0;
                db.actionNodes[aID].nameOpts.Add("[No object]");

                foreach (GameObject g in objects)
                {
                    if (g.activeInHierarchy && checkUseful(g))
                        db.actionNodes[aID].nameOpts.Add(g.name);

                    c++;
                }

                db.actionNodes[aID].Clean();

                //Fill up methods dictionary
                var gos = Resources.FindObjectsOfTypeAll<GameObject>();
                db.actionNodes[aID].methods = new Dictionary<string, string>();
                for (int i = 0; i < gos.Length; i++)
                {
                    if (gos[i].activeInHierarchy && checkUseful(gos[i]))
                    {
                        List<MethodInfo> methodz = GetMethods(gos[i]);

                        for (int ii = 0; ii < methodz.Count; ii++)
                        {
                            if (!db.actionNodes[aID].methods.ContainsKey(gos[i].name + ii.ToString()))
                                db.actionNodes[aID].methods.Add(gos[i].name + ii.ToString(), methodz[ii].Name);
                        }
                    }
                }

                db.actionNodes[aID].opts = new string[] { "[No method]" };

                needSave = true;
            }

            GUI.color = Color.white;

            if (!db.actionNodes[aID].editorRefreshed && Event.current.type == EventType.repaint)
            {
                db.actionNodes[aID].editorRefreshed = true;

                if (db.actionNodes[aID].nameIndex != 0)
                {
                    db.actionNodes[aID].gameObjectName = db.actionNodes[aID].nameOpts[db.actionNodes[aID].nameIndex];
                }
                else
                {
                    db.actionNodes[aID].gameObjectName = "[No object]";
                    db.actionNodes[aID].methodName = "[No method]";
                    db.actionNodes[aID].methodIndex = 0;
                    db.actionNodes[aID].paramType = -1;
                }

                if (db.actionNodes[aID].methodIndex != 0)
                {
                    db.actionNodes[aID].methodName = db.actionNodes[aID].opts[db.actionNodes[aID].methodIndex];
                }
                else
                {
                    db.actionNodes[aID].methodName = "[No method]";
                    db.actionNodes[aID].methodIndex = 0;
                    db.actionNodes[aID].paramType = -1;
                }

                Repaint();
                return;
            }

            if (db.actionNodes[aID].nameOpts.Count > 0)
            {
                EditorGUI.BeginChangeCheck();
                int idx = EditorGUILayout.Popup(db.actionNodes[aID].nameIndex, db.actionNodes[aID].nameOpts.ToArray());
                if (EditorGUI.EndChangeCheck()) //Pick name
                {
                    Undo.RecordObject(db, "Changed name Index");
                    db.actionNodes[aID].nameIndex = idx;
                    db.actionNodes[aID].gameObjectName = db.actionNodes[aID].nameOpts[db.actionNodes[aID].nameIndex];
                    db.actionNodes[aID].methodName = "[No method]";
                    db.actionNodes[aID].methodIndex = 0;
                    db.actionNodes[aID].paramType = -1;

                    List<string> opti = new List<string>();
                    opti.Add("[No method]");

                    for (int x = 0; x < 10000; x++)
                    {
                        if (db.actionNodes[aID].methods.ContainsKey(db.actionNodes[aID].gameObjectName + x.ToString()))
                        {
                            opti.Add(db.actionNodes[aID].methods[db.actionNodes[aID].gameObjectName + x.ToString()]);
                        }
                        else
                        {
                            break;
                        }
                    }
                    db.actionNodes[aID].opts = opti.ToArray();

                    needSave = true;
                }
            }

            EditorGUI.BeginChangeCheck();
            int meth = EditorGUILayout.Popup(db.actionNodes[aID].methodIndex, db.actionNodes[aID].opts);

            if (EditorGUI.EndChangeCheck()) //Pick method
            {
                Undo.RecordObject(db, "Changed method index");
                db.actionNodes[aID].methodIndex = meth;
                db.actionNodes[aID].methodName = db.actionNodes[aID].opts[db.actionNodes[aID].methodIndex];

                GameObject ob = GameObject.Find(db.actionNodes[aID].gameObjectName);
                var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == db.actionNodes[aID].gameObjectName);

                foreach (GameObject g in objects)
                {
                    if (g.activeInHierarchy && checkUseful(g))
                    {
                        ob = g;
                        break;
                    }

                }

                List<MethodInfo> methods = GetMethods(ob);


                if (db.actionNodes[aID].methodIndex > 0)
                    db.actionNodes[aID].paramType = checkParam(methods[db.actionNodes[aID].methodIndex - 1]);
                else
                    db.actionNodes[aID].paramType = -1;

                needSave = true;

            }


            GUI.color = Color.white;
            GUILayout.BeginHorizontal();


            if (db.actionNodes[aID].paramType > 0)
                GUILayout.Label("Param: ", GUILayout.Width(60));


            if (db.actionNodes[aID].paramType == 1)
            {
                EditorGUI.BeginChangeCheck();
                bool parab = EditorGUILayout.Toggle(db.actionNodes[aID].param_bool, GUILayout.Width(50));
                if (EditorGUI.EndChangeCheck()) //Pick method
                {
                    Undo.RecordObject(db, "Changed param");
                    db.actionNodes[aID].param_bool = parab;
                    needSave = true;
                }
            }

            if (db.actionNodes[aID].paramType == 2)
            {
                EditorGUI.BeginChangeCheck();
                string ps = EditorGUILayout.TextField(db.actionNodes[aID].param_string, GUILayout.Width(100));
                if (EditorGUI.EndChangeCheck()) //Pick method
                {
                    Undo.RecordObject(db, "Changed param");
                    db.actionNodes[aID].param_string = ps;
                    needSave = true;
                }
            }
            if (db.actionNodes[aID].paramType == 3)
            {
                EditorGUI.BeginChangeCheck();
                int pi = EditorGUILayout.IntField(db.actionNodes[aID].param_int, new GUIStyle(GUI.skin.textField), GUILayout.Width(100));
                if (EditorGUI.EndChangeCheck()) //Pick method
                {
                    Undo.RecordObject(db, "Changed param");
                    db.actionNodes[aID].param_int = pi;
                    needSave = true;
                }
            }
            if (db.actionNodes[aID].paramType == 4)
            {
                EditorGUI.BeginChangeCheck();
                float pf = EditorGUILayout.FloatField(db.actionNodes[aID].param_float, new GUIStyle(GUI.skin.textField), GUILayout.Width(100));
                if (EditorGUI.EndChangeCheck()) //Pick method
                {
                    Undo.RecordObject(db, "Changed param");
                    db.actionNodes[aID].param_float = pf;
                    needSave = true;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

        GUI.color = new Color(0, 0, 0, 0.2f);
        GUILayout.BeginVertical(GUI.skin.box);
        GUI.color = defaultColor;

        if (db.actionNodes[aID].pauseHere) GUI.color = Color.green; else GUI.color = Color.white;
        if (GUILayout.Button("Pause Here: " + db.actionNodes[aID].pauseHere.ToString()))
        {
            Undo.RecordObject(db, "Changed pause here");
            db.actionNodes[aID].pauseHere = !db.actionNodes[aID].pauseHere;
            needSave = true;
        }
        GUI.color = Color.white;

        GUILayout.EndVertical();

        if (Event.current.commandName == "UndoRedoPerformed")
            Repaint();

        if (Event.current.type == EventType.MouseDrag)
        {
            Undo.RecordObject(db, "Dragged node");
        }


        if (!lerpFocusTime && Event.current.button == 0)
        {
            if (position.Contains(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)))
                GUI.DragWindow();
        }

        if (!lerpFocusTime && Event.current.button == 1 && Event.current.type == EventType.MouseDown)
        {
            dragNewNode = 2;
            copiedNode = db.actionNodes[aID];
        }


        if (db.actionNodes[aID].rectRaw != db.actionNodes[aID].rect)
            needSave = true;

        db.actionNodes[aID].rectRaw = db.actionNodes[aID].rect;

    }

    bool notInBlackList(MonoBehaviour mb)
    {
        for (int i = 0; i < namespaceBlackList.Length; i++)
        {
            if (mb.GetType().Namespace != null && mb.GetType().Namespace.Contains(namespaceBlackList[i]))
                return false;
        }
        return true;
    }

    bool checkUseful(GameObject g)
    {
        bool useful = false;
        var methods = new List<MethodInfo>();
        var mbs = g.GetComponents<MonoBehaviour>();

        var publicFlags = bf.Instance | bf.Public | bf.DeclaredOnly | bf.IgnoreReturn;

        foreach (MonoBehaviour mb in mbs)
        {
            if (mb != null)
                if (notInBlackList(mb))
                {
                    methods.AddRange(mb.GetType().GetMethods(publicFlags));
                }
        }

        string[] ops = GetOptions(methods);

        if (ops.Length > 1)
            useful = true;
        else
            useful = false;

        if (mbs.Length < 1)
            useful = false;

        return useful;
    }

    int checkParam(MethodInfo m)
    {
        ParameterInfo[] ps = m.GetParameters();

        if (ps.Length == 1)
        {
            if (ps[0].ParameterType == typeof(System.Boolean))
            {
                return 1;
            }
            if (ps[0].ParameterType == typeof(System.String))
            {
                return 2;
            }
            if (ps[0].ParameterType == typeof(System.Int32))
            {
                return 3;
            }
            if (ps[0].ParameterType == typeof(System.Single))
            {
                return 4;
            }
            return -1;
        }

        if (ps.Length > 1)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    string[] GetOptions(List<MethodInfo> ms)
    {
        List<string> str = new List<string>();

        str.Add("[No object]");

        foreach (MethodInfo m in ms)
        {
            ParameterInfo[] ps = m.GetParameters();

            if (ps.Length < 2 && m.ReturnType == typeof(void))
            {
                if (checkParam(m) > -1)
                    str.Add(m.Name);
            }
        }
        return str.ToArray();
    }

    List<MethodInfo> GetMethods(GameObject obj)
    {
        var methods = new List<MethodInfo>();
        var methodsFiltered = new List<MethodInfo>();

        if (obj == null) { return methods; }

        var mbs = obj.GetComponents<MonoBehaviour>();

        var publicFlags = bf.Instance | bf.Public | bf.DeclaredOnly | bf.IgnoreReturn;

        foreach (MonoBehaviour mb in mbs)
        {
            if (notInBlackList(mb))
            {
                methods.AddRange(mb.GetType().GetMethods(publicFlags));
            }
        }

        foreach (MethodInfo m in methods)
        {
            if (checkParam(m) > -1)
            {
                methodsFiltered.Add(m);
            }
        }

        return methodsFiltered;
    }

    void DrawNewFileWindow(int id)
    {
        GUI.FocusControl("createFile");
        GUIStyle st = new GUIStyle(GUI.skin.label);
        st.alignment = TextAnchor.UpperCenter;
        st.fontSize = 16;
        st.fontStyle = FontStyle.Bold;
        GUILayout.Label("Please name your new dialogue:", st);
        GUIStyle stf = new GUIStyle(GUI.skin.textField);
        stf.fontSize = 14;
        stf.alignment = TextAnchor.MiddleCenter;
        GUI.SetNextControlName("createFile");
        newFileName = GUILayout.TextField(newFileName, stf, GUILayout.Height(40));
        newFileName = Regex.Replace(newFileName, @"[^a-zA-Z0-9_$&#]", "");
        GUI.color = Color.green;
        if (GUILayout.Button("Create", GUILayout.Height(30)))
        {
            if (tryCreate(newFileName))
            {
                db.fileIndex = db.currentDiag;
                newFileName = "My Dialogue";
                scrollArea = new Vector2(4000, 4000);
                editEnabled = true;
                newFile = false;
                errorMsg = "";
                needSave = true;
                Load(true);
                Repaint();
                saveEditorSettings(db.currentDiag);
                Save();
                AssetDatabase.Refresh();
            }
            else
            {
                errorMsg = "File already exists!";
            }
        }
        if (Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.keyUp)
        {
            if (tryCreate(newFileName))
            {
                db.fileIndex = db.currentDiag;
                newFileName = "My Dialogue";
                editEnabled = true;
                newFile = false;
                errorMsg = "";
                needSave = true;
                Load(true);
                Repaint();
                saveEditorSettings(db.currentDiag);
                Save();
                AssetDatabase.Refresh();
                return;
            }
            else
            {
                errorMsg = "File already exists!";
            }
        }
        GUI.color = defaultColor;
        if (GUILayout.Button("Cancel", GUILayout.Height(20)) || Event.current.keyCode == KeyCode.Escape)
        {
            newFileName = "My Dialogue";
            editEnabled = true;
            newFile = false;
            errorMsg = "";
            Repaint();
        }
        st.normal.textColor = Color.red;
        GUILayout.Label(errorMsg, st);
    }

    void DrawOverwriteWindow(int id)
    {
        GUIStyle st = new GUIStyle(GUI.skin.label);
        st.alignment = TextAnchor.UpperCenter;
        st.fontSize = 16;
        st.fontStyle = FontStyle.Bold;

        if (saveNames.Count > 0)
        {
            if (File.Exists(Application.dataPath + "/../" + VIDE_EditorDB.videRoot + "/Resources/Dialogues/" + saveNames[db.currentDiag] + ".json"))
            {
                GUILayout.Label('"' + saveNames[db.currentDiag] + '"' + " already exists! Overwrite?", st);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Yes!", GUILayout.Height(30), GUILayout.Width(position.width / 4)))
                {
                    Save();
                    needSave = false;
                    newFileName = "My Dialogue";
                    editEnabled = true;
                    overwritePopup = false;
                    newFile = false;
                    errorMsg = "";
                    saveEditorSettings(db.currentDiag);
                    return;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("No", GUILayout.Height(20), GUILayout.Width(position.width / 6)))
                {
                    newFileName = "My Dialogue";
                    editEnabled = true;
                    overwritePopup = false;
                    newFile = false;
                    errorMsg = "";
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
        }
        GUILayout.Label("Save as new...", st);
        GUIStyle stf = new GUIStyle(GUI.skin.textField);
        stf.fontSize = 14;
        stf.alignment = TextAnchor.MiddleCenter;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        newFileName = GUILayout.TextField(newFileName, stf, GUILayout.Height(40), GUILayout.Width(position.width / 4));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        newFileName = Regex.Replace(newFileName, @"[^a-zA-Z0-9_$&#]", "");
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save", GUILayout.Height(20), GUILayout.Width(position.width / 4)))
        {
            if (tryCreate(newFileName))
            {
                db.fileIndex = db.currentDiag;
                Load(false);
                newFileName = "My Dialogue";
                editEnabled = true;
                newFile = false;
                overwritePopup = false;
                errorMsg = "";
                needSave = true;
            }
            else
            {
                errorMsg = "File already exists!";
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Cancel", GUILayout.Height(20), GUILayout.Width(position.width / 6)))
        {
            newFileName = "My Dialogue";
            editEnabled = true;
            overwritePopup = false;
            newFile = false;
            errorMsg = "";
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        st.normal.textColor = Color.red;
        if (errorMsg != "")
            GUILayout.Label(errorMsg, st);
    }

    void DrawDeleteWindow(int id)
    {
        GUIStyle st = new GUIStyle(GUI.skin.label);
        st.alignment = TextAnchor.UpperCenter;
        st.fontSize = 16;
        st.fontStyle = FontStyle.Bold;
        GUILayout.Label("Are you sure you want to delete " + "'" + saveNames[db.fileIndex] + "'?", st);
        GUILayout.Label("LOC file will also get deleted.", st);
        GUILayout.Label("A VIDE_Assign might still have this dialogue assigned to it", st);

        if (GUILayout.Button("Yes", GUILayout.Height(30)) || Event.current.keyCode == KeyCode.Return)
        {
            DeleteDiag();
            db.fileIndex = 0;
            editEnabled = true;
            deletePopup = false;
            newFile = false;
            saveEditorSettings(db.currentDiag);
            CenterAll(false, db.startID, true);
            Repaint();
            return;
        }
        if (GUILayout.Button("No", GUILayout.Height(20)) || Event.current.keyCode == KeyCode.Escape)
        {
            editEnabled = true;
            deletePopup = false;
            newFile = false;
            Repaint();
            return;
        }
    }

    void DrawLines()
    {
        Handles.color = colors[3];
        if (editEnabled)
        {
            if (draggingLine)
            {
                DrawNodeLine3(dragStart, Event.current.mousePosition);
                Repaint();
            }
            for (int i = 0; i < db.playerDiags.Count; i++)
            {
                for (int ii = 0; ii < db.playerDiags[i].comment.Count; ii++)
                {
                    if (db.playerDiags[i].comment[ii].outNode != null)
                    {
                        DrawNodeLine(db.playerDiags[i].comment[ii].outRect,
                        db.playerDiags[i].comment[ii].outNode.rect, db.playerDiags[i].rect);
                    }

                    if (db.playerDiags[i].comment[ii].outAction != null)
                    {
                        DrawNodeLine(db.playerDiags[i].comment[ii].outRect,
                        db.playerDiags[i].comment[ii].outAction.rect, db.playerDiags[i].rect);
                    }
                }


            }
            for (int i = 0; i < db.actionNodes.Count; i++)
            {
                if (db.actionNodes[i].outPlayer != null)
                {
                    DrawActionNodeLine(db.actionNodes[i].rect,
                    db.actionNodes[i].outPlayer.rect);
                }

                if (db.actionNodes[i].outAction != null)
                {
                    DrawActionNodeLine(db.actionNodes[i].rect,
                    db.actionNodes[i].outAction.rect);
                }
            }
        }
        repaintLines = false;

    }

    Vector3 Bezier3(Vector3 s, Vector3 st, Vector3 et, Vector3 e, float t)
    {
        return (((-s + 3 * (st - et) + e) * t + (3 * (s + et) - 6 * st)) * t + 3 * (st - s)) * t + s;
    }

    //Player Node
    void DrawNodeLine(Rect start, Rect end, Rect sPos)
    {
        Color nc = Color.white;

        Vector3 startPos = new Vector3(start.x + sPos.x + 35, start.y + sPos.y + 10, 0);
        Vector3 endPos = new Vector3(end.x, end.y + (end.height / 2), 0);
        float ab = Vector2.Distance(startPos, endPos);
        Vector3 startTan = startPos + Vector3.right * (ab/3);
        Vector3 endTan = endPos + Vector3.left * (ab / 3);

        Handles.DrawBezier(startPos, endPos, startTan, endTan, nc, null, 3);

        //Draw arrow
        DrawArrow(startPos, startTan, endTan, endPos, sPos, start, true);

        if (repaintLines)
        {
            Repaint();
        }
    }

    void DrawArrow(Vector3 startPos, Vector3 startTan, Vector3 endTan, Vector3 endPos, Rect sPos, Rect start, bool hasYpos)
    {
        Handles.BeginGUI();
        float ab = Vector2.Distance(startPos, endPos);
        if (ab < 75) return;

        float dist = 0.4f;

        Vector2 cen = Bezier3(startPos, startTan, endTan, endPos, dist);

        cen = Bezier3(startPos, startTan, endTan, endPos, dist);

        float rot = AngleBetweenVector2(cen, Bezier3(startPos, startTan, endTan, endPos, dist + 0.05f));

        Matrix4x4 matrixBackup = GUI.matrix;
        GUIUtility.RotateAroundPivot(rot + 90, new Vector2(cen.x, cen.y));
        GUI.color = Color.white;
        GUI.DrawTexture(new Rect(cen.x - 15, cen.y - 15, 30, 30), lineIcon, ScaleMode.StretchToFill);
        GUI.color = Color.white;
        GUI.matrix = matrixBackup;

        Handles.EndGUI();
    }

    //Action Node line
    void DrawActionNodeLine(Rect start, Rect end)
    {
        Color nc2 = Color.white;

        Vector3 startPos = new Vector3(start.x + 190, start.y + 30, 0);
        Vector3 endPos = new Vector3(end.x, end.y + (end.height / 2), 0);
        float ab = Vector2.Distance(startPos, endPos);
        Vector3 startTan = startPos + Vector3.right * (ab/3);
        Vector3 endTan = endPos + Vector3.left * (ab / 3);

        Handles.DrawBezier(startPos, endPos, startTan, endTan, nc2, null, 3);

        DrawArrow(startPos, startTan, endTan, endPos, new Rect(0, 0, 0, 0), start, false);

        if (repaintLines)
        {
            Repaint();
        }
    }

    //Connection line
    void DrawNodeLine3(Vector2 start, Vector2 end)
    {
        Vector3 startPos = new Vector3(start.x, start.y, 0);
        Vector3 endPos = new Vector3(end.x, end.y, 0);
        float ab = Vector2.Distance(startPos, endPos);
        Vector3 startTan = startPos + Vector3.right * (ab/3);
        Vector3 endTan = endPos + Vector3.left * (ab / 3);

        Handles.DrawBezier(startPos, endPos, startTan, endTan, colors[0], null, 5);

        DrawArrow(startPos, startTan, endTan, endPos, new Rect(0, 0, 0, 0), new Rect(start.x, start.y, 0, 0), false);
    }

    private float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
    {
        Vector2 diference = vec2 - vec1;
        float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
        return Vector2.Angle(Vector2.right, diference) * sign;
    }

    //Clean the database
    void ClearAll()
    {
        db.playerDiags = new List<VIDE_EditorDB.DialogueNode>();
        db.actionNodes = new List<VIDE_EditorDB.ActionNode>();
    }

    void addNewNode(Vector2 pos, int type)
    {
        if (pos.y < 40) return;

        Undo.RecordObject(db, "Added Node");

        Rect viewport = canvas;
        viewport.x += scrollArea.x;
        viewport.y += scrollArea.y;
        viewport.width = position.width;
        viewport.height = position.height;

        pos.x += canvas.x + scrollArea.x;
        pos.y += canvas.y + scrollArea.y;


        switch (type)
        {
            case 1:
                db.playerDiags.Add(new VIDE_EditorDB.DialogueNode(pos, setUniqueID()));
                if (copiedNode != null)
                {
                    db.CopyLastDialogueNode(copiedNode);
                    copiedNode = null;
                }
                break;
            case 2:
                db.actionNodes.Add(new VIDE_EditorDB.ActionNode(pos, setUniqueID()));
                if (copiedNode != null)
                {
                    db.CopyLastActionNode(copiedNode);
                    copiedNode = null;
                }

                break;
        }
        needSave = true;

        dragNewNode = 0;
        Repaint();
    }

}