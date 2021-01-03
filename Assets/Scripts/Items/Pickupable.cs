using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : Entity {

    public int stackCount = 1;

    public virtual void DestroyItem() {
        Destroy(gameObject);
    }

    public virtual void EnableItem(bool b) {
        gameObject.SetActive(b);
    }

}
