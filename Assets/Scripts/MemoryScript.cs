using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MemoryScript : MonoBehaviour {

	private int count;
	private int max_count;
	public Text memory;

	// Use this for initialization
	void Start () {
		count = 0;
		max_count = 6;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void setMemoryCount() {
		memory.text = "Memories collected: " + count.ToString() + "/?";
		if (count >= 6) {
//			memory.color = Color.green;
		}
	}

	public void foundMemory(){
		count++;
		setMemoryCount();
	}
}
