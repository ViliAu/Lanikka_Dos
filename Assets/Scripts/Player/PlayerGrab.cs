using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour {

    [Header("Settings")]
    [Tooltip("How much the object moves when scrolling * interactionrange")]
    [SerializeField] private float scrollInterval = 0.1f;
    [Tooltip("Multiplier for interaction range")]
    [SerializeField] private Vector2 scrollBounds = Vector2.zero;
    [SerializeField] private float grabSpeed = 5f;
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float throwForce = 100f;

    [SerializeField] private float rotationCenteringSpeed = 5f;

    private Vector3 destination = Vector3.zero;
    private Vector3 offset = Vector3.zero;
    private float scrollOffset = 1;
    private bool equipped = false;
    private int ogLayer = 0;

    public Rigidbody grabbedRig = null;

    private void Update() {
        CheckInput();
        MoveRig();
        RotateRig();
        if (equipped)
            EquipObject(true);
    }

    private void CheckInput() {
        // Grab
        if (EntityManager.Player.Player_Input.grab == 1) {
            if (EntityManager.Player.Player_Interaction.interactRig != null && grabbedRig == null) {
                GrabObject(true);
            }
        }
        // Let go
        else if (EntityManager.Player.Player_Input.grab == -1 && !equipped) {
            if (grabbedRig != null) {
                GrabObject(false);
            }
        }
        // Scroll + scroll equip
        if (!equipped) {
            scrollOffset += EntityManager.Player.Player_Input.scroll * scrollInterval;
            scrollOffset = Mathf.Clamp(scrollOffset, scrollBounds.x, scrollBounds.y);
        }
        if (scrollOffset == scrollBounds.x && grabbedRig != null) {
            EntityManager.Player.Player_UI.SetCrosshair("crosshair_grab");
            if (EntityManager.Player.Player_Input.interacted)
                EquipObject(true);
        }
        else {
            if (EntityManager.Player.Player_UI.GetCrosshairName() == "crosshair_grab")
                EntityManager.Player.Player_UI.SetCrosshair("crosshair_dot");
        }
        if ((equipped && EntityManager.Player.Player_Equipment.equippedItem != null)
            || (equipped && EntityManager.Player.Player_Input.dropped)) {
            EquipObject(false);
        }
        // Throw
        if (EntityManager.Player.Player_Input.threw && !equipped && grabbedRig != null) {
            grabbedRig.AddForce(EntityManager.Player.Player_Camera.head.forward * throwForce, ForceMode.Impulse);
            GrabObject(false);
        }
    }

    private void GrabObject(bool grab) {
        if (grab) {
            // Setup gameobject
            grabbedRig = EntityManager.Player.Player_Interaction.interactRig;
            grabbedRig.interpolation = RigidbodyInterpolation.Interpolate;
            grabbedRig.freezeRotation = true;
            ogLayer = grabbedRig.gameObject.layer;
            EntityManager.ChangeLayer(grabbedRig.gameObject, 7);

            // Setup offset
            scrollOffset = Vector3.Distance(EntityManager.Player.Player_Interaction.interactionPoint, EntityManager.Player.Player_Camera.head.position) / EntityManager.Player.Player_Interaction.range;
            offset = grabbedRig.transform.position - EntityManager.Player.Player_Interaction.interactionPoint;
        }
        else {
            EntityManager.ChangeLayer(grabbedRig.gameObject, ogLayer);
            grabbedRig.interpolation = RigidbodyInterpolation.None;
            EntityManager.Player.Player_Camera.locked = false;
            grabbedRig.freezeRotation = false;
            grabbedRig = null;
        }
    }

    private void MoveRig() {
        if (!equipped) {
            if (grabbedRig == null) {
                return;
            }
            destination = EntityManager.Player.Player_Camera.head.position + (EntityManager.Player.Player_Camera.head.forward * EntityManager.Player.Player_Interaction.range * scrollOffset) + offset;
            grabbedRig.velocity = (destination - grabbedRig.transform.position) * grabSpeed;
        }
        else {
            grabbedRig.transform.position = EntityManager.Player.Player_Equipment.hand.transform.position;
            grabbedRig.transform.rotation = EntityManager.Player.Player_Equipment.hand.transform.rotation;
            grabbedRig.transform.parent = EntityManager.Player.Player_Equipment.hand;
        }
    }

    private void EquipObject(bool eq) {
        EntityManager.Player.Player_UI.SetCrosshair("crosshair_dot");
        if (eq) {
            // If the grabbed thing is actually an item, add it to inv instead
            Item i = null;
            if ((i = grabbedRig.transform.GetComponent<Item>()) != null) {
                GrabObject(false);
                EntityManager.Player.Player_Inventory.AddItem(i);
                return;
            }
            equipped = true;
            grabbedRig.isKinematic = true;
            grabbedRig.detectCollisions = false;
            EntityManager.Player.Player_Equipment.RemoveEquippedItem();
        }
        else {
            equipped = false;
            grabbedRig.isKinematic = false;
            grabbedRig.detectCollisions = true;
            grabbedRig.transform.parent = null;
            grabbedRig.AddForce(EntityManager.Player.Player_Camera.head.forward * EntityManager.Player.Player_Equipment.dropForce, ForceMode.Impulse);
            GrabObject(false);
        }
    }

    private void RotateRig() {
        if (grabbedRig == null) {
            return;
        }
        if (EntityManager.Player.Player_Input.rotating) {
            grabbedRig.transform.Rotate(transform.rotation * GetMouseEulers() * rotationSpeed * Time.deltaTime , Space.World);
            EntityManager.Player.Player_Camera.locked = true;
            if (GetMouseEulers().magnitude > 0) {
                offset = Vector3.Lerp(offset, grabbedRig.GetComponent<Interactable>().grabOffset, rotationCenteringSpeed * Time.deltaTime);
            }
        }
        else {
            EntityManager.Player.Player_Camera.locked = false;
        }
    }

    private Vector3 GetMouseEulers() {
        return new Vector3(EntityManager.Player.Player_Input.mouseInput.y, -EntityManager.Player.Player_Input.mouseInput.x, 0);
    }
}
