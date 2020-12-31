using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractCube : Interactable {

    public override void PlayerInteract() {
        base.PlayerInteract();
        Debug.Log("Moro ukko");
    }

}
