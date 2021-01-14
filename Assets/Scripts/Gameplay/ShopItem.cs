using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : Interactable {
    [SerializeField] private Interactable itemToGive = null;
    [SerializeField] private int amountToGive = 1;
    [SerializeField] private float price = 1f;
    [SerializeField] private float restockTime = 60f;

    public override void PlayerInteract() {
        base.PlayerInteract();
        if (EntityManager.Player.Player_Wallet.RemoveMoney(price)) {
            Interactable clone = Instantiate(itemToGive, transform.position, transform.rotation, null);
            if (clone as Item) {
                Item item = (Item)clone;
                item.stackCount = amountToGive;
                EntityManager.Player.Player_Inventory.AddItem(item);
            }
            if (restockTime > 0) {
                gameObject.SetActive(false);
                Invoke("Restock", restockTime);
            }
            else {
                Destroy(gameObject);
                EntityManager.Player.Player_UI.SetFocusText("");
                EntityManager.Player.Player_UI.SetCrosshair("crosshair_dot");
            }
        }   
        else {
            SoundSystem.PlaySound2D("ui_negative");
            EntityManager.Player.Player_UI.SetFocusText("Not enough money for " + itemToGive.GetReadableEntityName()+"!");
            Invoke("ResetFocusText", 2f);
        }
    }

    public override void PlayerFocusEnter() {
        base.PlayerFocusEnter();
        EntityManager.Player.Player_UI.SetCrosshair("crosshair_dollar");
        EntityManager.Player.Player_UI.SetFocusText("Buy " + (itemToGive as Item ? amountToGive + "x " : "a ") + itemToGive.GetReadableEntityName() + " " + price + "$");

    }

    public override void PlayerFocusExit() {
        base.PlayerFocusExit();
        CancelInvoke("ResetFocusText");
    }

    // Tää kutsutaa sit kaupas ku tulee lentokonetäydennyksii
    public void Restock() {
        gameObject.SetActive(true);
    }

    private void ResetFocusText() {
        EntityManager.Player.Player_UI.SetFocusText("Buy " + (itemToGive as Item ? amountToGive + "x " : "a ") + itemToGive.GetReadableEntityName() + " " + price + "$");
    }
}
