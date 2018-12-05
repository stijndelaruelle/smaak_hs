using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(HackPawn))]
[CanEditMultipleObjects]
public class CustomHackPawnInspector : CustomCharacterInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SerializedProperty serializedProperty = null;

        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Hack Pawn", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            serializedProperty = serializedObject.FindProperty("m_PatrolBehaviour");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Patrol Behaviour"));

            serializedProperty = serializedObject.FindProperty("m_ChaseBehaviour");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Chase Behaviour"));

            serializedProperty = serializedObject.FindProperty("m_HasHacker");
            serializedProperty.boolValue = EditorGUILayout.Toggle("Has Hacker", serializedProperty.boolValue);

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}