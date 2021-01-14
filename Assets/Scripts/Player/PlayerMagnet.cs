using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagnet : MonoBehaviour {
    [SerializeField] private float radius = 1f;
    [SerializeField] private float force = 1f;
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private float magnetOffset = 1f;
    [SerializeField] private float magnetActivationTime = 0.5f;

    private float activationStart = 0f;

    [SerializeField] private List<Interactable> attractedItems = new List<Interactable>();

    private void Update() {
        GetInput();
    }

    private void GetInput() {
        if (EntityManager.Player.Player_Input.interacted)
            activationStart = Time.time;
        if (EntityManager.Player.Player_Input.magnet) {
            if (Time.time > activationStart + magnetActivationTime)
                CastSphere();
        }
        else {
            if (attractedItems.Count > 0) {
                ClearAttractedItems();
            }
        }
    }

    private void CastSphere() {
        Collider[] cols = Physics.OverlapSphere(EntityManager.Player.Player_Camera.head.position + EntityManager.Player.Player_Interaction.ray.direction * magnetOffset, radius, LayerMask.GetMask("Default") | LayerMask.GetMask("Grabbable"), QueryTriggerInteraction.Collide);
        if (cols.Length == 0) {
            return;
        }
        // Assign cols to the attracted items list
        for (int i = 0; i < cols.Length; i++) {
            Interactable item;
            if ((item = cols[i].GetComponent<Interactable>()) == null || item.rig == null) {
                continue;
            }
            if (item as Item || item as Money) {
                if (!attractedItems.Contains(item)) {
                    attractedItems.Add(item);
                }
            }
        }
        List <Interactable> removables = new List<Interactable>();
        
        foreach (Interactable item in attractedItems) {
            EntityManager.ChangeLayer(item.gameObject, 7);
            // Check if the item is close enough so we can pick it up
            float dist = (EntityManager.Player.Player_Camera.head.position + EntityManager.Player.Player_Interaction.ray.direction * magnetOffset - item.rig.position).magnitude;
            if (dist < pickupRange) {
                // Queue items to remove
                removables.Add(item);
                continue;
            }
            item.rig.AddForce(((EntityManager.Player.Player_Camera.head.position + EntityManager.Player.Player_Interaction.ray.direction * magnetOffset - item.rig.position) * (force/dist)));
        }
        foreach (Interactable item in removables) {
            if (item as Item) {
                if (item.PlayerInteractBool()) {
                    EntityManager.ChangeLayer(item.gameObject, 0);
                    attractedItems.Remove(item);
                }
            }
            else if (item as Money) {
                item.PlayerInteract();
                attractedItems.Remove(item);
            }
        }

    }

    private void ClearAttractedItems() {
        foreach (Interactable item in attractedItems) {
            EntityManager.ChangeLayer(item.gameObject, 0);
        }
        attractedItems.Clear();
    }
}