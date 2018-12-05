using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Character))]
[CanEditMultipleObjects]
public class CustomCharacterInspector : Editor
{
    public override void OnInspectorGUI()
    {
        SerializedProperty serializedProperty = null;

        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("General - Character (Required)", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            serializedProperty = serializedObject.FindProperty("m_StartNode");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Start Node"));

            serializedProperty = serializedObject.FindProperty("m_Faction");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Faction"));

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("General - Character (Optional)", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            serializedProperty = serializedObject.FindProperty("m_Inventory");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Inventory"));

            serializedProperty = serializedObject.FindProperty("m_Animator");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Animator"));

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}