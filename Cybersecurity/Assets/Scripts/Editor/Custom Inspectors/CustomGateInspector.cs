using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Gate))]
[CanEditMultipleObjects]
public class CustomGateInspector : CustomLevelObjectInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Gate", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        SerializedProperty serializedProperty = null;

        serializedProperty = serializedObject.FindProperty("m_InverseAfterPassTrough");
        serializedProperty.boolValue = EditorGUILayout.Toggle("Inverse After Passtrough", serializedProperty.boolValue);

        serializedProperty = serializedObject.FindProperty("m_Animator");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Animator"));

        serializedProperty = serializedObject.FindProperty("m_OpenSoundEffect");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Open Sound Effect"));

        serializedProperty = serializedObject.FindProperty("m_CloseSoundEffect");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Close Sound Effect"));

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();

        DrawMaterialInspectorGUI();
    }
}