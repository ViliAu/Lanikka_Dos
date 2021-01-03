using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

    [SerializeField] private float range = 3f;
    [SerializeField] private LayerMask interactionMask = default;

    private Camera cam;
    private Ray ray;

    private void Awake() {
        cam = Camera.main;
    }

    private void Update() {
        CastInteractRay();
    }

    private void CastInteractRay() {
        ray = cam.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, range, interactionMask, QueryTriggerInteraction.Ignore)) {
            Interactable intera = null;
            Pickupable pick = null;
            if ((intera = hit.transform.GetComponent<Interactable>()) != null) {
                EntityManager.Player.Player_UI.ChangeCrosshairDarkness(1f);
                if (EntityManager.Player.Player_Input.interacted)
                    intera.PlayerInteract();
                return;
            }
            else if ((pick = hit.transform.GetComponent<Pickupable>()) != null) {
                EntityManager.Player.Player_UI.ChangeCrosshairDarkness(1f);
                if (EntityManager.Player.Player_Input.interacted)
                    EntityManager.Player.Player_Inventory.AddItem(pick);
                return;
            }
            else {
                
            }
        }
        else {
            EntityManager.Player.Player_UI.ChangeCrosshairDarkness(0.75f);
        }
    }
}
