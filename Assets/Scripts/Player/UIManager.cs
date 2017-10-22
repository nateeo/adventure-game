using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

	// for npc name, if any
	public Image nameBackground;
	public Text npcName;

	// for animating npc text
	IEnumerator npcTextAnimator;
	private bool animatingText;

	Vector3 original;


	// Use this for initialization
	void Start () {
		inventory_open = false;
		container_NPC.SetActive (false);
		container_PLAYER.SetActive (false);
		nameBackground.enabled = false;
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
		nameBackground.enabled = false;
		npcName.text = "";
		previous_text = null;
		text_NPC.text = "";
		if (conversation.alias != null && conversation.alias != "") {
			nameBackground.enabled = true;
			npcName.text = conversation.alias;
		}
		VD.BeginDialogue (conversation);
	}

	public void CallNext() {
		// skip animation if they press next
		if (VD.isActive) {
			if (animatingText) {
				CutTextAnim ();
				return;
			}
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
			previous_text = data.comments [0];
			npcTextAnimator = AnimateText(data);
			StartCoroutine(npcTextAnimator);
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
		VD.nodeData.commentIndex = choice;
		if (Input.GetMouseButtonUp (0)) {
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
		Cursor.lockState = CursorLockMode.Confined;
		inventory_open = true;
	}

	public void interfaceClosed() {
		playerController.dialogFix = false;
		playerCamera.dialogFix = false;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		inventory_open = false;
	}

	// animating text functions:

	public IEnumerator AnimateText(VD.NodeData data) {
		string text = data.comments [data.commentIndex];
		animatingText = true;

		if (!data.isPlayer) {
			StringBuilder builder = new StringBuilder ();
			int charIndex = 0;
			while (text_NPC.text != text) {
				builder.Append (text [charIndex]);
				charIndex++;
				text_NPC.text = builder.ToString ();
				yield return new WaitForSeconds (0.02f);
			}
		}

		text_NPC.text = data.comments[data.commentIndex]; //Now just copy full text		
		animatingText = false;
	}

	void CutTextAnim()
	{
		StopCoroutine(npcTextAnimator);
		text_NPC.text = VD.nodeData.comments[VD.nodeData.commentIndex]; //Now just copy full text		
		animatingText = false;
	}
}
