using System.Collections.Generic;
using UnityEngine;

public class AutomaticAssignerScript : MonoBehaviour {

    public static void AssignObjects() {
        AssignAudioClips();
        AssignEntityPrefabs();
        AssignCrosshairs();
        AssignIcons();
        AssignSoundGroups();
    }

    private static void AssignAudioClips() {
        Object[] objs = Resources.LoadAll("Audio", typeof(AudioClip));
        Database.Singleton.AssingAudioClips(objs);
    }

    private static void AssignEntityPrefabs() {
        Object[] objs = Resources.LoadAll("Prefabs", typeof(Entity));
        Database.Singleton.AssignEntityPrefabs(objs);
    }

    private static void AssignCrosshairs() {
         Object[] objs = Resources.LoadAll("Textures/UI/Crosshair", typeof(Sprite));
         Database.Singleton.AssignCrosshairs(objs);
    }

    private static void AssignIcons() {
         Object[] objs = Resources.LoadAll("Textures/UI/Icons/Generated", typeof(Sprite));
         Database.Singleton.AssignIcons(objs);
    }

    private static void AssignSoundGroups() {
        Object[] objs = Resources.LoadAll("ScriptableObjects", typeof(SoundGroup));
        Database.Singleton.AssignSoundGroups(objs);
    }

}