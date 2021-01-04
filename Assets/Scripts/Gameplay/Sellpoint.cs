using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sellpoint : Interactable {

    [SerializeField] private Money moneyPrefab = null;
    [SerializeField] private float dropMoneyInterval = 0.2f;
    [SerializeField] private Vector3 moneySpawnOffset = Vector3.up;

    private void Update() {
        if (EntityManager.Player.Player_Input.dropped) {
            CalculateValues();
        }
    }

    public override void PlayerFocusEnter() {
        base.PlayerFocusEnter();
        CalculateValues();
    }

    private void CalculateValues() {
        int totalShits = 0;
        float totalInventoryValue = 0;
        for (int i = 0; i < EntityManager.Player.Player_Inventory.items.Length; i++) {
            if (EntityManager.Player.Player_Inventory.items[i] == null)
                continue;
            Doodie d = null;
            if ((d = EntityManager.Player.Player_Inventory.items[i].GetComponent<Doodie>()) != null) {
                totalInventoryValue += d.price * EntityManager.Player.Player_Inventory.items[i].stackCount;
                totalShits += EntityManager.Player.Player_Inventory.items[i].stackCount;
            }
        }
        EntityManager.Player.Player_UI.ChangeFocusText("Sell "+ totalShits + " doodies for " + totalInventoryValue + "$");
    }

    public override void PlayerInteract() {
        base.PlayerInteract();
        float totalInventoryValue = 0;
        for (int i = 0; i < EntityManager.Player.Player_Inventory.items.Length; i++) {
            if (EntityManager.Player.Player_Inventory.items[i] == null)
                continue;
            Doodie d = null;
            if ((d = EntityManager.Player.Player_Inventory.items[i].GetComponent<Doodie>()) != null) {
                totalInventoryValue += d.price * EntityManager.Player.Player_Inventory.items[i].stackCount;
                EntityManager.Player.Player_Inventory.RemoveItemByIndex(i, true);
            }
        }
        if (totalInventoryValue > 0) {
            StartCoroutine(DropMoney(totalInventoryValue));
        }
        // ???
        CalculateValues(); // miskei otimi
        EntityManager.Player.Player_UI.ChangeFocusText("Sell 0 doodies for 0$");
    }

    // TODO: RAHA TEMPPINA
    private IEnumerator DropMoney(float value) {
        if (moneyPrefab == null) {
            Debug.LogError("No money prefab assigned to "+gameObject.name);
            yield return null;
        }
        while (value > 22) {
            Money clone = Instantiate(moneyPrefab, transform.position + moneySpawnOffset, Quaternion.identity, null) as Money;
            moneyPrefab.SetAmount(22);
            value -= moneyPrefab.amount;
            yield return new WaitForSecondsRealtime(dropMoneyInterval);
        }

        if (value > 0) {
            Money clone2 = Instantiate(moneyPrefab, transform.position + moneySpawnOffset, Quaternion.identity, null) as Money;
            clone2.SetAmount(value);
        }
    }  
}
