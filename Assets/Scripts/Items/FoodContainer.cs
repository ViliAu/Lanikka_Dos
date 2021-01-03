using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodContainer : Interactable {

    [Header("Container data")]
    public List<Edible> edibles = new List<Edible>();
    public int maxFoodCount = 10;

    [Tooltip("How far the dookers can be when eating")]
    public float dookerDistance = 1f;

    public override void PlayerInteract() {
        base.PlayerInteract();
        if (EntityManager.Player.Player_Equipment.equippedItem != null) {
            if (EntityManager.Player.Player_Equipment.equippedItem as Edible) {
                if (edibles.Count < maxFoodCount) {
                    edibles.Add((Edible)EntityManager.Player.Player_Equipment.equippedItem);
                    EntityManager.Player.Player_Inventory.DecrementStackSize(EntityManager.Player.Player_Equipment.itemIndex);
                }
            }
        }
    }

}
