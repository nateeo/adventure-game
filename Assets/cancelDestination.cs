using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class cancelDestination : MonoBehaviour {

	public bool cancel = false;
	public GameObject trigger;

	public void onCancel(){
		cancel = true;
		ComputerTrigger trig = trigger.GetComponent<ComputerTrigger> ();
		trig.cancel = cancel;
	}
}
