using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallet : MonoBehaviour {

    private float money = 0f;

    public void AddMoney(float amount) {
        money += amount;
        UpdateUI();
    }


    /// <summary>
    /// Used to remove money from the player by a certain amount
    /// </summary>
    /// <param name="amount">Amount to deduct from the player</param>
    /// <returns>Returns wether or not the player had enough money</returns>
    public bool RemoveMoney(float amount) {
        if (amount > money) { 
            return false;
        }
        else {
            money -= amount;
            UpdateUI();
            return true;
        }
    }

    private void UpdateUI() {
        EntityManager.Player.Player_UI.ChangeMoneyAmount(money);
    }
}
