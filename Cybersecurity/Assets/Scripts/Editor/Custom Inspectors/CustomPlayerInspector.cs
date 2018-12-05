using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Player))]
[CanEditMultipleObjects]
public class CustomHackPlayerInspector : CustomCharacterInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SerializedProperty serializedProperty = null;

        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Player", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            serializedProperty = serializedObject.FindProperty("m_StartDirection");
            EditorGUILayout.PropertyField(serializedProperty);

            serializedProperty = serializedObject.FindProperty("m_MinIdleVariationTime");
            serializedProperty.floatValue = EditorGUILayout.FloatField("Min. Idle Variation Time", serializedProperty.floatValue);

            serializedProperty = serializedObject.FindProperty("m_MaxIdleVariationTime");
            serializedProperty.floatValue = EditorGUILayout.FloatField("Max. Idle Variation Time", serializedProperty.floatValue);

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}