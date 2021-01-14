using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour {

    [SerializeField] private RectTransform canvas = null;
    [SerializeField] Image heldItemImage = null;
    [SerializeField] TMP_Text heldItemText = null;
    [SerializeField] private Image crosshair = null;
    [SerializeField] private TMP_Text moneyAmount = null;
    [SerializeField] private TMP_Text focusText = null;

    private void Awake() {
        // warningit kohillee
        if (canvas == null || heldItemImage == null || heldItemText == null || crosshair == null || moneyAmount == null || focusText == null) {
            Debug.LogError("Check player UI dependencies or revert prefab to fix UI issues.");
        }
    }

    public void SetCrosshairDarkness(float darkness) {
        if (crosshair.color.r != darkness)
            crosshair.color = new Color(darkness, darkness, darkness);
    }

    public void SetCrosshair(string name) {
        Sprite sprite = Database.Singleton.GetCrosshair(name);
        if (sprite != null && crosshair.sprite != sprite) {
            crosshair.sprite = sprite;
        }
    }

    public string GetCrosshairName() {
        return crosshair.sprite.name;
    }

    public void SetMoneyAmount(float amount) {
        if (moneyAmount != null)
            moneyAmount.text = amount.ToString() + "$";
    }

    public void SetFocusText(string s) {
        if (focusText != null)
            focusText.text = s;
    }

    public void SetHeldItem(Sprite sprite, string str) {
        heldItemImage.sprite = sprite;
        heldItemText.text = str;
        heldItemImage.gameObject.SetActive(sprite != null);
    }
}
