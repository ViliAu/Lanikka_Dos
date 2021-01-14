using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DookerCage : Interactable {

    public override void PlayerInteract() {
        base.PlayerInteract();
        if (EntityManager.DookerPen.IsInside(transform.position)) {
            Dooker duu = Instantiate<Dooker>(Database.Singleton.GetEntityPrefab("dooker") as Dooker, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        else {
            SoundSystem.PlaySound2D("ui_negative");
        }
    }

}
