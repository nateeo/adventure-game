using UnityEngine;
using System.Collections;


// methods to manage and track all the dialogues and menu
public class InterfaceManager : MonoBehaviour {

	// load these for each scene to assert control
	public PlayerScript player;
	public CameraThirdPerson camera;

	private int lockCount;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (gameObject);
		lockCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// methods to increase/decrease cursor 'resource' users (to keep it unlocked)
	public void interfaceOpen() {
		lockCount++;
		checkLock ();
	}

	public void interfaceClose() {
		lockCount--;
		checkLock ();
	}

	// check if anything else needs the cursor lock, otherwise change state
	private void checkLock() {
		Debug.Log ("lockCount " + lockCount);
		if (lockCount == 0) {
			lockCursor ();
		} else if (lockCount < 0) {
			Debug.Log ("lockCount less than 0!");
		} else {
			unlockCursor ();
		}
	}

	private void lockCursor() {
		player.dialogFix = false;
		camera.dialogFix = false;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void unlockCursor() {
		player.dialogFix = true;
		camera.dialogFix = true;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;
	}
}
