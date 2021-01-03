using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour {

    private static Player p;
    public static Player Player {
        get {
            if (p == null) {
                p = FindObjectOfType<Player>();
            }
            if (p == null) {
                Debug.LogError("No Player found...");
            }
            return p;
        }
    }

    private static DookerPen dp;
    public static DookerPen DookerPen {
        get {
            if (dp == null) {
                dp = FindObjectOfType<DookerPen>();
            }
            if (dp == null) {
                Debug.LogError("No Dooker Pen found...");
            }
            return dp;
        }
    }
}