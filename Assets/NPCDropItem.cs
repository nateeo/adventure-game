using UnityEngine;
using System.Collections;

public class NPCDropItem : MonoBehaviour
{

	public GameObject item;
	public int keyPressTrigger;
	private int keyPressed = 0;
	private GameObject _player;
	void Start()
	{
		_player = GameObject.FindGameObjectWithTag("Player");
		item.SetActive(false);
	}

	public void Update()
	{
		// Check if F has been pressed, only if the item hasn't been picked up and is still hidden and user is within range
		if (Input.GetKeyDown(KeyCode.F) && Vector3.Distance(this.transform.position, _player.transform.position) <= 2.2f && item != null && !item.activeSelf)
		{
			keyPressed++;
			// Check if the conversation between the user and NPC is finished
			if (keyPressed == keyPressTrigger)
			{
				showItem();
			}
		}
	}

	public void showItem()
	{
		item.SetActive(true);
	}
}