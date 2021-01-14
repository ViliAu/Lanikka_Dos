using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour {
    
    [SerializeField] private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    [SerializeField] private Dictionary<string, Entity> entityPrefabs = new Dictionary<string, Entity>();
    [SerializeField] private Dictionary<string, Sprite> crosshairs = new Dictionary<string, Sprite>();
    [SerializeField] private Dictionary<string, Sprite> icons = new Dictionary<string, Sprite>();

    [SerializeField] private List<SoundGroup> soundGroups = new List<SoundGroup>();

    private static Database _instance;
    public static Database Singleton {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType(typeof(Database)) as Database;
            }
            return _instance;
        }
        set {
            _instance = value;
        }
    }

    void Awake() {
        /* TODO : !!! This call should be moved somewhere else !!! */
        SoundSystem.Awake();
        AutomaticAssignerScript.AssignObjects();
    }

    public void AssingAudioClips(Object[] objs) {
        audioClips = new Dictionary<string, AudioClip>();
        int counter = 0;
        for (int i = 0; i < objs.Length; i++)  {
            audioClips.Add(objs[i].name, (objs[i] as AudioClip));
            counter++;
        }
        //Debug.Log("Succesfully assigned: " + counter + " audio files.");
    }

    public void AssignEntityPrefabs(Object[] objs) {
        entityPrefabs = new Dictionary<string, Entity>();
        int counter = 0;
        for (int i = 0; i < objs.Length; i++) {
            entityPrefabs.Add((objs[i] as Entity).entityName, (objs[i] as Entity));
            counter++;
        }
        //Debug.Log("Succesfully assigned: " + counter + " entities.");
    }

    public void AssignCrosshairs(Object[] objs) {
        crosshairs = new Dictionary<string, Sprite>();
        int counter = 0;
        for (int i = 0; i < objs.Length; i++) {
            crosshairs.Add((objs[i] as Sprite).name, (objs[i] as Sprite));
            counter++;
        }
        //Debug.Log("Succesfully assigned: " + counter + " entities.");
    }

    public void AssignIcons(Object[] objs) {
        icons = new Dictionary<string, Sprite>();
        int counter = 0;
        for (int i = 0; i < objs.Length; i++) {
            icons.Add((objs[i] as Sprite).name, (objs[i] as Sprite));
            counter++;
        }
        //Debug.Log("Succesfully assigned: " + counter + " entities.");
    }

    public void AssignSoundGroups(Object[] objs) {
        soundGroups = new List<SoundGroup>();
        int counter = 0;
        for (int i = 0; i < objs.Length; i++) {
            soundGroups.Add(objs[i] as SoundGroup);
            counter++;
        }
        //Debug.Log("Succesfully assigned: " + counter + " entities.");
    }

    public AudioClip GetAudioClip(string soundName) {
        AudioClip clip;
        audioClips.TryGetValue(soundName, out clip);
        return clip;
    }

    public Entity GetEntityPrefab(string entityName) {
        entityPrefabs.TryGetValue(entityName, out Entity entity);
        return entity;
    }

    public Sprite GetCrosshair(string crosshairName) {
        crosshairs.TryGetValue(crosshairName, out Sprite sprite);
        return sprite;
    }

    public Sprite GetIcon(string iconName) {
        icons.TryGetValue(iconName, out Sprite sprite);
        return sprite;
    }

    public SoundGroup[] GetSoundGroups() {
        return soundGroups.ToArray();
    }
}