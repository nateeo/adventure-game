using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
	public float speed;
	public bool isGrounded;
	private Rigidbody rigidBody;
	Vector3 movement;
	public Vector3 jump;
	private float jumpForce = 25.0f;
	Animator anim;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
		jump = new Vector3 (0.0f, 0.2f, 0.0f);
		anim = GetComponent<Animator> ();
	}

	void OnCollisionStay() {
		isGrounded = true;
	}

	// Update is called once per frame
	void FixedUpdate () {
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");
		Animating (h, v);
		Move (h, v);

	}

	void Update() {
		if(Input.GetKeyDown(KeyCode.Space) && isGrounded){

			rigidBody.AddForce(jump * jumpForce, ForceMode.Impulse);
			isGrounded = false;
		}
	}


	void Move(float h, float v) {

		Vector3 movement = new Vector3(h, 0.0f, v);
		movement = Camera.main.transform.TransformDirection(movement);
		//make sure player always moves in same speed no matter what combination of keys
		//this is called every with every FixedUpdate- dont want it to move 6 units every fixed update
		//want to change it so that it is per second- multiple it by delta time. delta time is the time between each update call
		//so if youre updating every 50th of a second, over the course of 50 50th of a second its going to move 6 units
		movement = movement.normalized * speed * Time.deltaTime;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.20F);
		rigidBody.MovePosition (transform.position + movement);

	

	}

	void Animating (float h, float v) {
		// did we press horizontal axis or vertical axis
		bool walking = h != 0f || v != 0f;
		anim.SetBool ("IsWalking", walking);
	}
}
