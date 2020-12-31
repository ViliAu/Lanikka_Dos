using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    [Header("Inventory attributes")]
    [SerializeField] private int maxStackSize = 10;
    [SerializeField] public Pickupable[] items; // Tää propertyks joskus

    private void Start() {
        items = new Pickupable[10];
    }

    /// <summary>
    /// Adds picked up item to Player's inventory if there's space.
    /// </summary>
    /// <param name="pick">Picked up item</param>
    public void AddItem(Pickupable pick) {
        // Check if we already have that item => Iterate through items
        foreach (Pickupable p in items) {
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
                    pick.PlayerPickup();
                    return;
                }
            }
        }

        // MIKÄ TÄSSÄ KUSEE
        for (int i = 0; i < items.Length; i++) {
            if (items[i] == null) {
                items[i] = Database.Singleton.GetEntityPrefab(pick.entityName) as Pickupable;
                items[i].stackCount = pick.stackCount;
                pick.PlayerPickup();
                break;
            }
        }
        if (items[0] != null && items[1] != null)
            print(items[0].gameObject.GetInstanceID()+"jerejere"+items[1].gameObject.GetInstanceID());
    }

    public void RemoveItemByIndex(int index) {
        items[index] = null;
    }

    public void RemoveItemByName(string entityName, int amount) {

    }
}
