using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable {

    public int stackCount = 1;

    public virtual void DestroyItem() {
        Destroy(gameObject);
    }

    public virtual void EnableItem(bool b) {
        gameObject.SetActive(b);
    }

    public override void PlayerInteract() {
        if (!canInteract)
            return;
        base.PlayerInteract();
        EntityManager.Player.Player_Inventory.AddItem(this);
        EntityManager.Player.Player_Equipment.UpdateUI();
    }

}
