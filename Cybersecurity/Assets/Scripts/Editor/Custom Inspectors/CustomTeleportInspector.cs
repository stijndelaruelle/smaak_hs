using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Teleport))]
[CanEditMultipleObjects]
public class CustomTeleportInspector : CustomLevelObjectInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Teleport", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        SerializedProperty serializedProperty = null;

        serializedProperty = serializedObject.FindProperty("m_TargetNode");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Target Node"));

        serializedProperty = serializedObject.FindProperty("m_AllowedFaction");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Allowed Faction"));

        serializedProperty = serializedObject.FindProperty("m_Animator");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Animator"));

        serializedProperty = serializedObject.FindProperty("m_ColorModel");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Color Model"));

        serializedProperty = serializedObject.FindProperty("m_EnabledMaterial");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Enabled Material"));

        serializedProperty = serializedObject.FindProperty("m_DisabledMaterial");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Disabled Material"));

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();

        DrawMaterialInspectorGUI();
    }
}