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
		GameObject.FindGameObjectWithTag ("InterfaceManager").GetComponent<InterfaceManager>().interfaceOpen();
		computer.SetActive (true);
		computerscreen.enabled = true;
		cancel = false;
	}

	void Update(){
		// For cancel
		if (cancel) {
			Debug.Log ("Cancel");
			GameObject.FindGameObjectWithTag ("InterfaceManager").GetComponent<InterfaceManager>().interfaceClose();
			computer.SetActive (false);
			computerscreen.enabled = false;
		}
	}
}
