using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(HintSystem))]
[CanEditMultipleObjects]
public class CustomHintSystemInspector : Editor
{
    //http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
    private ReorderableList m_HintsReorderableList;

    private void OnEnable()
    {
        HintSystem hintSystem = (HintSystem)target;

        m_HintsReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Hints"), true, false, true, true);

        //m_LevelObjectReorderableList.drawHeaderCallback = (Rect rect) =>
        //{
        //    EditorGUI.LabelField(rect, "Level Objects");
        //};

        m_HintsReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = m_HintsReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_HintableObject"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, 75, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_SideOfObject"), GUIContent.none);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(new Rect(rect.x + 75, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, rect.width - 75, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_Offset"), GUIContent.none);

            if (EditorGUI.EndChangeCheck())
            {
                hintSystem.MoveHintPointerEditor((HintSystem.Hint.Side)element.FindPropertyRelative("m_SideOfObject").enumValueIndex,
                                                 ((GameObject)element.FindPropertyRelative("m_HintableObject").objectReferenceValue).transform,
                                                  element.FindPropertyRelative("m_Offset").vector3Value);
            }
        };

        m_HintsReorderableList.elementHeightCallback = (int index) =>
        {
            return 40; //return ((EditorGUIUtility.singleLineHeight * 2) + (EditorGUIUtility.standardVerticalSpacing * 3));
        };
    }

    public override void OnInspectorGUI()
    {
        HintSystem hintSystem = (HintSystem)target;

        serializedObject.Update();

        //Neighbours
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Hint System", EditorStyles.boldLabel);

        SerializedProperty serializedProperty = serializedObject.FindProperty("m_AllowedFaction");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Allowed Faction"));

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            serializedProperty = serializedObject.FindProperty("m_HintPointers");
            for (int i = 0; i < serializedProperty.arraySize; ++i)
            {
                SerializedProperty subSerializedProperty = serializedProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.ObjectField(subSerializedProperty, new GUIContent(((HintSystem.Hint.Side)i).ToString()));
            }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        //Level Objects
        EditorGUILayout.LabelField("Hints", EditorStyles.boldLabel);
        m_HintsReorderableList.DoLayoutList();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Top"))    { hintSystem.ToggleHintPointer(HintSystem.Hint.Side.Top); }
        if (GUILayout.Button("Bottom")) { hintSystem.ToggleHintPointer(HintSystem.Hint.Side.Bottom); }
        if (GUILayout.Button("Left"))   { hintSystem.ToggleHintPointer(HintSystem.Hint.Side.Left); }
        if (GUILayout.Button("Right"))  { hintSystem.ToggleHintPointer(HintSystem.Hint.Side.Right); }

        GUILayout.EndHorizontal();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}
