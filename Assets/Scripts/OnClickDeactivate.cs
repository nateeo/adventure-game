using UnityEngine;
using System.Collections;

public class OnClickDeactivate : MonoBehaviour {

	public GameObject target;

	public void onClick() {
		target.SetActive(false);
	}
}
