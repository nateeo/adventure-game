using UnityEngine;
using System.Collections;

public class PersistAudioManager : MonoBehaviour {

	public AudioSource pickup;
	public AudioSource bonusPickUp;
	public AudioSource droppingItem;
	public AudioSource pickUpMemory;
	public AudioSource npcSound;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (gameObject);
	}
	
	// Update is called once per frame
	public void playPickUp() {
		pickup.Play ();
	}

	public void playBonusPickUp() {
		bonusPickUp.Play ();
	}

	public void playDroppingItem() {
		droppingItem.Play ();
	}

	public void playPickUpMemory(){
		pickUpMemory.Play ();
	}

	public void playNPCSound(){
		npcSound.Play ();
	}
}
