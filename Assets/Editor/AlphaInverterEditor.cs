using UnityEngine;
using System.Collections;
using UnityEditor;

public class AlphaInverterEditor : EditorWindow {

    private Texture2D tex;
    private Texture2D prevTex;
    private string messageText;
    private string headerText;
    private Coroutine msgCoroutine;
    private float msgStartLifetime = 2;
    private float msgLifetime;

    // Stylesd
    private GUIStyle buttonStyle;
    private GUIStyle headerStyle;
    private GUIStyle msgStyle;
    private GUIStyle texStyle;

    private GameObject model;
    private Editor modelEditor;
    
    // Colors
    private Color colDefaultButtonBackground;

    [MenuItem("Dossi/Alpha Inverter")]
    public static void ShowWindow() {
        EditorWindow.GetWindowWithRect<AlphaInverterEditor>(new Rect(0, 0, 300, 800));
    }

    void SetupStyles() {
        colDefaultButtonBackground = GUI.backgroundColor;
        int fixedWidth = 250;

        // Button
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedWidth = fixedWidth;
        buttonStyle.fixedHeight =  35;
        buttonStyle.fontSize = 16;
        buttonStyle.alignment = TextAnchor.MiddleCenter;

        // Title label
        headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontSize = 25;
        headerStyle.fontStyle = FontStyle.Bold;

        // Message label
        msgStyle = new GUIStyle(GUI.skin.label);
        msgStyle.alignment = TextAnchor.MiddleCenter;
        msgStyle.fontSize = 15;

        // Texture style
        texStyle = new GUIStyle(GUI.skin.button);
        texStyle.alignment = TextAnchor.MiddleCenter;
        texStyle.fixedWidth = fixedWidth;
        texStyle.fixedHeight = fixedWidth;
    }

    void OnGUI() {
        SetupStyles();
        UpdateHeaderText();
        UpdateMessageText();
        float spacing = 15f;

        // Title
        GUILayout.Space(spacing);
        GUILayout.Label(headerText, headerStyle);

        GUILayout.Space(spacing);

        // Texture button
        GUILayout.BeginHorizontal();
        GUILayout.Space(Screen.width / 2 - 250 / 2);
        if (GUILayout.Button(prevTex, texStyle)) {
            EditorGUIUtility.ShowObjectPicker<Texture2D>(null, false, "", 69);
        }
        GUILayout.EndHorizontal();

        // Update selected texture
        if (Event.current.commandName == "ObjectSelectorUpdated") {
            tex = EditorGUIUtility.GetObjectPickerObject() as Texture2D;
        }
        UpdatePreviewTexture();       

        GUILayout.Space(spacing);

        // Disable invert button if no texture is selected
        if (tex == null || (tex != null && !tex.isReadable) || IsCompressed()) {
            GUI.enabled = false;
        }

        // Invert button
        GUILayout.BeginHorizontal();
        CenterButton();
        if (GUILayout.Button("Invert Alpha Channel", buttonStyle)) {
            InvertChannel();
        }
        GUILayout.EndHorizontal();
        GUI.enabled = true;

        GUILayout.Space(spacing);

        // Message label
        GUILayout.Label(messageText, msgStyle);
    }

    void InvertChannel() {
        // Invert alpha colors
        Color[] pixels = tex.GetPixels();
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i].a = 1 - pixels[i].a;
        }

        // Save inverted colors
        tex.SetPixels(pixels);
        tex.Apply();

        // Override old texture with the inverted one
        string path = AssetDatabase.GetAssetPath(tex);
        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        
        // Refresh database
        AssetDatabase.Refresh();
    }

    void UpdatePreviewTexture() {
        if (tex == null) {
            prevTex = null;
            return;
        }

        // Make grayscale preview texture from selected textures alpha
        // if it's marked as readable 
        if (tex.isReadable) {
            prevTex = new Texture2D(tex.width, tex.height);
            Color[] pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i++) {
                pixels[i].r = pixels[i].a;
                pixels[i].g = pixels[i].a;
                pixels[i].b = pixels[i].a;
            }

            prevTex.SetPixels(pixels);
            prevTex.Apply();
        }

        // Otherwise just use the preview texture
        else {
            prevTex = tex;
        }
    }

    void UpdateHeaderText() {
        if (tex == null) {
            headerText = "Select Texture";
        }
        else if (!tex.isReadable) {
            headerText = "Previewing RGB";
        }
        else {
            headerText = "Previewing Alpha";
        }
    }

    void UpdateMessageText() {
        if (tex == null) {
            messageText = "";
        }

        else if (!tex.isReadable && IsCompressed()) {
            messageText = "Set the texture as readable/writeable.\n" +
                           "and texture compression to none.";
        }

        else if (!tex.isReadable) {
            messageText = "Set the texture as readable/writeable.";
        }
        
        else if (IsCompressed()) {
            messageText = "Set the texture compression to none.";
        }

        else {
            messageText = "";
        }
    }

    bool IsCompressed() {
        if (tex.format == TextureFormat.RGB24 ||
            tex.format == TextureFormat.RGB48 ||
            tex.format == TextureFormat.RGBA32 ||
            tex.format == TextureFormat.RGBA64) {
            return false;
        }

                        
        return true;
    }

    void SetMessage(string msg) {
        msgLifetime = msgStartLifetime;
        messageText = msg;
    }

    void CenterButton() {
        GUILayout.Space(Screen.width / 2 - buttonStyle.fixedWidth / 2);
    }
}
