using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShitBucket : Interactable {
    
    [Header("Container data")]
    public List<Doodie> doodies = new List<Doodie>();
    public int maxCount = 10;

    [Tooltip("How far the dookers can be when shitting")]
    public float dookerDistance = 1f;

    private void Update() {
        CheckPosition();
    }

    private void CheckPosition() {
        if (transform.position.x > EntityManager.DookerPen.xBounds.y && transform.position.x < EntityManager.DookerPen.xBounds.x
            && transform.position.z > EntityManager.DookerPen.zBounds.y && transform.position.z < EntityManager.DookerPen.zBounds.x 
                && transform.position.y < 2) {
            EntityManager.DookerPen.shitBucket = this;
        }
        else {
            EntityManager.DookerPen.shitBucket = null;
        }
    }

    public bool CanAddShit() {
        int i = 0;
        foreach (Doodie d in doodies) {
            i += d.stackCount;
        }
        return i < maxCount;
    }

    public void AddShit(Doodie diuppa) {
        // Simply add the doodie in a stack
        foreach (Doodie d in doodies) {
            if (d.entityName == diuppa.entityName) {
                d.stackCount++;
                return;
            }
        }
        // Instantiate and add a new prefab
        Doodie duu = Instantiate<Doodie>(Database.Singleton.GetEntityPrefab(diuppa.entityName) as Doodie, transform.position, transform.rotation, null);
        duu.EnableItem(false);
        doodies.Add(duu);
    }

    public override void PlayerInteract() {
        base.PlayerInteract();
        if (doodies.Count == 0)
            return;
        if (doodies.Count > 1) {
            for (int i = 0; i < doodies.Count; i++) {
                EntityManager.Player.Player_Inventory.AddItem(doodies[i]);
                doodies.RemoveAt(i);
            }
        }
        else {
            EntityManager.Player.Player_Inventory.AddItem(doodies[0]);
            doodies.RemoveAt(0);
        }
    }

}
