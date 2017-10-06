using UnityEngine;
using System.Collections;
using VIDE_Data;

public class PlayerScript : MonoBehaviour {
	public bool dialogFix = false;
	public float speed;
	private Rigidbody rigidBody;
	Vector3 movement;
	public UIManager diagUI;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.F)) {
			TryInteract ();
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
		Move (h, v);
	}

	void TryInteract()
	{
		if (VD.isActive) {
			VD.Next ();
			return;
		}

		Collider[] hits = Physics.OverlapSphere (transform.position, 5);
		for (int i = 0; i < hits.Length; i++) {
			Collider rHit = hits [i];
			VIDE_Assign assigned;
			if (rHit.GetComponent<Collider>().GetComponent<VIDE_Assign> () != null) {
				assigned = rHit.GetComponent<Collider>().GetComponent<VIDE_Assign> ();
				if (!VD.isActive) {
					//... and use it to begin the conversation, look at the target
					Debug.logger.Log ("BEGIN");
					diagUI.Begin (assigned);
				}
				return;
			}
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
		rigidBody.MovePosition (transform.position + movement);
	}
}
