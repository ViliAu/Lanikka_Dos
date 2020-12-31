using System.Collections.Generic;
using UnityEngine;

public class AutomaticAssignerScript : MonoBehaviour {

    public static void AssignObjects() {
        AssignAudioClips();
        AssignEntityPrefabs();
    }

    private static void AssignAudioClips() {
        Object[] objs = Resources.LoadAll("Audio", typeof(AudioClip));
        Database.Singleton.AssingAudioClips(objs);
    }

    private static void AssignEntityPrefabs() {
        Object[] objs = Resources.LoadAll("Prefabs", typeof(Entity));
        Database.Singleton.AssignEntityPrefabs(objs);
    }

}