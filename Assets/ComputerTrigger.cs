using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ComputerTrigger : MonoBehaviour {

	public CameraThirdPerson playerCamera;
	public PlayerScript playerController;
	public GameObject computer;
	private InitialSetActive screen;
	public Image computerscreen;
	public bool cancel = false;
	


	void Start(){
		screen = computer.GetComponent<InitialSetActive> ();
	}

	void OnTriggerEnter(Collider collider) {
		playerController.dialogFix = true;
		playerCamera.dialogFix = true;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;
		computer.SetActive (true);
		computerscreen.enabled = true;
		cancel = false;

	}

	void Update(){
		// For cancel
		if (cancel) {
			Debug.Log ("Cancel");
			playerController.dialogFix = false;
			playerCamera.dialogFix = false;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			computer.SetActive (false);
			computerscreen.enabled = false;
		}
	}
}
