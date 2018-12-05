using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DarkHacker))]
[CanEditMultipleObjects]
public class CustomDarkHackerInspector : CustomHackerInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SerializedProperty serializedProperty = null;

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Dark Hacker", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            serializedProperty = serializedObject.FindProperty("m_LaughAtStart");
            serializedProperty.boolValue = EditorGUILayout.Toggle("Laugh At Start", serializedProperty.boolValue);

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}
