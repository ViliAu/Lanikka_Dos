using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingGenerator)), CanEditMultipleObjects]
public class BuildingGeneratorEditor : Editor {

    private BuildingGenerator gen;
    private int tabIndex = 0;
    
    // Styles
    private GUIStyle buttonStyle;
    private GUIStyle titleStyle;
    private GUIStyle headerStyle;
    private GUIStyle tabStyle;

    // Style vars
    private float buttonWidth = 150;
    private int titleFontSize = 25;
    private int headerFontSize = 15;
    private float headerSpace = 15;

    // Colors
    private Color colBackground;
    private Color colDefaultBackground;
    private Color colGreen = Color.green;
    private Color colTitle = Color.gray;
    private Color colHeader = Color.white;

    void OnEnable() {
        gen = target as BuildingGenerator;
        gen.LoadDefaultAssets();
    }

    void SetupStyles() {
        // Tab
        tabStyle = new GUIStyle(GUI.skin.button);
        tabStyle.fixedWidth = 40;
        tabStyle.fixedHeight = 40;
        tabStyle.alignment = TextAnchor.MiddleCenter;

        // Button
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedWidth = buttonWidth;
        buttonStyle.alignment = TextAnchor.MiddleCenter;

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

        // Defaults
        colDefaultBackground = GUI.backgroundColor;
    }

    public override void OnInspectorGUI() {
        SetupStyles();

        // Title
        GUILayout.Space(headerSpace);
        DrawTitle();
        GUILayout.Space(headerSpace);
        DrawTabButtons();
        GUILayout.Space(headerSpace);
        DrawSelectedTab();
        GUILayout.Space(headerSpace);

        // Update buiding
        gen.BuildingModified();
    }

    void DrawTitle() {
        GUI.backgroundColor = colTitle;
        GUILayout.Label("Building Generator", titleStyle);
    }

    void DrawTabButtons() {
        GUI.backgroundColor = colDefaultBackground;

        GUILayout.BeginHorizontal();
        CenterElement(tabStyle.fixedWidth * 2 + tabStyle.fixedWidth);
        if (GUILayout.Button("", tabStyle)) {
            tabIndex = 0;
        }
        if (GUILayout.Button("", tabStyle)) {
            tabIndex = 1;
        }
        GUILayout.EndHorizontal();
    }

    void DrawSelectedTab() {
        switch(tabIndex) {
            case 0:
                DrawPointTab();
                break;
            case 1:
                DrawPrefabTab();
                break;
        }
    }

    void DrawPointTab() {
        //     
        gen.buildingHeight = EditorGUILayout.IntField("Building Height", gen.buildingHeight);

        // Reset points button
        GUILayout.BeginHorizontal();
        CenterElement(buttonStyle.fixedWidth);
        if (GUILayout.Button("Reset Points", buttonStyle)) {
            gen.ResetControlPoints();
        }
        GUILayout.EndHorizontal();
    }

    void DrawPrefabTab() {

    }

    void CenterElement(float elementWidth) {
        GUILayout.Space(Screen.width / 2 - elementWidth / 2);
    }

    #region SCENE_GUI

    protected virtual void OnSceneGUI() {
        DrawControlPoints();
        DrawAreaLines();
    }

    void DrawControlPoints() {
        Vector3 pos;
        float handleSize = 0.5f;
        float pickSize = handleSize * 2;

        EditorGUI.BeginChangeCheck();
        Handles.color = Color.green;
        for (int i = 0; i < gen.controlPoints.Length; i++) {
            pos = DUtil.Vec2ToVec3XZ(gen.controlPoints[i]) + gen.transform.position;
            pos = Handles.FreeMoveHandle(pos, Quaternion.identity, handleSize, Vector3.zero, Handles.SphereHandleCap);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(gen, "Changed some");
                gen.controlPoints[i] = DUtil.Vec3ToVec2XZ(pos - gen.transform.position);
            }
        }
    }

    void DrawAreaLines() {
        Handles.color = Color.yellow;
        Handles.DrawDottedLines(
            new Vector3[] {
                DUtil.Vec2ToVec3XZ(gen.controlPoints[0]) + gen.transform.position,
                DUtil.Vec2ToVec3XZ(gen.controlPoints[1]) + gen.transform.position,

                DUtil.Vec2ToVec3XZ(gen.controlPoints[1]) + gen.transform.position,
                DUtil.Vec2ToVec3XZ(gen.controlPoints[2]) + gen.transform.position,

                DUtil.Vec2ToVec3XZ(gen.controlPoints[2]) + gen.transform.position,
                DUtil.Vec2ToVec3XZ(gen.controlPoints[3]) + gen.transform.position,

                DUtil.Vec2ToVec3XZ(gen.controlPoints[3]) + gen.transform.position,
                DUtil.Vec2ToVec3XZ(gen.controlPoints[0]) + gen.transform.position,
            }, 
            10f
        );
    }

    #endregion
    
}
