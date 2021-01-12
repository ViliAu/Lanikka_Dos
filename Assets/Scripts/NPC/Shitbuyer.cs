using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Shitbuyer : Shopkeeper {
    [Header("Shit buyer attributes")]
    [SerializeField] private Door dumpsterLid = null;
    [SerializeField] private Door garageDoor = null;
    [SerializeField] private Sellpoint sellpoint;
    [SerializeField] private float openRange = 2f;
    [SerializeField] private Vector3 restPosition = Vector3.zero;
    [SerializeField] private float buyInterval = 10f;
    [SerializeField] private float maxAmount = 100f;
    [SerializeField] private AudioClip[] laughStates;

    private Vector3 buyPosition = Vector3.zero;
    private NavMeshAgent agent = null;
    private bool active = true;

    protected override void Awake() {
        base.Awake();
        if (dumpsterLid == null) {
            Debug.LogError("Shit buyer dumpster lid not assigned...");
        }
        if (garageDoor == null) {
            Debug.LogError("Shit buyer garage door not assigned...");
        }
        if (sellpoint == null) {
            Debug.LogError("Shit buyer doesn't have a sellpoint assigned");
        }
        else {
            sellpoint.OnSellingStarted += SellStartedCallBack;
            sellpoint.OnSellingFinished += SellFinishedCallback;
        }
        if ((agent = GetComponent<NavMeshAgent>()) == null) {
            Debug.LogError("Shit buyer navmeshagent not found...");
        }
        buyPosition = transform.position;
    }

    protected override void CheckVision() {
        if (!active)
            return;
        base.CheckVision();
        if (playerRange < openRange && !dumpsterLid.Open) {
            dumpsterLid.ChangeState(false);
        }
        else if (playerRange > openRange && dumpsterLid.Open){
            dumpsterLid.ChangeState(true);
        }
    }

    private void SellStartedCallBack(float value) {
        active = false;
        SoundSystem.PlaySound(CalculateLaughState(value), transform.position);
        sellpoint.canInteract = false;
    }

    // TODO: Temppi vaa
    private void SellFinishedCallback(float value) {
        if (value <= 0) {
            SoundSystem.PlaySound(Random.Range(0, 2) == 0 ? "shitbuyer_angry" : "shitbuyer_angry2", transform.position);
            return;
        }
        dumpsterLid.ChangeState(true);
        playerInVision = false;
        StartCoroutine(WalkAnimation());
    }

    private string CalculateLaughState(float value) {
        int i = (int)(Mathf.Clamp01(value / maxAmount) * (laughStates.Length - 1));
        return laughStates[i].name;
    }

    private IEnumerator WalkAnimation() {
        // Wait for dumpster lid animation
        while (dumpsterLid.playAnimation != null) {
            yield return null;
        }
        // Wait for garage door animation
        garageDoor.ChangeState(false);
        while (garageDoor.playAnimation != null) {
            yield return null;
        }
        // Go to the garage
        agent.destination = restPosition;
        while (agent.remainingDistance > 0) {
            yield return null;
        }
        garageDoor.ChangeState(true);
        // Wait for the timeout
        yield return new WaitForSecondsRealtime(buyInterval);
        // Go to the sell point
        agent.destination = buyPosition;
        while (agent.remainingDistance > 0) {
            yield return null;
        }
        active = true;
        sellpoint.canInteract = true;
        yield return null;
    }

}
