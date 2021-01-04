using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour {

    [SerializeField] private RectTransform canvas = null;
    private Image crosshair = null;
    private TMP_Text moneyAmount = null;
    private TMP_Text focusText = null;

    private void Awake() {
        // Find canvas
        if (canvas == null)
            if ((canvas = EntityManager.Player.transform.Find("Head").Find("Main camera").Find("Canvas").GetComponent<RectTransform>()) == null)
                Debug.LogError("Couldn't find player canvas");

        // Find crosshair
        if (crosshair == null)
            if ((crosshair = canvas.Find("Crosshair").GetComponent<Image>()) == null)
                Debug.LogError("Couldn't find player crosshair");

        // Find moeny amount
        if (moneyAmount == null)
            if ((moneyAmount = canvas.Find("MoneyAmount").GetComponent<TMP_Text>()) == null)
                Debug.LogError("Couldn't find money text");

        // Find focus text
        if (focusText == null)
            if ((focusText = canvas.Find("FocusText").GetComponent<TMP_Text>()) == null)
                Debug.LogError("Couldn't find focus text");
    }

    public void ChangeCrosshairDarkness(float darkness) {
        if (crosshair.color.r != darkness)
            crosshair.color = new Color(darkness, darkness, darkness);
    }

    public void ChangeCrosshair(string name) {
        Sprite sprite = Database.Singleton.GetCrosshair(name);
        if (sprite != null && crosshair.sprite != sprite) {
            crosshair.sprite = sprite;
        }
    }

    public void ChangeMoneyAmount(float amount) {
        if (moneyAmount != null)
            moneyAmount.text = amount.ToString() + "$";
    }

    public void ChangeFocusText(string s) {
        if (focusText != null)
            focusText.text = s;
    }
}
