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
   private float originalAngle;
   private float currAngle = 0;
   private Rigidbody rig;

   private enum Axis {
       X,
       Y,
       Z
   }

   private void Awake() {
       rig = GetComponent<Rigidbody>();
       switch (axis) {
           case Axis.X: {
               currAngle = originalAngle = rig == null ? transform.rotation.eulerAngles.x : rig.rotation.eulerAngles.x;
               break;
           }
           case Axis.Y: {
               currAngle = originalAngle = rig == null ? transform.rotation.eulerAngles.y : rig.rotation.eulerAngles.y;
               break;
           }
           case Axis.Z: {
               currAngle = originalAngle = rig == null ? transform.rotation.eulerAngles.z : rig.rotation.eulerAngles.z;
               break;
           }
       }

       openAngle += originalAngle;
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
            switch (axis) {
                case Axis.X: {
                    if (rig == null)
                        transform.rotation = Quaternion.Euler(new Vector3(a, 0, 0));
                    else
                        rig.MoveRotation(Quaternion.Euler(new Vector3(a, 0, 0)));
                    break;
                }
                case Axis.Y: {
                    if (rig == null)
                        transform.rotation = Quaternion.Euler(new Vector3(0, a, 0));
                    else
                        rig.MoveRotation(Quaternion.Euler(new Vector3(0, a, 0)));
                    break;
                }
                case Axis.Z: {
                    if (rig == null)
                        transform.rotation = Quaternion.Euler(new Vector3(0, 0, a));
                    else
                        rig.MoveRotation(Quaternion.Euler(new Vector3(0, 0, a)));
                    break;
                }
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