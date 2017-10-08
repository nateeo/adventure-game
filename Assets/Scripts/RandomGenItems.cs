using UnityEngine;
using System.Collections;

public class RandomGenItems : MonoBehaviour {
	//number of items to generate
	public int noItems = 2;

	//get the list of possible items to generate
	static ItemDataBaseList items;

	//up to the number of items wanted
	int counter = 0;

	void Start () {
		//loead all the items that may be used
		items = (ItemDataBaseList)Resources.Load("Itemdatabase");

		while (counter < noItems) {
			//randomly pick the items	
			int rand = Random.Range (1, items.itemList.Count - 1);

			//add to the scene
			if (items.itemList [rand].itemType == ItemType.Memory) {
				counter++;

				Terrain terrain = Terrain.activeTerrain;

				//pick random coodinates within the area
				float x = Random.Range(55, 80);
				float z = Random.Range (-50, -140);

				Debug.Log ("coord" + x + ", " + z);

				GameObject randomItem = (GameObject)Instantiate (items.itemList [rand].itemModel);

				//so that the item will fall and land on the surface of the terrain
				Rigidbody rb = randomItem.AddComponent<Rigidbody>();
				rb.useGravity = true;
				rb.detectCollisions = true;
				BoxCollider bc = randomItem.AddComponent<BoxCollider> ();

				//put the item at a random position
				randomItem.transform.localPosition = new Vector3(x, 15, z);
			}
		}
	}
}
