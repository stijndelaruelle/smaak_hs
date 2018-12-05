using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(OldCameraMoveTrigger))]
[CanEditMultipleObjects]
public class OldCustomCameraMoveTriggerInspector : CustomLevelObjectInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Camera Move Trigger", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        SerializedProperty serializedProperty = null;

        serializedProperty = serializedObject.FindProperty("m_OriginalCamera");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Original Camera"));

        serializedProperty = serializedObject.FindProperty("m_NewCamera");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("New Camera"));

        serializedProperty = serializedObject.FindProperty("m_AllowedFaction");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Allowed Faction"));

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("NOTE: Will only work properly when the player can only move in 1 direction!");
        //And I currently can't afford to spend any more time on this feature

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}