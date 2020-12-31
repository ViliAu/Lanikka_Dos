using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : Item {

    public int stackCount = 1;

    public virtual void PlayerPickup() {
        Destroy(gameObject);
    }

}
