using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(DatablockPickup))]
[CanEditMultipleObjects]
public class CustomDatablockPickupInspector : CustomPickupInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Datablock Pickup", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            SerializedProperty serializedProperty = null;

            serializedProperty = serializedObject.FindProperty("m_QuestionPool");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Question Pool"));

            serializedProperty = serializedObject.FindProperty("m_Animator");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Animator"));

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}