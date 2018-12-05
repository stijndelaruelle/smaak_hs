using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Hacker))]
[CanEditMultipleObjects]
public class CustomHackerInspector : CustomCharacterInspector
{
    //http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
    private ReorderableList m_VisionRangeReorderableList;

    private void OnEnable()
    {
        m_VisionRangeReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_VisionRange"), true, false, true, true);

        //m_LinkedLevelobjectsReorderableList.drawHeaderCallback = (Rect rect) =>
        //{
        //    EditorGUI.LabelField(rect, "Linked LevelObjects");
        //};

        m_VisionRangeReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = m_VisionRangeReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        };
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SerializedProperty serializedProperty = null;

        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Hacker", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            serializedProperty = serializedObject.FindProperty("m_HackPawn");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Hack Pawn"));

            serializedProperty = serializedObject.FindProperty("m_Target");
            EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Target"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Vision", EditorStyles.boldLabel);

            serializedProperty = serializedObject.FindProperty("m_Direction");
            EditorGUILayout.PropertyField(serializedProperty, new GUIContent("Direction"));

            m_VisionRangeReorderableList.DoLayoutList();

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}