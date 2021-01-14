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

    public Rigidbody grabbedRig = null;
    private Vector3 destination = Vector3.zero;
    private float scrollOffset = 1;
    private bool equipped = false;

    private Quaternion originalRotation = Quaternion.identity;
    private int ogLayer = 0;

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
            EntityManager.Player.Player_UI.ChangeCrosshair("crosshair_grab");
            if (EntityManager.Player.Player_Input.interacted)
                EquipObject(true);
        }
        else {
            if (EntityManager.Player.Player_UI.GetCrosshairName() == "crosshair_grab")
                EntityManager.Player.Player_UI.ChangeCrosshair("crosshair_dot");
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
            grabbedRig = EntityManager.Player.Player_Interaction.interactRig;
            grabbedRig.interpolation = RigidbodyInterpolation.Interpolate;
            originalRotation = grabbedRig.transform.rotation;
            grabbedRig.freezeRotation = true;
            ogLayer = grabbedRig.gameObject.layer;
            EntityManager.ChangeLayer(grabbedRig.gameObject, 7);
            scrollOffset = Vector3.Distance(grabbedRig.transform.position,
                EntityManager.Player.Player_Camera.head.position) / EntityManager.Player.Player_Interaction.range;
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
            if (grabbedRig == null)
                return;
            destination = EntityManager.Player.Player_Camera.head.transform.position + EntityManager.Player.Player_Camera.head.forward
                * (EntityManager.Player.Player_Interaction.range * scrollOffset);

            grabbedRig.velocity = (destination - grabbedRig.transform.position) * grabSpeed;
        }
        else {
            grabbedRig.transform.position = EntityManager.Player.Player_Equipment.hand.transform.position;
            grabbedRig.transform.rotation = EntityManager.Player.Player_Equipment.hand.transform.rotation;
            grabbedRig.transform.parent = EntityManager.Player.Player_Equipment.hand;
        }
    }

    private void EquipObject(bool eq) {
        EntityManager.Player.Player_UI.ChangeCrosshair("crosshair_dot");
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
            grabbedRig.transform.Rotate(transform.rotation * GetMouseEulers() , Space.World);
            EntityManager.Player.Player_Camera.locked = true;
        }
        else {
            EntityManager.Player.Player_Camera.locked = false;
        }
    }

    private Vector3 GetMouseEulers() {
        return new Vector3(EntityManager.Player.Player_Input.mouseInput.y * rotationSpeed * Time.deltaTime, -EntityManager.Player.Player_Input.mouseInput.x * rotationSpeed * Time.deltaTime, 0);
    }
}
