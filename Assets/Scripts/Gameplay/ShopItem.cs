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
            Invoke("Restock", restockTime);
            Item clone = Instantiate(itemToGive, transform.position, transform.rotation, null);
            clone.stackCount = amountToGive;
            EntityManager.Player.Player_Inventory.AddItem(clone);
            gameObject.SetActive(false);
        }   
        else {
            SoundSystem.PlaySound2D("ui_negative");
            EntityManager.Player.Player_UI.ChangeFocusText("Not enough money for " + itemToGive.GetReadableEntityName()+"!");
            Invoke("ResetFocusText", 2f);
        }
    }

    public override void PlayerFocusEnter() {
        base.PlayerFocusEnter();
        EntityManager.Player.Player_UI.ChangeCrosshair("crosshair_dollar");
        EntityManager.Player.Player_UI.ChangeFocusText("Buy "+ amountToGive + "x " + itemToGive.GetReadableEntityName() + " " + price + "$");
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
        EntityManager.Player.Player_UI.ChangeFocusText("Buy "+ amountToGive + "x " + itemToGive.GetReadableEntityName() + " " + price + "$");
    }
}
