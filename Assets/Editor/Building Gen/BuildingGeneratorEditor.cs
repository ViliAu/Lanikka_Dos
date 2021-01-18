using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingGenerator))]
public class BuildingGeneratorEditor : Editor {

    private BuildingGenerator gen;
    
    // Styles
    private GUIStyle buttonStyle;
    private GUIStyle titleStyle;
    private GUIStyle headerStyle;
    private float buttonWidth = 150;
    private int titleFontSize = 25;
    private int headerFontSize = 15;
    private float headerSpace = 15;

    // Colors
    private Color colBackground;
    private Color colDefaultBackground;
    private Color colGreen = Color.green;
    private Color colTitle = Color.yellow;
    private Color colHeader = Color.white;
    

    void OnEnable() {
    }

    void SetupStyles() {
        gen = target as BuildingGenerator;

        // Button
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedWidth = buttonWidth;
        buttonStyle.alignment = TextAnchor.MiddleCenter;
        colDefaultBackground = GUI.backgroundColor;

        // Title text
        titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = titleFontSize;
        titleStyle.fontStyle = FontStyle.Bold;

        // Header text
        headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontSize = headerFontSize;
        headerStyle.fontStyle = FontStyle.Bold;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        SetupStyles();

        // Title
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = colTitle;
        CenterElement();
        GUILayout.Label("Building Generator", titleStyle);
        GUILayout.EndHorizontal();

        GUILayout.Space(headerSpace);

        GUILayout.BeginHorizontal();
        GUI.backgroundColor = colHeader;
        CenterElement();
        GUILayout.Label("Generation", headerStyle);
        GUILayout.EndHorizontal();

        GUILayout.Space(headerSpace);

        // Generate button
        GUILayout.BeginHorizontal();
        CenterElement();
        GUI.backgroundColor = colGreen;
        if (GUILayout.Button("Generate Building", buttonStyle)) {
            gen.GenerateBuilding();
        }
        GUILayout.EndHorizontal();

        GUI.backgroundColor = colDefaultBackground;
    }

    void CenterElement() {
        GUILayout.Space(Screen.width / 2 - buttonWidth / 2);
    }
}
