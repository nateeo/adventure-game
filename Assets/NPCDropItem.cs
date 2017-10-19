using UnityEngine;
using System.Collections;

public class NPCDropItem : MonoBehaviour
{

	public GameObject item;
	public int keyPressTrigger;
	private int keyPressed = 0;

	void Start()
	{
		item.SetActive(false);
	}

	public void Update()
	{
		// Check if F has been pressed, only if the item hasn't been picked up and is still hidden
		if (item != null && !item.activeSelf && Input.GetKeyDown(KeyCode.F) == true)
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