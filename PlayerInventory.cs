using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    //inventory slots of the player 1-3
    public List<GameObject> inventoryItems = new List<GameObject>();
    public int currentSlot;
    public int emptySlot;


    public void SetActiveItem(int slot) {
        InventoryUIManager.Instance.SetActiveSlot(slot);
        currentSlot = slot;
        for (int i = 0; i < inventoryItems.Count; i++) {
            inventoryItems[i].SetActive(false);
        }
        inventoryItems[slot].SetActive(true);
    }

    public void PutInInventory(GameObject item) {

        if (inventoryItems[currentSlot].GetComponent<InventorySlotInfo>().inUse) {
            ReplaceItem(item);
        }
        else {
            Debug.Log("Going in else inventory slot is not in use");
            CheckSlots(item);
        }
    }

    void ReplaceItem(GameObject item) {
        inventoryItems[currentSlot].transform.GetChild(0).GetComponent<ItemInfo>().DropItem();
        inventoryItems[currentSlot].transform.GetChild(0).parent = null;
        SetParent(item);
    }

    void CheckSlots(GameObject item) {
        for (int i = 0; i < inventoryItems.Count; i++) {
            if (!inventoryItems[i].GetComponent<InventorySlotInfo>().inUse) {
                inventoryItems[i].GetComponent<InventorySlotInfo>().inUse = true;
                emptySlot = i;
                break;
            }
        }

        SetParent(item);
    }

    void SetParent(GameObject child) {
        child.transform.SetParent(inventoryItems[currentSlot].transform, false);
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
    }
}
