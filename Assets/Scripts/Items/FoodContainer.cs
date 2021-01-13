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
    [SerializeField] private float positionJitter = 0.1f;
    [SerializeField] private float rotationJitter = 360f;
    [SerializeField] private int rows = 2;

    [Header("Debug")]
    [SerializeField] private bool debug = false;

    int currentModels = 0;

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
                    UpdateModels(true);
                }
            }
        }
    }

    /// <summary>
    /// Black magic (Updates the models of the table)
    /// </summary>
    /// <param name="append">If we added an item to the table</param>
    public void UpdateModels(bool append) {
        if (!append) {
            GameObject go;
            if ((go = transform.Find(edibles.Count.ToString()).gameObject) != null) {
                Destroy(go);
            }
            return;
        }
        int k = edibles.Count-1;
        Edible a = Instantiate<Edible>(Database.Singleton.GetEntityPrefab(edibles[k].entityName) as Edible, transform.position + transform.rotation * (modelSpawnStartOffset + (k % this.rows) * rowOffset + ((int) k / this.rows) * columnOffset) + transform.right * Random.Range(-positionJitter, positionJitter) + transform.forward * Random.Range(-positionJitter, positionJitter), transform.rotation * Quaternion.Euler(0, Random.Range(-rotationJitter, rotationJitter), 0), transform); // Mörkö (älä koske)
        a.name = k.ToString();
        a.GetComponent<Rigidbody>().isKinematic = true; // Presume that the edible has a rig
        a.GetComponent<Rigidbody>().detectCollisions = false;
        a.canInteract = false;
    }

    private void DrawLines() {
        if (!debug)
            return;
        Debug.DrawLine(transform.position, transform.position + transform.rotation * modelSpawnStartOffset, Color.red);
        Debug.DrawLine(transform.position + transform.rotation * modelSpawnStartOffset, transform.position + transform.rotation * modelSpawnStartOffset + transform.rotation * rowOffset, Color.green);
        Debug.DrawLine(transform.position + transform.rotation * modelSpawnStartOffset, transform.position + transform.rotation * modelSpawnStartOffset + transform.rotation * columnOffset, Color.blue);
    }

}
