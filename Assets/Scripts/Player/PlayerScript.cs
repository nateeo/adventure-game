using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using VIDE_Data;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour {
	public bool dialogFix = false;
	public float speed;
	public bool isGrounded;
	public Rigidbody rigidBody;
	Vector3 movement;
	public Vector3 jump;
	public Animator anim;
	public int forceConst;

	// tooltip for various notifications
	public Text toolTip;

	private CharacterController controller;

	private float verticalVelocity;
	private float gravity = 14.0f;
	private float jumpForce = 20.0f;
	private float walkSpeed = 4.0f;
	private float runSpeed = 10.0f;
	private float leftGround = 0f; // y height of ground before a jump
	public float maxHeight;


	// jump modifiers to reduce low-gravity feel
	public float fallMultiplier = 2.5f;
	public float lowJumpMultiplier = 2f;

	// for dialogue management
	public UIManager diagUI;

	// for journal management
	public GameObject journal;
	public bool journalEnabled;
	public InputField input;

	public PlayerInventory inventory;

	public static float NPC_RANGE = 2f;

	// Use this for initialization
	void Start () {
		toolTip.enabled = false;
		journal.SetActive (false);
		journalEnabled = false;

		Screen.lockCursor = true;
		rigidBody = GetComponent<Rigidbody> ();
		//jump = new Vector3 (0.0f, 0.2f, 0.0f);
		anim = GetComponent<Animator> ();
		controller = GetComponent<CharacterController> ();
	}

	// force text selection to end so journal is preserved
	IEnumerator moveEnd()
	{
		yield return 0; // Skip the first frame in which this is called.
		input.MoveTextEnd(false); // Do this during the next frame.
	}

	void Update() {

		if (SceneManager.GetActiveScene ().buildIndex == 0) {
			Destroy(gameObject);
		}

		// handle journal interface, disable the rest if active
		if (Input.GetKeyDown (KeyCode.Minus)) {
			toggleJournal ();
			if (journalEnabled) {
				input.Select ();
				input.ActivateInputField ();
				input.text = input.text.Substring (0, input.text.Length - 1);
				StartCoroutine (moveEnd ());
			}
		}
		// disable all other interaction if journal is enabled
		if (journalEnabled && inventory.enabled) {
			inventory.disable ();
		} else if (inventory.disabled) {
			inventory.enable ();
		}

		if (journalEnabled) {
			return;
		}
		
		if (Input.GetKeyDown (KeyCode.F)) {
			TryInteract ();
		}
		if (Input.GetKeyDown (KeyCode.I)) {
			if (diagUI.inventory_open) {
				diagUI.interfaceClosed();
			} else {
				diagUI.interfaceOpen ();
			}
		}
		diagUI.interactToolTipDisabled();
		Collider[] hits = Physics.OverlapSphere (transform.position, NPC_RANGE);
		for (int i = 0; i < hits.Length; i++) {
			Collider rHit = hits [i];
			if (rHit.GetComponent<Collider> ().GetComponent<VIDE_Assign> () != null) {
				diagUI.interactToolTipEnabled();
				break;
			}
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (dialogFix) {
			rigidBody.freezeRotation = true;
			return;
		}


		// handle jumping velocity for more responsive jump
		if (rigidBody.velocity.y < 0) {
			rigidBody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
		} else if (rigidBody.velocity.y > 0 && !Input.GetKey(KeyCode.Space)) {
			rigidBody.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
		}
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");
		Animating (h, v);
		Move (h, v);

		// clamp max height jump
		if (!isGrounded && rigidBody.position.y > leftGround + maxHeight && rigidBody.velocity.y > 0) {
			Debug.Log("MAX REACHED");
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().angularVelocity = Vector3.zero; 
			rigidBody.AddForce(0, -3 * forceConst, 0, ForceMode.Impulse);
		}
	}

	// handle bounty notifications
	public void notifyBounty(bool enabled) {
		if (toolTip.enabled != enabled) {
			toolTip.enabled = enabled;
		}
		if (enabled) {
			toolTip.text = "Press 'F' to capture the fugitive!";
		} else {
			toolTip.text = "";
		}
	}

	void Move(float h, float v) {

		// move towards the camera position

		Vector3 movement = new Vector3(h, 0.0f, v);
		movement = Camera.main.transform.TransformDirection(movement);

		if (Input.GetKey (KeyCode.LeftShift)) {
			speed = runSpeed;
		} else {
			speed = walkSpeed;
		}

		movement = movement.normalized * speed * Time.deltaTime;
			
		rigidBody.MovePosition (transform.position + movement);

		if (movement != Vector3.zero) {
			rigidBody.MoveRotation (Quaternion.LookRotation (new Vector3(movement.x, 0.0f, movement.z)));
		}

		//jumping vector generation
		if (Input.GetKey (KeyCode.Space) && isGrounded) {
			rigidBody.AddForce (0, forceConst, 0, ForceMode.Impulse);
			leftGround = rigidBody.position.y;
			isGrounded = false;
		}

	}

	void OnCollisionStay()
	{
		isGrounded = true;
	}

	void Animating (float h, float v) {
		// did we press horizontal axis or vertical axis
		bool walking = false;
		bool running = false;

		if (speed == walkSpeed) {
			walking = h != 0f || v != 0f;
			running = false;
		} else if (speed == runSpeed) {
			running = h != 0f || v != 0f;
			walking = false;
		}

		anim.SetBool ("IsWalking", walking);
		anim.SetBool ("IsRunning", running);
	}

	void toggleJournal() {
		journalEnabled = !journalEnabled;
		if (journalEnabled) {
			diagUI.interfaceOpen ();
			journal.SetActive (true);
		} else {
			diagUI.interfaceClosed ();
			journal.SetActive (false);
		}
	}

	// to talk to NPCs
    void TryInteract()
    {
        if (VD.isActive)
        {
			diagUI.CallNext ();
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, NPC_RANGE);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider rHit = hits[i];
            VIDE_Assign assigned;
            if (rHit.GetComponent<Collider>().GetComponent<VIDE_Assign>() != null)
            {
                assigned = rHit.GetComponent<Collider>().GetComponent<VIDE_Assign>();
                if (!VD.isActive)
                {
                    //... and use it to begin the conversation, look at the target
                    diagUI.Begin(rHit, assigned);
                }
                return;
            }
        }
    }
}
