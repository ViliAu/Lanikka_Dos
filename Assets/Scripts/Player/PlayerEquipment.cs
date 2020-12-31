using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour {

    [Header("Sway")]
    [SerializeField] private float swayAmount = 0.2f;
    [SerializeField] private float swaySpeed = 6f;
    [SerializeField] private Vector2 swayBounds = new Vector2(0.1f, 0.1f);

    [Header("Drop")]
    [SerializeField] private float dropForce = 2f;

    private Vector3 initialPos = default;
    private PlayerInventory inv;
    private Pickupable equippedItem = null;
    private int itemIndex = 0;

    private void Start() {
        initialPos = transform.localPosition;
        inv = EntityManager.Player.Player_Inventory;
    }

    private void Update() {
        CheckInput();
        Sway();
    }

    private void Sway() {
        Vector2 swayPos = EntityManager.Player.Player_Input.mouseInput * swayAmount;
        swayPos.x = Mathf.Clamp(swayPos.x, -swayBounds.x, swayBounds.x);
        swayPos.y = Mathf.Clamp(swayPos.y, -swayBounds.y, swayBounds.y);
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPos - (Vector3)swayPos, swaySpeed * Time.deltaTime);
    }

    private void CheckInput() {
        // Numeric equipment selection
        if (EntityManager.Player.Player_Input.pressedNum != -1) {
            ChangeEquipment(EntityManager.Player.Player_Input.pressedNum, 0);
        }
        if (EntityManager.Player.Player_Input.dropped) {
            DropCurrentStack();
        }
        // Scrollwheel select
        if (EntityManager.Player.Player_Input.scroll != 0) {
            ChangeEquipment(itemIndex, EntityManager.Player.Player_Input.scroll);
        }
    }

    private void ChangeEquipment(int num, int scroll) {

        // If we don't have that item
        if (EntityManager.Player.Player_Inventory.items[num] == null) {
            // Iterate slots so we'll equip the first applicable weapon
            if (scroll != 0) {
                for (int i = 0; i < 10; i++) {
                    if (EntityManager.Player.Player_Inventory.items[num] == null) {
                        if (scroll == 1) {
                            num ++;
                            num = num == 10 ? 0 : num;
                        }
                        else {
                            num --;
                            num = num == -1 ? 9 : num;
                        }
                    }
                    else {
                        Equip(EntityManager.Player.Player_Inventory.items[num]);
                        return;
                    }
                }
            }
            else {
                RemoveEquippedItem();
                return;
            }
        }

        // We have that item;
        print(num);
        Equip(EntityManager.Player.Player_Inventory.items[num]);
        itemIndex = num;
    }

    private void Equip(Pickupable p) {
        if (p == null) {
            return;
        }
        RemoveEquippedItem();
        p.gameObject.SetActive(true);
        p.transform.position = transform.position;
        p.transform.rotation = transform.rotation;
        p.transform.parent = transform;
        Rigidbody rig = null;
        if ((rig = p.GetComponent<Rigidbody>()) != null) {
            rig.isKinematic = true;
        }
        equippedItem = p;
    }

    private void RemoveEquippedItem() {
        if (equippedItem != null) {
            equippedItem.EnableItem(false);
            equippedItem = null;
        }
    }

    private void DropCurrentStack() {
        if (equippedItem == null) {
            return;
        }
        // Spawn item
        equippedItem.transform.parent = null;
        Rigidbody rig = null;
        if ((rig = equippedItem.GetComponent<Rigidbody>()) != null) {
            rig.isKinematic = false;
            rig.AddForce(transform.forward * dropForce, ForceMode.Impulse);
        }
        equippedItem = null;
        EntityManager.Player.Player_Inventory.RemoveItemByIndex(itemIndex);
        ChangeEquipment(itemIndex, 1);
    }
}
