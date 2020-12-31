using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

    [SerializeField] private float range = 2f;
    [SerializeField] private LayerMask interactionMask = default;

    private Camera cam;
    private Ray ray;

    private void Awake() {
        cam = Camera.main;
    }

    private void Update() {
        GetInput();
    }

    private void GetInput() {
        if (EntityManager.Player.Player_Input.interacted)
            CastInteractRay();
    }

    private void CastInteractRay() {
        ray = cam.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, range, interactionMask, QueryTriggerInteraction.Ignore)) {
            Interactable intera = null;
            Pickupable pick = null;
            if ((intera = hit.transform.GetComponent<Interactable>()) != null) {
                intera.PlayerInteract();
                return;
            }
            else if ((pick = hit.transform.GetComponent<Pickupable>()) != null) {
                EntityManager.Player.Player_Inventory.AddItem(pick);
                return;
            }
            else {
                print("Löyty juttu muttei kiinnosta");
            }
        }
        else {
            print("Ei löytynny interaa");
        }
    }
}
