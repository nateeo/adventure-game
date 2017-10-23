using UnityEngine;
using System.Collections;

public class RandomGenBonus : MonoBehaviour {
	//number of items to generate
	public int noItems = 1;
	public float xmin = 211;
	public float xmax = 250;
	public float zmin = -120;
	public float zmax = -85;

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
			if (items.itemList [rand].itemType == ItemType.Bonus) {
				counter++;

				Terrain terrain = Terrain.activeTerrain;

				//pick random coodinates within the area
				float x = Random.Range(xmin, xmax);
				float z = Random.Range (zmin, zmax	);

				Debug.Log ("coord" + x + ", " + z);

				GameObject randomItem = (GameObject)Instantiate (items.itemList [rand].itemModel);

				//so that the item will fall and land on the surface of the terrain
				Rigidbody rb = randomItem.AddComponent<Rigidbody>();
				rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
				rb.useGravity = true;
				rb.detectCollisions = true;

				PickUpItem pui = randomItem.AddComponent<PickUpItem> ();

				if (pui != null) {
					pui.item = items.itemList [rand];
				}


				//put the item at a random position
				randomItem.transform.localPosition = new Vector3(x, 15, z);
			}
		}
	}
}
