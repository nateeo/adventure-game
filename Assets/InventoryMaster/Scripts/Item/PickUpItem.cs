using UnityEngine;
using System.Collections;

// this class handles picking up on objects
public class PickUpItem : MonoBehaviour
{
    public Item item;
    private Inventory _inventory;
	private GameObject _playerObj;
	private PlayerScript _player;
	public MemoryScript _memoryScript;

	private bool enabled;

    void Start()
    {
		enabled = false;
		_playerObj = GameObject.FindGameObjectWithTag("Player");
		if (_playerObj != null)
			_player = _playerObj.GetComponent<PlayerScript> ();
            _inventory = _playerObj.GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
    }
		
    // Update is called once per frame
    void Update()
    {
		if (_inventory == null) {
			_inventory = _playerObj.GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
		}
		float distance = Vector3.Distance(this.gameObject.transform.position, _player.transform.position);

		// handle tooltips based on distance
		if (distance <= 3 && item.itemType == ItemType.Bonus) {
			enabled = true;
			_player.notifyBounty (enabled);
		} else {
			if (enabled) {
				enabled = false;
				_player.notifyBounty (enabled);
			}
		}
        if (_inventory != null && Input.GetKeyDown(KeyCode.F))
        {
			if (distance <= 3) {
				if (item.itemType == ItemType.Bonus) {
					//increment the bonus counter
					PlayerScore ps = _player.GetComponent<PlayerScore> ();
					ps.incrementBonus ();
					// remove tooltip
					_player.notifyBounty(false);
					//destory the object once counter has been incremented
					Destroy (this.gameObject);

				} else if (item.itemType == ItemType.Memory) {
					_memoryScript.foundMemory ();
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