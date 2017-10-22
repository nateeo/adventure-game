using UnityEngine;
using System.Collections;
using VIDE_Data;

/**
 * This class is the Controller for the GameObjectives.
 * It turns the Objective markers on and off in an order as the player arrives at the objective.
 */
public class GameObjectiveController : MonoBehaviour {
	public GameObjective shack;
	public GameObjective mine;
	public GameObjective oasis;
	public GameObjective tower;
	public GameObjective ship;

	public UIManager diagUI;

	private GameObjective[] markers = new GameObjective[5];
	private int markerValue = 0;

	// Use this for initialization
	void Start () {
		markers [0] = oasis;
		markers [1] = mine;
		markers [2] = shack;
		markers [3] = tower;
		markers [4] = ship;

		//Disable all markers except for the first one
		for (int i = 0; i < 5; i++) {
			markers [i].gameObject.SetActive (false);
		}

		markers [2].gameObject.SetActive (true);
	}

	//This is called when the player enters the triggered area
	public void disable(GameObjective objective) {
		for (int i = 0; i < 5; i++) {
			if (objective.Equals(markers[i])) { //Disable the objective and enable the next one
				Debug.Log ("Disable " + i);
				markers [i].gameObject.SetActive (false);
				markers [i + 1].gameObject.SetActive (true);

				VIDE_Assign assigned = objective.GetComponent<VIDE_Assign> ();
				if (!VD.isActive) {
					diagUI.Begin (objective.GetComponent<Collider>(), assigned);
				}

				if (i == 2) {
					activateAllWaypoints ();
				}

				return;
			}
		}
	}

	void activateAllWaypoints() {
		for (int i = 0; i < 5; i++) {
			if (i != 2) { //Do not activate the town
				markers [i].gameObject.SetActive (true);
			}
		}
	}
}
