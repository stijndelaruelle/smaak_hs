using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Pickup))]
[CanEditMultipleObjects]
public class CustomPickupInspector : CustomLevelObjectInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Pickup", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            SerializedProperty serializedProperty = null;

            serializedProperty = serializedObject.FindProperty("m_Item");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Item"));

            serializedProperty = serializedObject.FindProperty("m_Amount");
            serializedProperty.intValue = EditorGUILayout.IntField("Amount", serializedProperty.intValue);

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}