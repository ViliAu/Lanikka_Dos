using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable {
   
   [SerializeField] private float openAngle = 90;
   [SerializeField] private float openSpeed = 5f;
   [SerializeField] private AudioClip openSound = null;
   [SerializeField] private AudioClip closeSound = null;
   [SerializeField] private Axis axis = Axis.Y;
   private bool open = false;
   private Rigidbody rig;

   private Quaternion originalRotation = Quaternion.identity;
   private Quaternion currentRotation = Quaternion.identity;
   private Quaternion openRotation = Quaternion.identity;

   private enum Axis {
       X,
       Y,
       Z
   }

   private void Awake() {
       rig = GetComponent<Rigidbody>();
       switch (axis) {
           case Axis.X: {
               currentRotation = originalRotation = rig == null ? transform.rotation : rig.rotation;
               openRotation = originalRotation * Quaternion.Euler(openAngle, 0, 0);
               break;
           }
           case Axis.Y: {
               currentRotation = originalRotation = rig == null ? transform.rotation : rig.rotation;
               openRotation = originalRotation * Quaternion.Euler(0, openAngle, 0);
               break;
           }
           case Axis.Z: {
               currentRotation = originalRotation = rig == null ? transform.rotation : rig.rotation;
               openRotation = originalRotation * Quaternion.Euler(0, 0, openAngle);
               break;
           }
       }
   }

    public override void PlayerInteract() {
        base.PlayerInteract();
        StopAllCoroutines();
        StartCoroutine(PlayAnimation(open));
        PlaySound(open);
        open = !open;
    }

    private IEnumerator PlayAnimation(bool open) {
        Quaternion destinationRotation = open ? originalRotation : openRotation;
        while ((!open && Quaternion.Angle(transform.rotation, openRotation) > 0.1f) || (open && Quaternion.Angle(transform.rotation, originalRotation) > 0.1f)) {
            transform.rotation = Quaternion.Lerp(transform.rotation, destinationRotation, openSpeed * Time.deltaTime);
            yield return null;
        }
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