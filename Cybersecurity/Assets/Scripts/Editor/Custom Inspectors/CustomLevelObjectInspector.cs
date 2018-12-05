using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(LevelObject))]
[CanEditMultipleObjects]
public class CustomLevelObjectInspector : Editor
{
    //http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
    private ReorderableList m_LevelObjectReorderableList;

    protected virtual void OnEnable()
    {
        m_LevelObjectReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Renderers"), true, false, true, true);

        //m_LevelObjectReorderableList.drawHeaderCallback = (Rect rect) =>
        //{
        //    EditorGUI.LabelField(rect, "Level Objects");
        //};

        m_LevelObjectReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = m_LevelObjectReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_Renderer"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + rect.width - 90, rect.y, 90, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_Material"), GUIContent.none);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Color prevColor = GUI.backgroundColor;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("General - LevelObject", EditorStyles.boldLabel);

        SerializedProperty serializedProperty = serializedObject.FindProperty("m_IsEnabled");

        //EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.fixedHeight = 30;

            string buttonText = "";
            if (serializedProperty.boolValue == false)
            {
                buttonText = "Disabled";
                GUI.backgroundColor = new Color(1.0f, 0.75f, 0.75f);
            }
            else
            {
                buttonText = "Enabled";
                GUI.backgroundColor = new Color(0.75f, 1.0f, 0.75f);
            }

            if (GUILayout.Button(buttonText, style)) { ToggleEnabled(serializedProperty); }

            GUI.backgroundColor = prevColor;

        //EditorGUILayout.EndVertical();
        
        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }

    protected void DrawMaterialInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Level Wizard - Materials", EditorStyles.boldLabel);

        if (m_LevelObjectReorderableList != null)
        m_LevelObjectReorderableList.DoLayoutList();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }

    private void ToggleEnabled(SerializedProperty serializedProperty)
    {
        serializedProperty.boolValue = !serializedProperty.boolValue;
    }
}