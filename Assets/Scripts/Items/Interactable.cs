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
    public virtual void PlayerFocusEnter() {
        EntityManager.Player.Player_UI.ChangeCrosshairDarkness(1f);
    }

    /// <summary>
    /// Get's called when the player loses focus to an item
    /// </summary>
    public virtual void PlayerFocusExit() {
        EntityManager.Player.Player_UI.ChangeCrosshairDarkness(0.75f);
        EntityManager.Player.Player_UI.ChangeFocusText("");
    }
}
