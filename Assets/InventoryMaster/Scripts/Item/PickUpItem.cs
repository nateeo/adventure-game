using UnityEngine;
using System.Collections;
public class PickUpItem : MonoBehaviour
{
    public Item item;
    private Inventory _inventory;
    private GameObject _player;
    // Use this for initialization
	public MemoryScript _memoryScript;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player != null)
            _inventory = _player.GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
    }
		
    // Update is called once per frame
    void Update()
    {
        if (_inventory != null && Input.GetKeyDown(KeyCode.F))
        {
            float distance = Vector3.Distance(this.gameObject.transform.position, _player.transform.position);
            if (distance <= 3)
            {
				if (item.itemType == ItemType.Bonus) {
					PlayerScript ps = _player.GetComponent<PlayerScript> ();
					ps.incrementBonus ();
					Destroy (this.gameObject);
				} else if (item.itemType == ItemType.Memory) {
					_memoryScript.foundMemory();
					Destroy (this.gameObject);
				} else {
					bool check = _inventory.checkIfItemAllreadyExist (item.itemID, item.itemValue);
					if (check)
						Destroy (this.gameObject);
					else if (_inventory.ItemsInInventory.Count < (_inventory.width * _inventory.height)) {
						_inventory.addItemToInventory (item.itemID, item.itemValue);
						_inventory.updateItemList ();
						_inventory.stackableSettings ();
						Destroy (this.gameObject);
					}
				}

            }
        }
    }

}