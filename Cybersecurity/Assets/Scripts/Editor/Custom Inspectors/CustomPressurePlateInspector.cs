using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(PressurePlate))]
[CanEditMultipleObjects]
public class CustomPressurePlateInspector : CustomSwitchInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Pressure Plate", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        SerializedProperty serializedProperty = null;

        serializedProperty = serializedObject.FindProperty("m_Animator");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Animator"));

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}