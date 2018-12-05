using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(CharacterTypeDefinition))]
[CanEditMultipleObjects]
public class CustomCharacterTypeDefinitionInspector : Editor
{
    public override void OnInspectorGUI()
    {
        SerializedProperty serializedProperty = null;

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            serializedProperty = serializedObject.FindProperty("m_NameLocalizationID");
            serializedProperty.stringValue = EditorGUILayout.TextField("Name Localization ID", serializedProperty.stringValue);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Localized Text: " + LocalizationManager.GetText(serializedProperty.stringValue, LocalizationManager.Language.Dutch));
            EditorGUILayout.EndVertical();

            serializedProperty = serializedObject.FindProperty("m_DialogueSprite");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Dialogue Sprite"));

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Text", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            serializedProperty = serializedObject.FindProperty("m_TextFont");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Font"));

            serializedProperty = serializedObject.FindProperty("m_TextColor");
            serializedProperty.colorValue = EditorGUILayout.ColorField("Color", serializedProperty.colorValue);

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}