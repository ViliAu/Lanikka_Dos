using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable {
   
   private bool open = false;
   private bool interactable = true;

    public override void PlayerInteract() {
        if (!interactable) {
            return;
        }
        base.PlayerInteract();
        open = !open;
        PlayAnimation();
    }

    private void PlayAnimation() {
        
    }


}
