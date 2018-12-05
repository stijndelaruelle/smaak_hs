using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(QuestionDefinition))]
[CanEditMultipleObjects]
public class CustomQuestionDefinitionInspector : Editor
{
    //http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
    private ReorderableList m_AnswerReorderableList;

    private void OnEnable()
    {
        m_AnswerReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Answers"), true, false, true, true);

        //m_LevelObjectReorderableList.drawHeaderCallback = (Rect rect) =>
        //{
        //    EditorGUI.LabelField(rect, "Level Objects");
        //};

        m_AnswerReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = m_AnswerReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, EditorGUIUtility.singleLineHeight), new GUIContent("Correct"));
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, rect.width - 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_IsCorrect"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + 60, rect.y, rect.width - 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_Text"), GUIContent.none); //+ EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing
        };

        m_AnswerReorderableList.elementHeightCallback = (int index) =>
        {
            return 75;
        };
    }

    public override void OnInspectorGUI()
    {
        SerializedProperty serializedProperty = null;

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Question", EditorStyles.boldLabel);

        //EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        //Question
        serializedProperty = serializedObject.FindProperty("m_Question");
        serializedProperty.stringValue = EditorGUILayout.TextField("Localization ID", serializedProperty.stringValue);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Localized Text: " + LocalizationManager.GetText(serializedProperty.stringValue, LocalizationManager.Language.Dutch));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        //Explanation
        EditorGUILayout.LabelField("Correct Explanation", EditorStyles.boldLabel);

        serializedProperty = serializedObject.FindProperty("m_CorrectExplanation");
        serializedProperty.stringValue = EditorGUILayout.TextField("Localization ID", serializedProperty.stringValue);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Localized Text: " + LocalizationManager.GetText(serializedProperty.stringValue, LocalizationManager.Language.Dutch));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        //Explanation
        EditorGUILayout.LabelField("Incorrect Explanation", EditorStyles.boldLabel);

        serializedProperty = serializedObject.FindProperty("m_IncorrectExplanation");
        serializedProperty.stringValue = EditorGUILayout.TextField("Localization ID", serializedProperty.stringValue);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Localized Text: " + LocalizationManager.GetText(serializedProperty.stringValue, LocalizationManager.Language.Dutch));
        EditorGUILayout.EndVertical();


        //Level Objects
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Answers", EditorStyles.boldLabel);
        m_AnswerReorderableList.DoLayoutList();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}