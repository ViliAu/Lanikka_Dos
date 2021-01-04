using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : Entity {

    /// <summary>
    /// Get's called when the player interacts with an item
    /// </summary>
    public virtual void PlayerInteract() {

    }

    /// <summary>
    /// Get's called when the player focuses to an item
    /// </summary>
    public virtual void PlayerFocus() {
        EntityManager.Player.Player_UI.ChangeCrosshairDarkness(1f);
    }
}
