using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    [Header("Inventory attributes")]
    public int maxStackSize = 10;
    public Item[] items; // Tää propertyks joskus

    private void Start() {
        items = new Item[10];
    }

    /// <summary>
    /// Adds picked up item to Player's inventory if there's space.
    /// </summary>
    /// <param name="item">Picked up item</param>
    public virtual bool AddItem(Item item) {
        // Check if we already have that item => Iterate through items
        foreach (Item p in items) {
            if (p == null) {
                continue;
            }
            if (p.entityName == item.entityName) {
                // The inv item's stack count is at max => continue loop
                if (p.stackCount == maxStackSize) {
                    continue;
                }
                // There's already that item in inv and we have SOME space to pickup => Increment stack size and reduce it from the obj
                else if (p.stackCount + item.stackCount > maxStackSize) {
                    item.stackCount -= (maxStackSize - p.stackCount);
                    p.stackCount = maxStackSize;
                    continue;
                }
                // There's already that item in inv and we have space to pickup => Increment stack size and destroy gobj.
                else if (p.stackCount + item.stackCount <= maxStackSize) {
                    p.stackCount += item.stackCount;
                    item.DestroyItem();
                    return true;
                }
            }
        }

        // Try to add the item to an empty slot
        for (int i = 0; i < items.Length; i++) {
            if (items[i] == null) {
                items[i] = item;
                items[i].stackCount = item.stackCount;
                item.EnableItem(false);
                return true;
            }
        }
        return false;
    }

    public virtual void RemoveItemByIndex(int index, bool destroy) {
        if (destroy)
            items[index].DestroyItem();
        items[index] = null;
    }

    public virtual void DecrementStackSize(int index) {
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
