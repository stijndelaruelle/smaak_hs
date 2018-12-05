using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(DialogueTrigger))]
[CanEditMultipleObjects]
public class CustomDialogueTriggerInspector : CustomLevelObjectInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Dialogue Trigger", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        SerializedProperty serializedProperty = null;

        serializedProperty = serializedObject.FindProperty("m_Dialogue");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Dialogue"));

        serializedProperty = serializedObject.FindProperty("m_AllowedFaction");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Allowed Faction"));

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}