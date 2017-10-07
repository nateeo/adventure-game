using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
	public float speed;
	public bool isGrounded;
	private Rigidbody rigidBody;
	Vector3 movement;
	public Vector3 jump;
	Animator anim;

	private CharacterController controller;

	private float verticalVelocity;
	private float gravity = 14.0f;
	private float jumpForce = 20.0f;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
		//jump = new Vector3 (0.0f, 0.2f, 0.0f);
		anim = GetComponent<Animator> ();
		controller = GetComponent<CharacterController> ();
	}


	// Update is called once per frame
	void FixedUpdate () {
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");
		Animating (h, v);
		Move (h, v);

//		if (controller.isGrounded) {
//			verticalVelocity = -gravity * Time.deltaTime;
//			if (Input.GetKeyDown (KeyCode.Space)) {
//				verticalVelocity = jumpForce;
//			}
//		} else {
//			verticalVelocity -= gravity * Time.deltaTime;
//		}
//
//		Vector3 moveVector = new Vector3 (0, verticalVelocity, 0);
//		controller.Move (moveVector * Time.deltaTime);

	}

	void Update() {
		
	}


	void Move(float h, float v) {

		Vector3 movement = new Vector3(h, 0.0f, v);

		movement = Camera.main.transform.TransformDirection(movement);

		//make sure player always moves in same speed no matter what combination of keys
		//this is called every with every FixedUpdate- dont want it to move 6 units every fixed update
		//want to change it so that it is per second- multiple it by delta time. delta time is the time between each update call
		//so if youre updating every 50th of a second, over the course of 50 50th of a second its going to move 6 units
		if (Input.GetKey (KeyCode.LeftShift)) {
			speed = 8f;
		} else {
			speed = 4f;
		}

		movement = movement.normalized * speed * Time.deltaTime;

		rigidBody.MovePosition (transform.position + movement);
		rigidBody.transform.rotation = Quaternion.LookRotation (new Vector3(movement.x, 0, movement.z));



	}

	void Animating (float h, float v) {
		// did we press horizontal axis or vertical axis
		bool walking = false;
		bool running = false;

		if (speed == 4f) {
			walking = h != 0f || v != 0f;
			running = false;
		} else if (speed == 8f) {
			running = h != 0f || v != 0f;
			walking = false;
		}

		anim.SetBool ("IsWalking", walking);
		anim.SetBool ("IsRunning", running);
	}
}
