using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : Inventory {

    public Inventory connectedInventory = null;

    public override void RemoveItemByIndex(int index, bool destroy) {
        base.RemoveItemByIndex(index, destroy);
        if (EntityManager.Player.Player_Equipment.itemIndex == index)
            EntityManager.Player.Player_Equipment.RemoveEquippedItem();
    }

}
