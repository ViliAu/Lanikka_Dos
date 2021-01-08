using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBox : Interactable {

    public override void PlayerInteract() {
        base.PlayerInteract();

        AudioSource source = GetComponent<AudioSource>();
        if (source.isPlaying) {
            source.Stop();
        }
        else {
            source.Play();
        }
    }

}
