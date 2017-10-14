using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using VIDE_Data;

public class PlayerScript : MonoBehaviour {
	public bool dialogFix = false;
	public float speed;
	private bool isGrounded;
	public Rigidbody rigidBody;
	Vector3 movement;
	public Vector3 jump;
	Animator anim;
	public int forceConst;

	private CharacterController controller;

	private float verticalVelocity;
	private float gravity = 14.0f;
	private float jumpForce = 20.0f;
	private float walkSpeed = 4.0f;
	private float runSpeed = 10.0f;

	public UIManager diagUI;

	public static float NPC_RANGE = 2f;

	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
		rigidBody = GetComponent<Rigidbody> ();
		//jump = new Vector3 (0.0f, 0.2f, 0.0f);
		anim = GetComponent<Animator> ();
		controller = GetComponent<CharacterController> ();
		rigidBody = GetComponent<Rigidbody> ();
	}

	void Awake() {
		DontDestroyOnLoad (transform.gameObject);
	}

	void Update() {
		
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
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");
		Animating (h, v);
		Move (h, v);

	}

	void Move(float h, float v) {

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

    void TryInteract()
    {
        if (VD.isActive)
        {
            VD.Next();
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
