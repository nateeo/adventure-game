using UnityEngine;
using System.Collections;

public class GameObjectiveController : MonoBehaviour {
	public GameObjective shack;
	public GameObjective mine;
	public GameObjective oasis;
	public GameObjective tower;
	public GameObjective ship;

	private GameObjective[] markers = new GameObjective[5];
	private int markerValue = 0;

	// Use this for initialization
	void Start () {
		markers [0] = shack;
		markers [1] = mine;
		markers [2] = oasis;
		markers [3] = tower;
		markers [4] = ship;

		//Disable all markers except for the first one
		for (int i = 1; i < 5; i++) {
			markers [i].gameObject.SetActive (false);
		}
	}

	public void disable(GameObjective objective) {
		for (int i = 0; i < 5; i++) {
			if (objective.Equals(markers[i])) { //Disable the objective and enable the next one
				Debug.Log ("Disable " + i);
				markers [i].gameObject.SetActive (false);
				markers [i + 1].gameObject.SetActive (true);

				return;
			}
		}
	}
}
