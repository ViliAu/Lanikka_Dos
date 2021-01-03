using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    [SerializeField] private RectTransform canvas = null;
    private Image crosshair = null;

    private void Awake() {
        // Find canvas
        if (canvas == null)
            if ((canvas = EntityManager.Player.transform.Find("Head").Find("Main camera").Find("Canvas").GetComponent<RectTransform>()) == null)
                Debug.LogError("Couldn't find player canvas");

        // Find crosshair
        if (crosshair == null)
            if ((crosshair = canvas.Find("Crosshair").GetComponent<Image>()) == null)
                Debug.LogError("Couldn't find player crosshair");
    }

    public void ChangeCrosshairDarkness(float darkness) {
        if (crosshair.color.r != darkness)
            crosshair.color = new Color(darkness, darkness, darkness);
    }
}
