using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : Interactable {
    [SerializeField] private Item itemToGive = null;
    [SerializeField] private int amountToGive = 1;
    [SerializeField] private float price = 1f;
    [SerializeField] private float restockTime = 60f;

    public override void PlayerInteract() {
        base.PlayerInteract();
        if (EntityManager.Player.Player_Wallet.RemoveMoney(price)) {
            gameObject.SetActive(false);
            Invoke("Restock", restockTime);
            for(int i = 0; i < amountToGive; i++)
                EntityManager.Player.Player_Inventory.AddItem(itemToGive);
        }   
        else {
            SoundSystem.PlaySound2D("ui_negative");
        }
    }

    public override void PlayerFocus() {
        base.PlayerFocus();
        EntityManager.Player.Player_UI.ChangeCrosshair("crosshair_dollar");
        EntityManager.Player.Player_UI.ChangeFocusText("Buy "+ amountToGive + "x " + itemToGive.entityName + " " + price + "$");
    }

    // Tää kutsutaa sit kaupas ku tulee lentokonetäydennyksii
    public void Restock() {
        gameObject.SetActive(true);
    }
}
