using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(DialogueTypeDefinition))]
[CanEditMultipleObjects]
public class CustomDialogueTypeDefinitionInspector : Editor
{
    //http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
    private ReorderableList m_DialogueLineReorderableList;

    private void OnEnable()
    {
        m_DialogueLineReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_DialogueLines"), true, false, true, true);

        //m_LevelObjectReorderableList.drawHeaderCallback = (Rect rect) =>
        //{
        //    EditorGUI.LabelField(rect, "Level Objects");
        //};

        m_DialogueLineReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = m_DialogueLineReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, 65, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_SideOfPanel"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + 75, rect.y, rect.width - 75, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_Character"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_TextLocalizationID"), GUIContent.none);
        };

        m_DialogueLineReorderableList.elementHeightCallback = (int index) =>
        {
            return 75;
        };

        m_DialogueLineReorderableList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = m_DialogueLineReorderableList.serializedProperty.GetArrayElementAtIndex(index);

            if (element == null)
                return;

            SerializedProperty characterProperty = element.FindPropertyRelative("m_Character");

            if (characterProperty.objectReferenceValue == null)
                return;

            SerializedObject character = new SerializedObject(characterProperty.objectReferenceValue);

            if (character == null)
                return;

            SerializedProperty color = character.FindProperty("m_TextColor");

            //Feels so dirty...
            //https://pastebin.com/WhfRgcdC
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color.colorValue);
            tex.Apply();

            GUI.DrawTexture(rect, tex as Texture);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Level Objects
        EditorGUILayout.LabelField("Dialogue", EditorStyles.boldLabel);
        m_DialogueLineReorderableList.DoLayoutList();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}