using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable {
   
   [SerializeField] private float openAngle = 90;
   [SerializeField] private float openSpeed = 5f;
   [SerializeField] private AudioClip openSound = null;
   [SerializeField] private AudioClip closeSound = null;
   private bool open = false;
   private float originalAngle;
   private float currAngle = 0;
   private Rigidbody rig;

   private void Awake() {
       rig = GetComponent<Rigidbody>();
       currAngle = originalAngle = rig == null ? transform.rotation.eulerAngles.y : rig.rotation.eulerAngles.y;
       openAngle += originalAngle;
       if (rig != null) {
           rig.centerOfMass = new Vector3(0, rig.centerOfMass.y, 0);
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
        float destinationAngle = open ? originalAngle : openAngle;
        float a = currAngle;

        while ((!open && !DUtil.Approx(a, openAngle, 0.1f)) || (open && !DUtil.Approx(a, originalAngle, 0.1f))) {
            currAngle = a = Mathf.Lerp(a, destinationAngle, openSpeed * Time.deltaTime);
            if (rig == null)
                transform.rotation = Quaternion.Euler(new Vector3(0, a, 0));
            else {
                rig.MoveRotation(Quaternion.Euler(new Vector3(0, a, 0)));
            }
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