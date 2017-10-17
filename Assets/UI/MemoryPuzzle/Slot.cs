using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

/*This script should be attached to a slot. 
* This will allow a slot to receive an UI element through dropping.
* 
* Author: Victor
*/
public class Slot : MonoBehaviour, IDropHandler{
    public GameObject item
    {
        get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
    }

    #region IDropHandler implementation
    public void OnDrop(PointerEventData eventData)
    {
        if (!item) //If a slot is empty, accept the item.
        {
            DragHandler.itemBeingDragged.transform.SetParent(transform);
        } else //If a slot is occupied, swap item.
        {
            item.transform.SetParent(DragHandler.itemBeingDragged.transform.parent);
            DragHandler.itemBeingDragged.transform.SetParent(transform);
        }
    }
    #endregion
}
