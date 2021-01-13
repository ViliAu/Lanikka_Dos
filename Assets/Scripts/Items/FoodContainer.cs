using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodContainer : Interactable {

    [Header("Container data")]
    public List<Edible> edibles = new List<Edible>();
    public int maxFoodCount = 10;

    [Tooltip("How far the dookers can be when eating")]
    public float dookerDistance = 1f;

    [Header("Model spawn settings")]
    [Tooltip("Where the first model spawns")]
    [SerializeField] private Vector3 modelSpawnStartOffset = Vector3.zero;
    [SerializeField] private Vector3 rowOffset = Vector3.zero;
    [SerializeField] private Vector3 columnOffset = Vector3.zero;
    [SerializeField] private int rows = 2;

    [Header("Debug")]
    [SerializeField] private bool debug = false;

    private void Update() {
        CheckPosition();
        DrawLines();
    }

    private void CheckPosition() {
        if (transform.position.x > EntityManager.DookerPen.xBounds.y && transform.position.x < EntityManager.DookerPen.xBounds.x
            && transform.position.z > EntityManager.DookerPen.zBounds.y && transform.position.z < EntityManager.DookerPen.zBounds.x 
                && transform.position.y < 2) {
            EntityManager.DookerPen.foodContainer = this;
        }
        else {
            EntityManager.DookerPen.foodContainer = null;
        }
    }

    public override void PlayerInteract() {
        base.PlayerInteract();
        if (EntityManager.Player.Player_Equipment.equippedItem != null) {
            if (EntityManager.Player.Player_Equipment.equippedItem as Edible) {
                if (edibles.Count < maxFoodCount) {
                    edibles.Add((Edible)EntityManager.Player.Player_Equipment.equippedItem);
                    EntityManager.Player.Player_Inventory.DecrementStackSize(EntityManager.Player.Player_Equipment.itemIndex);
                    SoundSystem.PlaySound2D("place_item_generic");
                    UpdateModels();
                }
            }
        }
    }

    public void UpdateModels() {
        for (int i = 0; i < edibles.Count+1; i++) {
            if (transform.Find(i.ToString()) != null) {
                Destroy(transform.Find(i.ToString()).gameObject);
            }
        }
        int k = 0;
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < maxFoodCount/rows; j++) {
                if (k == edibles.Count)
                    break;
                Edible a = Instantiate<Edible>(Database.Singleton.GetEntityPrefab(edibles[i].entityName) as Edible, transform.position + transform.rotation * (modelSpawnStartOffset + i * rowOffset + j * columnOffset), transform.rotation, transform);
                a.name = k.ToString();
                a.GetComponent<Rigidbody>().isKinematic = true; // Presume that the edible has a rig
                a.canInteract = false;
                k++;
            }
        }
    }

    private void DrawLines() {
        if (!debug)
            return;
        Debug.DrawLine(transform.position, transform.position + transform.rotation * modelSpawnStartOffset, Color.red);
        Debug.DrawLine(transform.position + transform.rotation * modelSpawnStartOffset, transform.position + transform.rotation * modelSpawnStartOffset + transform.rotation * rowOffset, Color.green);
        Debug.DrawLine(transform.position + transform.rotation * modelSpawnStartOffset, transform.position + transform.rotation * modelSpawnStartOffset + transform.rotation * columnOffset, Color.blue);
    }

}
