using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopkeeper : NPC {
    [SerializeField] private Transform headBone = null;
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float visionAngle = 180f;
    [SerializeField] private float visionCheckInterval = 0.5f;
    [SerializeField] private float lookSpeed = 5f; 
    [SerializeField] private Vector3 visionOffset = Vector3.zero;

    private Quaternion restRotation;
    private bool playerInVision = false;

    // HuoM TÄMÄ ON TEMPPI TÄMÄ VAIHTUU SIT KU SAADAAN KUNNOLLA VOISLAINEJA
    private bool playerGreeted = false;
    RaycastHit hit;

    private void Awake() {
        if (headBone == null) {
            Debug.LogError("Shopkeeper "+entityName+" does not have his head bone assigned.");
            return;
        }
        restRotation = headBone.rotation;
        InvokeRepeating("CheckVision", 0, visionCheckInterval);
    }

    private void Update() {
        RotateHead();    
    }

    private void CheckVision() {
        Vector3 visionVector = EntityManager.Player.Player_Camera.head.position - headBone.transform.position;
        playerInVision = visionVector.magnitude <= visionRange && Vector3.Angle(transform.forward, visionVector) <= visionAngle;
    }

    private void RotateHead() {
        if (playerInVision) {
            headBone.transform.rotation = Quaternion.Lerp(headBone.transform.rotation,
                Quaternion.LookRotation(EntityManager.Player.Player_Camera.head.position - headBone.transform.position + visionOffset, Vector3.up)
                    , lookSpeed * Time.deltaTime);
            Debug.DrawLine(EntityManager.Player.Player_Camera.head.position, headBone.transform.position);
            if (!playerGreeted) {
                if (Physics.Raycast(headBone.position, EntityManager.Player.Player_Camera.head.position - headBone.transform.position, out hit, visionRange, LayerMask.GetMask("Default") | LayerMask.GetMask("Player"), QueryTriggerInteraction.Collide)) {
                    if (hit.transform.GetComponent<Player>() != null) {
                        SoundSystem.PlaySound("shopkeeper_greet_player", transform.position);
                        playerGreeted = true;
                    }
                }
            }
        }
        else {
            headBone.transform.rotation = Quaternion.Lerp(headBone.transform.rotation, restRotation, lookSpeed * Time.deltaTime);
            playerGreeted = false;
        }
    }


}
