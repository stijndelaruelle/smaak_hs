using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Exit))]
[CanEditMultipleObjects]
public class CustomExitInspector : CustomLevelObjectInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Exit", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        SerializedProperty serializedProperty = null;

            serializedProperty = serializedObject.FindProperty("m_AllowedFaction");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Allowed Faction"));

            serializedProperty = serializedObject.FindProperty("m_ExitColorModel");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Color Model"));

            serializedProperty = serializedObject.FindProperty("m_ExitEnabledMaterial");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Enabled Material"));

            serializedProperty = serializedObject.FindProperty("m_ExitDisabledMaterial");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Disabled Material"));

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();

        //Draw materials
        DrawMaterialInspectorGUI();
    }
}