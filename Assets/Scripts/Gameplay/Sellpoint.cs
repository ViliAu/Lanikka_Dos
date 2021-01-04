using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sellpoint : Interactable {

    [SerializeField] private Money moneyPrefab = null;
    [SerializeField] private float dropMoneyInterval = 0.2f;
    [SerializeField] private Vector3 moneySpawnOffset = Vector3.up;

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
    }

    private IEnumerator DropMoney(float value) {
        if (moneyPrefab == null) {
            Debug.LogError("No money prefab assigned to "+gameObject.name);
            yield return null;
        }

        while (value > moneyPrefab.amount) {
            Money clone = Instantiate(moneyPrefab, transform.position + moneySpawnOffset, Quaternion.identity, null) as Money;
            value -= moneyPrefab.amount;
            yield return new WaitForSeconds(dropMoneyInterval);
        }
        if (value > 0) {
            Money clonee = Instantiate(moneyPrefab, transform.position + moneySpawnOffset, Quaternion.identity, null) as Money;
            clonee.SetAmount(value);
        }
    }  

}
