﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using VIDE_Data;

public class PlayerScript : MonoBehaviour {
	public bool dialogFix = false;
	public float speed;
	public bool isGrounded;
	public Rigidbody rigidBody;
	Vector3 movement;
	public Vector3 jump;
	Animator anim;

	private CharacterController controller;

	private float verticalVelocity;
	private float gravity = 14.0f;
	private float jumpForce = 20.0f;
	private float walkSpeed = 6.0f;
	private float runSpeed = 12.0f;

	  //Fields for time and score

    private float startTime;
    public int maxPlayTimeInMinutes;
    private float maxTime;

	public int maxNumberOfBonuses;
    private int numberOfBonuses;
    //Fields for time and score ends here ===============
	public UIManager diagUI;

	public static float NPC_RANGE = 5f;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
		//jump = new Vector3 (0.0f, 0.2f, 0.0f);
		anim = GetComponent<Animator> ();
		controller = GetComponent<CharacterController> ();

		//Code for initializing time and score.
        startTime = Time.time;
        maxTime = maxPlayTimeInMinutes * 60;

        rigidBody = GetComponent<Rigidbody> ();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.P))
		{
			incrementBonus();
		}

		if (Input.GetKeyDown(KeyCode.O))
		{
			decrementBonus();
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			endSceneAndDisplayScore();
		}
		if (Input.GetKeyDown (KeyCode.F)) {
			TryInteract ();
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

	void TryInteract()
	{
		if (VD.isActive) {
			VD.Next ();
			return;
		}

				Collider[] hits = Physics.OverlapSphere (transform.position, NPC_RANGE);
		for (int i = 0; i < hits.Length; i++) {
			Debug.Log ("Nanichan");
			Collider rHit = hits [i];
			VIDE_Assign assigned;
			if (rHit.GetComponent<Collider>().GetComponent<VIDE_Assign> () != null) {
				assigned = rHit.GetComponent<Collider>().GetComponent<VIDE_Assign> ();
				if (!VD.isActive) {
					//... and use it to begin the conversation, look at the target
					Debug.logger.Log ("BEGIN");
					diagUI.Begin (rHit, assigned);
				}
				return;
			}
		}
	}

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
//	}
	//Method for computing the score based on a maximum time.
    //The policy is a 3 section idea: 0/1/2/3 stars.
    int computeTimeBasedScore()
    {
        float timeEllapsed = Time.time - startTime;
        float division = maxTime / 3;
        if (timeEllapsed < division)
        {
            return 3;
        }
        else if (timeEllapsed < division * 2)
        {
            return 2;
        } else if (timeEllapsed < division * 3)
        {
            return 1;
        }
        return 0;
    }

    //Method for computing the score based on a number of bonuses picked up.
    //The policy is a 3 section idea: 0/1/2/3 stars.
    int computeBonusBasedScore()
    {
        if (numberOfBonuses >= maxNumberOfBonuses)
        {
            return 3;
        }
        else if (numberOfBonuses >= maxNumberOfBonuses / 2.0)
        {
            return 2;
        }
        else if (numberOfBonuses > 0)
        {
            return 1;
        }
        return 0;
    }

    //Use this method to display the score.
    //This will switch to scene number 2 (the score screen)
    private void endSceneAndDisplayScore()
    {
        int timeScore = computeTimeBasedScore();
        int bonusScore = computeBonusBasedScore();

        PlayerPrefs.SetInt("TimeScore", timeScore);
        PlayerPrefs.SetInt("BonusScore", bonusScore);

        SceneManager.LoadScene(2);
    }

    //private function for updating the time and the slider.
    /* TIMER REMOVED (code might be useful some time, so has been left in here!)
     *
     *
    private void updateTimeSlider()
    {
        // This section is to do with displaying the time.
        float timeinSec = Time.time - startTime;
        int minutes = ((int)timeinSec / 60);
        int seconds = (int)(timeinSec % 60);

        string minToDisplay = minutes.ToString("00");
        string secToDisplay = seconds.ToString("00");

        timeText.text = minToDisplay + ":" + secToDisplay;

        float proportion = timeinSec / maxTime;
        if (proportion > 1) { proportion = 1; }

        float proportionRemaining = 1 - proportion;
        timeSlider.value = proportionRemaining;

        var fill = (timeSlider as UnityEngine.UI.Slider).GetComponentsInChildren<UnityEngine.UI.Image>().FirstOrDefault(t => t.name == "Fill");
        if (fill != null)
        {
            fill.color = Color.Lerp(Color.red, Color.green, proportionRemaining);
        }
    }
    */



    //Use this method when a bonus object has been picked up
    public void incrementBonus()
    {
        numberOfBonuses++;
		Debug.Log ("bonus" + numberOfBonuses);
    }
    //Use this method when you want to deduct points
    public void decrementBonus()
    {
        numberOfBonuses--;
    }


	void Move(float h, float v) {

		Vector3 movement = new Vector3(h, 0.0f, v);

		movement = Camera.main.transform.TransformDirection(movement);

		//make sure player always moves in same speed no matter what combination of keys
		//this is called every with every FixedUpdate- dont want it to move 6 units every fixed update
		//want to change it so that it is per second- multiple it by delta time. delta time is the time between each update call
		//so if youre updating every 50th of a second, over the course of 50 50th of a second its going to move 6 units
		if (Input.GetKey (KeyCode.LeftShift)) {
			speed = runSpeed;
		} else {
			speed = walkSpeed;
		}

		movement = movement.normalized * speed * Time.deltaTime;
		rigidBody.MovePosition (transform.position + movement);
		rigidBody.transform.rotation = Quaternion.LookRotation (new Vector3(movement.x, 0, movement.z));



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
}
