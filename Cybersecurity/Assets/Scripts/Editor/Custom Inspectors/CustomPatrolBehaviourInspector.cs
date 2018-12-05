using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

[CustomEditor(typeof(PatrolBehaviour))]
[CanEditMultipleObjects]
public class CustomPatrolBehaviourInspector : Editor
{
    //http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
    private ReorderableList m_PathReorderableList;

    private void OnEnable()
    {
        CreateLinkedObjectList();
    }

    private void CreateLinkedObjectList()
    {
        m_PathReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Path"), true, false, true, true);

        //m_LinkedLevelobjectsReorderableList.drawHeaderCallback = (Rect rect) =>
        //{
        //    EditorGUI.LabelField(rect, "Linked LevelObjects");
        //};

        m_PathReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = m_PathReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        };
    }


    public override void OnInspectorGUI()
    {
        SerializedProperty serializedProperty = null;

        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        serializedProperty = serializedObject.FindProperty("m_LoopType");
        serializedProperty.enumValueIndex = (int)(PatrolBehaviour.LoopType)EditorGUILayout.EnumPopup("Loop Type", (PatrolBehaviour.LoopType)Enum.GetValues(typeof(PatrolBehaviour.LoopType)).GetValue(serializedProperty.enumValueIndex));

        serializedProperty = serializedObject.FindProperty("m_ReverseWhenStuck");
        serializedProperty.boolValue = EditorGUILayout.Toggle("Reverse When Stuck", serializedProperty.boolValue);

        EditorGUILayout.LabelField("Arrow", EditorStyles.boldLabel);

        serializedProperty = serializedObject.FindProperty("m_ArrowSprite");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Arrow Sprite"));

        serializedProperty = serializedObject.FindProperty("m_ArrowColor");
        serializedProperty.colorValue = EditorGUILayout.ColorField("Arrow Color", serializedProperty.colorValue);

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Path", EditorStyles.boldLabel);
        m_PathReorderableList.DoLayoutList();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}