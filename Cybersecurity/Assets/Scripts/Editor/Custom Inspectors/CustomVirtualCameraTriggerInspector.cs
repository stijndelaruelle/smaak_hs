using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(VirtualCameraTrigger))]
[CanEditMultipleObjects]
public class CustomVirtualCameraTriggerInspector : CustomLevelObjectInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Virtual Camera Trigger", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        SerializedProperty serializedProperty = null;

        serializedProperty = serializedObject.FindProperty("m_VirtualCamera");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Virtual Camera"));

        serializedProperty = serializedObject.FindProperty("m_AllowedFaction");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Allowed Faction"));

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}