using UnityEngine;
using System.Collections;
using UnityEditor;

public class AlphaInverterEditor : EditorWindow {

    public Texture2D tex;
    private string message;
    private Coroutine msgCoroutine;
    private float msgStartLifetime = 2;
    private float msgLifetime;

    [MenuItem("Dossi/Alpha Inverter")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(AlphaInverterEditor));
    }

    void OnGUI() {
        tex = (Texture2D)EditorGUILayout.ObjectField(tex, typeof(Texture2D), allowSceneObjects: true);

        if (GUILayout.Button("Invert Alpha Channel")) {
            InvertChannel();
        }

        GUILayout.Label(message);

        msgLifetime -= Time.deltaTime;
        if (msgLifetime <= 0) {
            message = "";
        }
    }

    void InvertChannel() {
        // Check if a texture is assigned
        if (tex == null) {
            SetMessage("No texture assigned");
            return;
        }

        // Check if the texture is readable/writeable
        if (!tex.isReadable) {
            SetMessage("Please set the texture as readable!");
            return;
        }

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

    void SetMessage(string msg) {
        msgLifetime = msgStartLifetime;
        message = msg;
    }

}
