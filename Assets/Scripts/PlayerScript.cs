using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class PlayerScript : MonoBehaviour {
	public float speed;

    //Fields for time and score
    public Text timeText;
    private float startTime;
    public int maxPlayTimeInMinutes;
    private float maxTime;
    public Canvas scoreScreen;
    public Slider timeSlider;

	private Rigidbody rigidBody;
	Vector3 movement;

	// Use this for initialization
	void Start () {
        //Code for initializing time and score.
        startTime = Time.time;
        maxTime = maxPlayTimeInMinutes * 60; 
        scoreScreen.enabled = false;
        changeText("Your score is shit!");
        timeSlider.value = 1;

        rigidBody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");
		Move (h, v);

        updateTimeSlider();

        if (Input.GetKeyDown("escape"))
        {
            int score = computeTimeBasedScore();
            changeText("hello, score is: " + score);
            scoreScreen.enabled = !scoreScreen.enabled;
        }
    }

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

    //private function for updating the time and the slider. 
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

    //private helper method for changing text.
    private void changeText (string text)
    {
        Transform child = scoreScreen.transform.Find("ScoreText");
        Text t = child.GetComponent<Text>();
        t.text = text;
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
