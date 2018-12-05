using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(TimelineTrigger))]
[CanEditMultipleObjects]
public class CustomTimelineTriggerInspector : CustomLevelObjectInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Timeline Trigger", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        SerializedProperty serializedProperty = null;

        serializedProperty = serializedObject.FindProperty("m_AllowedFaction");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Allowed Faction"));

        serializedProperty = serializedObject.FindProperty("m_PlayableDirector");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Playable Director"));

        serializedProperty = serializedObject.FindProperty("m_TriggerOnEnter");
        serializedProperty.boolValue = EditorGUILayout.Toggle("Trigger On Enter", serializedProperty.boolValue);

        serializedProperty = serializedObject.FindProperty("m_TriggerOnLeave");
        serializedProperty.boolValue = EditorGUILayout.Toggle("Trigger On Leave", serializedProperty.boolValue);

        serializedProperty = serializedObject.FindProperty("m_DisableAfterTrigger");
        serializedProperty.boolValue = EditorGUILayout.Toggle("Disable After Trigger", serializedProperty.boolValue);

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}