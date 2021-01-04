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
    private Interactable interactable;

    private void Awake() {
        cam = Camera.main;
    }

    private void Update() {
        CastInteractRay();
    }

    private void CastInteractRay() {
        ray = cam.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        if (Physics.Raycast(ray, out hit, range, interactionMask, QueryTriggerInteraction.Collide)) {
            Interactable intera = null;
            // Get rigidbody for grabbing
            if (interactRig == null) {
                interactRig = hit.transform.GetComponent<Rigidbody>();
            }
            // Get intera for interacting / focusing
            if ((intera = hit.transform.GetComponent<Interactable>()) != null) {
                // If the interactable thing exists and we currently are not focusing on an interactable object
                if (interactable == null) {
                    interactable = intera;
                    interactable.PlayerFocusEnter();
                }
                // If the interactable thing is same as ours
                else if (intera.GetInstanceID() == interactable.GetInstanceID()) {
                    if (EntityManager.Player.Player_Input.interacted)
                        interactable.PlayerInteract();
                }
                // If we lose focus and are focusing on a new interactable
                else if (intera.GetInstanceID() != interactable.GetInstanceID()) {
                    interactable.PlayerFocusExit();
                    interactable = intera;
                    interactable.PlayerFocusEnter();
                }
            }
            // We aren't focusing on anything interactable
            else {
                LoseFocus();
            }
        }
        // We aren't hitting or focusing on anything
        else {
            LoseFocus();
        }
    }
    private void LoseFocus() {
        EntityManager.Player.Player_UI.ChangeCrosshairDarkness(0.75f);
        EntityManager.Player.Player_UI.ChangeCrosshair("crosshair_dot");
        EntityManager.Player.Player_UI.ChangeFocusText("");
        if (interactable != null)
            interactable.PlayerFocusExit();
        interactable = null;
        interactRig = null;
    }
}
