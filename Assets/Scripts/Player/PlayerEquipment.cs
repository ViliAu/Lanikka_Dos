using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEquipment : MonoBehaviour {

    [Header("Sway")]
    [SerializeField] private float swayAmount = 0.2f;
    [SerializeField] private float swaySpeed = 6f;
    [SerializeField] private Vector2 swayBounds = new Vector2(0.1f, 0.1f);

    [Header("Drop")]
    public float dropForce = 2f;

    public Item equippedItem = null;
    public int itemIndex = 0;
    public Transform hand = null;

    private Vector3 initialPos = default;
    private PlayerInventory inv;

    private void Awake() {
        inv = EntityManager.Player.Player_Inventory;
        hand = transform.Find("Head").Find("EquipmentHold").transform;
        initialPos = hand.localPosition;
        // Init UI
        UpdateUI();
    }

    private void Update() {
        CheckInput();
        Sway();
    }

    private void Sway() {
        Vector2 swayPos = EntityManager.Player.Player_Input.mouseInput * swayAmount;
        swayPos.x = Mathf.Clamp(swayPos.x, -swayBounds.x, swayBounds.x);
        swayPos.y = Mathf.Clamp(swayPos.y, -swayBounds.y, swayBounds.y);
        hand.localPosition = Vector3.Lerp(hand.localPosition, initialPos - (Vector3)swayPos, swaySpeed * Time.deltaTime);
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
        //Scroll
        if (scroll != 0) {
            num += scroll;
            num = num == 10 ? 0 : num == -1 ? 9 : num;
        }
        else {
            num = num == 0 ? 10 : num;
            num--;
        }

        Equip(EntityManager.Player.Player_Inventory.items[num]);
        itemIndex = num;
        UpdateUI();
    }

    private void Equip(Item p) {
        RemoveEquippedItem();
        if (p == null) {
            return;
        }
        p.gameObject.SetActive(true);
        p.transform.position = hand.position;
        p.transform.rotation = hand.rotation;
        p.transform.parent = hand;
        Rigidbody rig = null;
        if ((rig = p.GetComponent<Rigidbody>()) != null) {
            rig.isKinematic = true;
            rig.detectCollisions = false;
        }
        equippedItem = p;
    }

    public void RemoveEquippedItem() {
        if (equippedItem != null) {
            equippedItem.EnableItem(false);
            equippedItem = null;
            UpdateUI();
        }
    }

    public void DestroyEquippedItem() {
        if (equippedItem != null) {
            equippedItem.DestroyItem();
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
            rig.detectCollisions = true;
            rig.AddForce(hand.forward * dropForce, ForceMode.Impulse);
        }
        equippedItem = null;
        EntityManager.Player.Player_Inventory.RemoveItemByIndex(itemIndex, false);
        ChangeEquipment(itemIndex, 0);
    }

    private void UpdateUI() {
        if (equippedItem == null)
            EntityManager.Player.Player_UI.ChangeHeldItem(null, "");
        else {
            EntityManager.Player.Player_UI.ChangeHeldItem(Database.Singleton.GetIcon("icon_"+equippedItem.gameObject.name),
                EntityManager.Player.Player_Inventory.items[itemIndex].stackCount + "/" + EntityManager.Player.Player_Inventory.maxStackSize);
        }
    }

}
