using UnityEngine;
using System.Collections;
using VIDE_Data;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public PlayerScript playerController;
	public Rigidbody rigidBody;
	public CameraThirdPerson playerCamera;
	public GameObject container_NPC;
	public GameObject container_PLAYER;
	public Text interactToolTip;
	public Text text_NPC;
	public string previous_text;
	public bool interactTooltip;
	public Text[] text_choices;
	public bool inventory_open;

	Vector3 original;


	// Use this for initialization
	void Start () {
		inventory_open = false;
		container_NPC.SetActive (false);
		container_PLAYER.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Begin(Collider collider, VIDE_Assign conversation) {
		collider.gameObject.transform.LookAt(new Vector3 (
			rigidBody.position.x,
			collider.transform.position.y,
			rigidBody.position.z
											)
		);
		// disable camera rotation and player movement
		interfaceOpen();

		VD.OnNodeChange += UpdateUI;
		VD.OnEnd += End;

		// clean data
		previous_text = null;
		text_NPC.text = "";

		VD.BeginDialogue (conversation);
	}

	public void CallNext() {
		if (VD.isActive) {
			VD.Next ();
		}
	}

	void UpdateUI(VD.NodeData data) {
		container_NPC.SetActive (false);
		container_PLAYER.SetActive (false);
		if (data.isPlayer) {
			if (previous_text != null && previous_text != "") {
				container_NPC.SetActive (true);
				text_NPC.text = previous_text;
			}
			container_PLAYER.SetActive (true);
			for (int i = 0; i < text_choices.Length; i++) {
				Debug.Log (text_choices.Length);
				Debug.Log (data.comments.Length);
				if (i < data.comments.Length) {
					text_choices [i].transform.parent.gameObject.SetActive (true);
					text_choices [i].text = data.comments [i];
				} else {
					text_choices [i].transform.parent.gameObject.SetActive (false);
				}
			}
			text_choices [0].transform.parent.GetComponent<Button> ().Select ();
		} else {
			container_NPC.SetActive (true);
			text_NPC.text = data.comments [data.commentIndex];
			previous_text = data.comments [data.commentIndex];
		}
	}

	void End(VD.NodeData data) {
		VD.OnNodeChange -= UpdateUI;
		VD.OnEnd -= End;
		VD.EndDialogue ();

		// re-enable camera and disable cursor
		interfaceClosed();
		container_NPC.SetActive (false);
		container_PLAYER.SetActive (false);
	}

	void OnDisable() {
		if (container_NPC != null) {
			End (null);
		}
	}

	public void SetPlayerChoice(int choice) {
		Debug.Log ("choose " + choice);
		VD.nodeData.commentIndex = choice;
		if (Input.GetMouseButtonUp (0)) {
			Debug.Log ("HELLO");
			VD.Next ();
		}
	}

	public void interactToolTipDisabled() {
		if (interactToolTip.enabled == true) {
			interactToolTip.enabled = false;
		}
	}

	public void interactToolTipEnabled() {
		if (!VD.isActive && interactToolTip.enabled == false) {
			interactToolTip.enabled = true;
		}
	}

	public void interfaceOpen() {
		playerController.dialogFix = true;
		playerCamera.dialogFix = true;
		Cursor.visible = true;
		inventory_open = true;
	}

	public void interfaceClosed() {
		playerController.dialogFix = false;
		playerCamera.dialogFix = false;
		Cursor.visible = false;
		inventory_open = false;
	}
}
