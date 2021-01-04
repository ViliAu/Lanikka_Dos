using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour {

    [Header("Settings")]
    [SerializeField] private float scrollInterval = 0.1f;
    [SerializeField] private float scrollBounds = 1f;
    [SerializeField] private float grabSpeed = 5f;
    [SerializeField] private float rotationSpeed = 20f;

    private Rigidbody grabbedRig = null;
    private Vector3 destination = Vector3.zero;
    private float scrollOffset = 1;
    private bool equipped = false;

    private Quaternion originalRotation = Quaternion.identity;

    private void Update() {
        CheckInput();
        UpdateDestination();
        MoveRig();
        RotateRig();
    }

    private void CheckInput() {
        if (EntityManager.Player.Player_Input.grab == 1) {
            if (EntityManager.Player.Player_Interaction.interactRig != null && grabbedRig == null) {
                GrabObject(true);
            }
        }
        else if (EntityManager.Player.Player_Input.grab == -1 && !equipped) {
            if (grabbedRig != null) {
                GrabObject(false);
            }
        }
        scrollOffset += EntityManager.Player.Player_Input.scroll * scrollInterval;
        scrollOffset = Mathf.Clamp(scrollOffset, -scrollBounds, scrollBounds);
        if (scrollOffset == -scrollBounds && grabbedRig != null) {
            EquipRig();
        }
        if ((equipped && EntityManager.Player.Player_Equipment.equippedItem != null)
            || (equipped && EntityManager.Player.Player_Input.dropped)) {
            DisEquipRig();
        }
    }

    private void GrabObject(bool grab) {
        if (grab) {
            grabbedRig = EntityManager.Player.Player_Interaction.interactRig;
            grabbedRig.interpolation = RigidbodyInterpolation.Interpolate;
            originalRotation = grabbedRig.transform.rotation;
            grabbedRig.freezeRotation = true;
            scrollOffset = 1;
        }
        else {
            grabbedRig.interpolation = RigidbodyInterpolation.None;
            EntityManager.Player.Player_Camera.locked = false;
            grabbedRig.freezeRotation = false;
            grabbedRig = null;
        }
    }

    private void UpdateDestination() {
        destination = EntityManager.Player.Player_Camera.head.transform.position + EntityManager.Player.Player_Camera.head.forward
            * EntityManager.Player.Player_Interaction.range + EntityManager.Player.Player_Camera.head.forward * scrollOffset;
    }

    private void MoveRig() {
        if (grabbedRig == null || equipped) {
            return;
        }
        grabbedRig.velocity = (destination - grabbedRig.transform.position) * grabSpeed;
    }

    private void EquipRig() {
        // If the grabbed thing is actually an item, add it to inv
        Item i = null;
        if ((i = grabbedRig.transform.GetComponent<Item>()) != null) {
            GrabObject(false);
            EntityManager.Player.Player_Inventory.AddItem(i);
            return;
        }
        equipped = true;
        grabbedRig.isKinematic = true;
        EntityManager.Player.Player_Equipment.RemoveEquippedItem();
        grabbedRig.transform.position = EntityManager.Player.Player_Equipment.hand.transform.position;
        grabbedRig.transform.rotation = EntityManager.Player.Player_Equipment.hand.transform.rotation;
        grabbedRig.transform.parent = EntityManager.Player.Player_Equipment.hand;
    }

    private void DisEquipRig() {
        equipped = false;
        grabbedRig.isKinematic = false;
        grabbedRig.transform.parent = null;
        GrabObject(false);
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
