using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    [Header("Inventory attributes")]
    public int maxStackSize = 10;
    public Item[] items; // Tää propertyks joskus

    private void Start() {
        items = new Item[10];
    }

    /// <summary>
    /// Adds picked up item to Player's inventory if there's space.
    /// </summary>
    /// <param name="pick">Picked up item</param>
    public bool AddItem(Item pick) {
        // Check if we already have that item => Iterate through items
        foreach (Item p in items) {
            if (p == null) {
                continue;
            }
            if (p.entityName == pick.entityName) {
                // The inv item's stack count is at max => continue loop
                if (p.stackCount == maxStackSize) {
                    continue;
                }
                // There's already that item in inv and we have SOME space to pickup => Increment stack size and reduce it from the obj
                else if (p.stackCount + pick.stackCount > maxStackSize) {
                    pick.stackCount -= (maxStackSize - p.stackCount);
                    p.stackCount = maxStackSize;
                    continue;
                }
                // There's already that item in inv and we have space to pickup => Increment stack size and destroy gobj.
                else if (p.stackCount + pick.stackCount <= maxStackSize) {
                    p.stackCount += pick.stackCount;
                    pick.DestroyItem();
                    return true;
                }
            }
        }

        // Try to add the item to an empty slot
        for (int i = 0; i < items.Length; i++) {
            if (items[i] == null) {
                items[i] = pick;
                items[i].stackCount = pick.stackCount;
                pick.EnableItem(false);
                return true;
            }
        }
        return false;
    }

    public void RemoveItemByIndex(int index, bool destroy) {
        if (destroy)
            items[index].DestroyItem();
        if (EntityManager.Player.Player_Equipment.itemIndex == index)
            EntityManager.Player.Player_Equipment.RemoveEquippedItem();
        items[index] = null;
    }

    public void DecrementStackSize(int index) {
        if (items[index] == null) {
            return;
        }
        items[index].stackCount--;
        if (items[index].stackCount == 0) {
            RemoveItemByIndex(index, true);
        }
        EntityManager.Player.Player_Equipment.UpdateUI();
    }
}
