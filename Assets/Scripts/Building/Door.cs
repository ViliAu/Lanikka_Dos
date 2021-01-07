using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable {
   
   [SerializeField] private float openAngle = 90;
   [SerializeField] private float openSpeed = 5f;
   private bool open = false;
   private float originalAngle;
   private Rigidbody rig;

   private void Awake() {
       rig = GetComponent<Rigidbody>();
       originalAngle = rig == null ? transform.rotation.eulerAngles.y : rig.rotation.eulerAngles.y;
       openAngle += originalAngle;
       if (rig != null) {
           rig.centerOfMass = new Vector3(0, rig.centerOfMass.y, 0);
       }
   }

    public override void PlayerInteract() {
        base.PlayerInteract();
        StopAllCoroutines();
        StartCoroutine(PlayAnimation(open));
        if (!open)
            SoundSystem.PlaySound("door_squeak", transform.position);
        else
            SoundSystem.PlaySound("door_close", transform.position);
        open = !open;
    }

    private IEnumerator PlayAnimation(bool open) {
        float destinationAngle = open ? originalAngle : openAngle;
        float a = rig == null ? transform.rotation.eulerAngles.y : rig.rotation.eulerAngles.y;

        while ((!open && !DUtil.Approx(a, openAngle, 0.1f)) || (open && !DUtil.Approx(a, originalAngle, 0.1f))) {
            a = Mathf.Lerp(a, destinationAngle, openSpeed * Time.deltaTime);
            if (rig == null)
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, a, transform.rotation.y));
            else {
                rig.MoveRotation(Quaternion.Euler(new Vector3(0, a, 0)));
            }
            yield return null;
        }
    }
}