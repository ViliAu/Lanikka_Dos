using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : Interactable {

    [SerializeField] private float amount = 100f;

    public override void PlayerInteract() {
        base.PlayerInteract();
        EntityManager.Player.Player_Wallet.AddMoney(amount);
        Destroy(gameObject);
    }

}
