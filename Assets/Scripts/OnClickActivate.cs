using UnityEngine;
using System.Collections;

public class OnClickActivate : MonoBehaviour {
	public GameObject target;

	public void onClick() {
		target.SetActive (true);
	}
}
