using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sellpoint : Interactable {

    [SerializeField] private Money moneyPrefab = null;
    [SerializeField] private float dropMoneyInterval = 0.2f;
    [SerializeField] private Vector3 moneySpawnOffset = Vector3.up;

    private int totalShits = 0;
    private float totalInventoryValue = 0;

    public override void PlayerFocusEnter() {
        base.PlayerInteract();
        CalculateValues();
        EntityManager.Player.Player_UI.ChangeFocusText("Sell "+ totalShits + " doodies for " + totalInventoryValue + "$");
    }

    public override void PlayerFocusExit() {
        base.PlayerFocusExit();
        totalShits = 0;
        totalInventoryValue = 0;
    }

    private void CalculateValues() {
        for (int i = 0; i < EntityManager.Player.Player_Inventory.items.Length; i++) {
            if (EntityManager.Player.Player_Inventory.items[i] == null)
                continue;
            Doodie d = null;
            if ((d = EntityManager.Player.Player_Inventory.items[i].GetComponent<Doodie>()) != null) {
                totalInventoryValue += d.price * EntityManager.Player.Player_Inventory.items[i].stackCount;
                totalShits += EntityManager.Player.Player_Inventory.items[i].stackCount;
            }
        }
    }

    public override void PlayerInteract() {
        base.PlayerInteract();
        for (int i = 0; i < EntityManager.Player.Player_Inventory.items.Length; i++) {
            if (EntityManager.Player.Player_Inventory.items[i] == null)
                continue;
            Doodie d = null;
            if ((d = EntityManager.Player.Player_Inventory.items[i].GetComponent<Doodie>()) != null) {
                totalInventoryValue += d.price * EntityManager.Player.Player_Inventory.items[i].stackCount;
                totalShits += EntityManager.Player.Player_Inventory.items[i].stackCount;
                EntityManager.Player.Player_Inventory.RemoveItemByIndex(i, true);
            }
        }
        if (totalInventoryValue > 0) {
            Invoke("DropMoney", dropMoneyInterval);
        }
    }

    private void DropMoney() {
        float value = totalInventoryValue;
        if (moneyPrefab == null) {
            Debug.LogError("No money prefab assigned to "+gameObject.name);
        }

        Money clone = Instantiate(moneyPrefab, transform.position + moneySpawnOffset, Quaternion.identity, null) as Money;
        value -= moneyPrefab.amount;

        if (value > 0) {
            Invoke("DropMoney", dropMoneyInterval);
        }
    }  
}
