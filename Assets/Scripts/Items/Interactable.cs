using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : Entity {

    [Header("Interactable settings")]
    public bool canInteract = true;
    // Highlight settings
    public bool isHighlightable = true;
    public bool isGrabbable = true;
    
    private Coroutine highlightCoroutine = null;
    private MeshRenderer[] meshRenderers = null;
    private const string HL_PROPERTY_NAME = "_HighlightAmount";

    public Rigidbody rig {get; private set;}

    protected virtual void Awake() {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        rig = GetComponent<Rigidbody>();
    }


    /// <summary>
    /// Get's called when the player interacts with an item
    /// </summary>
    public virtual void PlayerInteract() {
        if (!canInteract)
            return;
    }

    /// <summary>
    /// Get's called when the player focuses to an item
    /// </summary>
    public virtual void PlayerFocusEnter() {
        if (!canInteract) {
            PlayerFocusExit();
            return;
        }
        EntityManager.Player.Player_UI.ChangeCrosshairDarkness(1f);

        if (highlightCoroutine != null)
            StopCoroutine(highlightCoroutine);
        highlightCoroutine = StartCoroutine(PlayHighlightAnimation(1));
    }

    /// <summary>
    /// Get's called when the player loses focus to an item
    /// </summary>
    public virtual void PlayerFocusExit() {
        EntityManager.Player.Player_UI.ChangeCrosshairDarkness(0.75f);
        EntityManager.Player.Player_UI.ChangeFocusText("");

        if (gameObject.activeSelf) {
            if (highlightCoroutine != null)
                StopCoroutine(highlightCoroutine);
            highlightCoroutine = StartCoroutine(PlayHighlightAnimation(0));
        }
        else {
            SetHighlightAmount(0);
        }
    }

    private IEnumerator PlayHighlightAnimation(float target) {
        if (meshRenderers == null || meshRenderers.Length == 0 || !meshRenderers[0].material.HasProperty(HL_PROPERTY_NAME)) {
            yield break;
        }

        float animationSpeed = 5f;
        float hl = meshRenderers[0].material.GetFloat(HL_PROPERTY_NAME);
        while (hl != target) {
            hl = Mathf.MoveTowards(hl, target, animationSpeed * Time.deltaTime);
            SetHighlightAmount(hl);
            yield return null;
        }
        highlightCoroutine = null;
    }

    private void SetHighlightAmount(float target) {
        if (meshRenderers == null || !meshRenderers[0].material.HasProperty(HL_PROPERTY_NAME)) {
            return;
        }

        foreach (MeshRenderer mr in meshRenderers) {
            foreach (Material m in mr.materials) {
                m.SetFloat(HL_PROPERTY_NAME, target);
            }
        } 
    }
}
