using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DropTableComponent))]
public class DropTableEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        DropTableComponent table = target as DropTableComponent;
        if (table != null) {

            // Draw drop tables
            DrawDropTables();

            if (GUILayout.Button("Add Table")) {
                table.dropTable.Add(new DropTable());
            }

            if (GUILayout.Button("Remove Table")) {
                if (table.dropTable.Count > 0)
                    table.dropTable.RemoveAt(0);
            }

            if (GUILayout.Button("Test Roll")) {
                if (table.dropTable.Count > 0)
                    table.dropTable[0].SetupRoll();
            }
        }
    }

    void DrawDropTables() {
        
        SerializedProperty prop = serializedObject.FindProperty("dropTable");
        for (int i = 0; i < prop.arraySize; i++) {
            SerializedProperty prop3 = prop.GetArrayElementAtIndex(i);
            while (true) {
                EditorGUILayout.PropertyField(prop3);

                bool hasNext = prop3.NextVisible(prop3.isExpanded);
                if (!hasNext) {
                    break;
                }
            }

            prop3 = prop.GetArrayElementAtIndex(i).FindPropertyRelative("prefabs");

            if (GUILayout.Button("Add prefab")) {
                prop3.InsertArrayElementAtIndex(prop3.arraySize);
            }
            serializedObject.ApplyModifiedProperties();
        }
        
        //SerializedProperty prop = serializedObject.FindProperty("dropTable");
        //EditorGUILayout.PropertyField(prop, true);

    }
}
