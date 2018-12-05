using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Inventory))]
[CanEditMultipleObjects]
public class CustomInventoryInspector : Editor
{
    //http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
    private ReorderableList m_ItemReorderableList;

    private void OnEnable()
    {
        m_ItemReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Items"), true, false, true, true);

        //m_LinkedLevelobjectsReorderableList.drawHeaderCallback = (Rect rect) =>
        //{
        //    EditorGUI.LabelField(rect, "Linked LevelObjects");
        //};

        m_ItemReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = m_ItemReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, 65, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_Amount"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + 75, rect.y, rect.width - 75, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_Item"), GUIContent.none);
        };
    }
    

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Items", EditorStyles.boldLabel);
        m_ItemReorderableList.DoLayoutList();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }

}