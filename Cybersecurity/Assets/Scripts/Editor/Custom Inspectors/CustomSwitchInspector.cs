using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Switch))]
[CanEditMultipleObjects]
public class CustomSwitchInspector : CustomLevelObjectInspector
{
    //http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
    private ReorderableList m_LinkedLevelobjectsReorderableList;
    private ReorderableList m_RequiredItemsReorderableList;

    protected override void OnEnable()
    {
        base.OnEnable();

        CreateLinkedObjectList();
        CreateRequiredItemsList();
    }

    private void CreateLinkedObjectList()
    {
        m_LinkedLevelobjectsReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_LinkedLevelObjects"), true, false, true, true);

        //m_LinkedLevelobjectsReorderableList.drawHeaderCallback = (Rect rect) =>
        //{
        //    EditorGUI.LabelField(rect, "Linked LevelObjects");
        //};

        m_LinkedLevelobjectsReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = m_LinkedLevelobjectsReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        };
    }

    private void CreateRequiredItemsList()
    {
        m_RequiredItemsReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_RequiredItems"), true, false, true, true);

        //m_LinkedLevelobjectsReorderableList.drawHeaderCallback = (Rect rect) =>
        //{
        //    EditorGUI.LabelField(rect, "Linked LevelObjects");
        //};

        m_RequiredItemsReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = m_RequiredItemsReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, 65, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_Amount"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + 75, rect.y, rect.width - 75, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_Item"), GUIContent.none);
        };
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Switch", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        LayoutTriggers();
        EditorGUILayout.Space();

        LayoutLinkedObjects();
        EditorGUILayout.Space();

        LayoutRequiredItems();
        EditorGUILayout.Space();

        LayoutMaterials();

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();

        DrawMaterialInspectorGUI();
    }

    private void LayoutTriggers()
    {
        SerializedProperty serializedProperty = null;

        EditorGUILayout.LabelField("Triggers", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        serializedProperty = serializedObject.FindProperty("m_TriggerOnEnter");
        serializedProperty.boolValue = EditorGUILayout.Toggle("On Enter", serializedProperty.boolValue);

        serializedProperty = serializedObject.FindProperty("m_TriggerOnLeave");
        serializedProperty.boolValue = EditorGUILayout.Toggle("On Leave", serializedProperty.boolValue);

        serializedProperty = serializedObject.FindProperty("m_TriggerOnUse");
        serializedProperty.boolValue = EditorGUILayout.Toggle("On Use", serializedProperty.boolValue);

        serializedProperty = serializedObject.FindProperty("m_AutoInventory");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Auto Inventory"));

        EditorGUILayout.EndVertical();
    }

    private void LayoutLinkedObjects()
    {
        EditorGUILayout.LabelField("Linked Objects", EditorStyles.boldLabel);
        m_LinkedLevelobjectsReorderableList.DoLayoutList();
    }

    private void LayoutRequiredItems()
    {
        EditorGUILayout.LabelField("Required Items", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        SerializedProperty serializedProperty = serializedObject.FindProperty("m_ConsumeItems");
        serializedProperty.boolValue = EditorGUILayout.Toggle("Consume Items", serializedProperty.boolValue);

        m_RequiredItemsReorderableList.DoLayoutList();

        EditorGUILayout.EndVertical();
    }

    private void LayoutMaterials()
    {
        SerializedProperty serializedProperty = null;

        EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        serializedProperty = serializedObject.FindProperty("m_ColorModel");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Color Model"));

        serializedProperty = serializedObject.FindProperty("m_EnabledMaterial");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Enabled Material"));

        serializedProperty = serializedObject.FindProperty("m_DisabledMaterial");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Disabled Material"));

        EditorGUILayout.EndVertical();
    }
}