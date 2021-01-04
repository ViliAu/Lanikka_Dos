using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

    public float range = 3f;
    [SerializeField] private LayerMask interactionMask = default;

    public Rigidbody interactRig;

    private Camera cam;
    private Ray ray;
    private RaycastHit hit;
    private Interactable intera;

    private void Awake() {
        cam = Camera.main;
    }

    private void Update() {
        CastInteractRay();
    }

    private void CastInteractRay() {
        ray = cam.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        if (Physics.Raycast(ray, out hit, range, interactionMask, QueryTriggerInteraction.Collide)) {
            interactRig = hit.transform.GetComponent<Rigidbody>();
            if ((intera = hit.transform.GetComponent<Interactable>()) != null) {
                intera.PlayerFocus();
                if (EntityManager.Player.Player_Input.interacted)
                    intera.PlayerInteract();
                return;
            }
            else {
                
            }
        }
        else {
            EntityManager.Player.Player_UI.ChangeCrosshairDarkness(0.75f);
            EntityManager.Player.Player_UI.ChangeCrosshair("crosshair_dot");
            EntityManager.Player.Player_UI.ChangeFocusText("");
            intera = null;
            interactRig = null;
        }
    }
}
