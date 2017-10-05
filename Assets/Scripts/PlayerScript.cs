using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
	public float speed;
	public float turnSmoothing = 15f; // A smoothing value for turning the player.
	private Rigidbody rigidBody;
	Vector3 movement;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");

		Move (h, v);
	
	}


	void Move(float h, float v) {

		Vector3 movement = new Vector3(h, 0.0f, v);
		movement = Camera.main.transform.TransformDirection(movement);
		//make sure player always moves in same speed no matter what combination of keys
		//this is called every with every FixedUpdate- dont want it to move 6 units every fixed update
		//want to change it so that it is per second- multiple it by delta time. delta time is the time between each update call
		//so if youre updating every 50th of a second, over the course of 50 50th of a second its going to move 6 units
		movement = movement.normalized * speed * Time.deltaTime;
		rigidBody.MovePosition (transform.position + movement);
	}
}
