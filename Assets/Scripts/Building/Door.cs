using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable {
   
   [Header("Door settings")]
   [SerializeField] private bool interactable = true;
   [SerializeField] private float openAngle = 90;
   [SerializeField] private float openSpeed = 5f;
   [SerializeField] private AudioClip openSound = null;
   [SerializeField] private AudioClip closeSound = null;
   [SerializeField] private Axis axis = Axis.Y;
   public bool Open {get; private set;}
   private Rigidbody rig;

   private Quaternion originalRotation = Quaternion.identity;
   private Quaternion currentRotation = Quaternion.identity;
   private Quaternion openRotation = Quaternion.identity;

   public Coroutine playAnimation = null;

   private enum Axis {
       X,
       Y,
       Z
   }

   private void Awake() {
       Open = false;
       rig = GetComponent<Rigidbody>();
       currentRotation = originalRotation = rig == null ? transform.rotation : rig.rotation;
       openRotation = originalRotation * Quaternion.Euler(axis == Axis.X ? openAngle : 0, axis == Axis.Y ? openAngle : 0, axis == Axis.Z ? openAngle : 0);
   }

    public override void PlayerInteract() {
        if (!interactable)
            return;
        base.PlayerInteract();
        ChangeState(Open);
    }

    public void ChangeState(bool open) {
        if (playAnimation != null)
            StopCoroutine(playAnimation);
        playAnimation = StartCoroutine(PlayAnimation(open));
        PlaySound(open);
        this.Open = !open;
    }

    private IEnumerator PlayAnimation(bool open) {
        Quaternion destinationRotation = open ? originalRotation : openRotation;
        while ((!open && Quaternion.Angle(transform.rotation, openRotation) > 0.5f) || (open && Quaternion.Angle(transform.rotation, originalRotation) > 0.5f)) {
            transform.rotation = Quaternion.Lerp(transform.rotation, destinationRotation, openSpeed * Time.deltaTime);
            yield return null;
        }
        playAnimation = null;
    }

    private void PlaySound(bool open) {
        if (!open) {
            if (openSound != null) {
                SoundSystem.PlaySound(openSound.name, transform.position);
            }
            else {
                SoundSystem.PlaySound("door_squeak", transform.position);
            }
        }
        else {
            if (openSound != null) {
                SoundSystem.PlaySound(closeSound.name, transform.position);
            }
            else {
                SoundSystem.PlaySound("door_close", transform.position);
            }
        }
    }
}