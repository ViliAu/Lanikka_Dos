using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : Interactable {

    [Header("Money settings")]
    public float amount = 100f;
    [Tooltip("Thresholds for the money models (max.)")]
    public float[] modelThresholds;

    // Activate the right money model
    protected override void Awake() {
        base.Awake();
        bool activated = false;
        for (int i = 0; i < modelThresholds.Length; i++) {
            if (amount <= modelThresholds[i] && !activated) {
                transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
                activated = true;
            }
            else {
                transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
            }
        }    
    }

    public override void PlayerInteract() {
        base.PlayerInteract();
        EntityManager.Player.Player_Wallet.AddMoney(amount);
        Destroy(gameObject);
    }

    public void SetAmount(float a) {
        amount = a;
        bool activated = false;
        for (int i = 0; i < modelThresholds.Length; i++) {
            if (amount <= modelThresholds[i] && !activated) {
                transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
                activated = true;
            }
            else {
                transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
            }
        } 
    }
}
