using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

[CustomEditor(typeof(Node))]
[CanEditMultipleObjects]
public class CustomNodeInspector : Editor
{
    //http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
    private ReorderableList m_LevelObjectReorderableList;

    private LevelEditorSettings m_LevelEditorSettings;

    private void OnEnable()
    {
        //Load the settings (may end up in a different scriptable object at some point)
        m_LevelEditorSettings = (LevelEditorSettings)Resources.Load("LevelEditorSettings", typeof(LevelEditorSettings));

        if (m_LevelEditorSettings == null)
        {
            Debug.LogError("No Level editor settings found, using a temporary one to avoid crashes!");
            m_LevelEditorSettings = ScriptableObject.CreateInstance<LevelEditorSettings>();
        }

        m_LevelObjectReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_LevelObjects"), true, false, true, true);

        //m_LevelObjectReorderableList.drawHeaderCallback = (Rect rect) =>
        //{
        //    EditorGUI.LabelField(rect, "Level Objects");
        //};

        m_LevelObjectReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
	        SerializedProperty element = m_LevelObjectReorderableList.serializedProperty.GetArrayElementAtIndex(index);
	        rect.y += 2;       
	        EditorGUI.PropertyField(new Rect(rect.x, rect.y, 65, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_Direction"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + 75, rect.y, rect.width - 75, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_LevelObject"), GUIContent.none);
        };

        //Add a dropdown menu for selecting LevelObjects
        m_LevelObjectReorderableList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
        {
            GenericMenu menu = new GenericMenu();
            List<LevelObject> levelObjects = m_LevelEditorSettings.GetLevelObjects();

            foreach(LevelObject levelObject in levelObjects)
            {
                menu.AddItem(new GUIContent(levelObject.name), false, DropdownClickHandler, levelObject);
            }

            menu.ShowAsContext();
        };

        //Ping the object when clicking it
        m_LevelObjectReorderableList.onSelectCallback = (ReorderableList reorderalbeList) =>
        {
            SerializedProperty element = m_LevelObjectReorderableList.serializedProperty.GetArrayElementAtIndex(reorderalbeList.index);

            GameObject levelObject = element.FindPropertyRelative("m_LevelObject").objectReferenceValue as GameObject;

            if (levelObject != null)
                EditorGUIUtility.PingObject(levelObject);
        };

        //Avoid duplication of the last element (as this is never what we want and only causes confusion)
        m_LevelObjectReorderableList.onAddCallback = (ReorderableList reorderalbeList) =>
        {
            //Add a new element
            int index = reorderalbeList.serializedProperty.arraySize;
            reorderalbeList.serializedProperty.arraySize++;
            reorderalbeList.index = index;

            //Modify the new element
            SerializedProperty element = reorderalbeList.serializedProperty.GetArrayElementAtIndex(index);

            element.FindPropertyRelative("m_Direction").enumValueIndex = (int)Direction.Center + 1;
            element.FindPropertyRelative("m_LevelObject").objectReferenceValue = null;
        };
    }

    public override void OnInspectorGUI()
    {
        Node currentNode = (Node)target;

        serializedObject.Update();

        //Neighbours
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Neighbours", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        SerializedProperty serializedProperty = serializedObject.FindProperty("m_Neighbours");
        for (int i = 0; i < serializedProperty.arraySize; ++i)
        {
            SerializedProperty subSerializedProperty = serializedProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.ObjectField(subSerializedProperty, new GUIContent(((Direction)i).ToString()));
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        //Level Objects
        EditorGUILayout.LabelField("Level Objects", EditorStyles.boldLabel);

        m_LevelObjectReorderableList.DoLayoutList();

        Color prevColor = GUI.backgroundColor;

            GUI.backgroundColor = new Color(0.75f, 1.0f, 0.75f);
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fixedHeight = 30;

            if (GUILayout.Button("Detect Level Objects", buttonStyle))
            {
                currentNode.DetectLevelObjects();
                EditorUtility.SetDirty(target);
            }

        GUI.backgroundColor = prevColor;

        //Direction Mesh Filter
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Level Wizard References", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        serializedProperty = serializedObject.FindProperty("m_NodeMesh");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Node Mesh"));

        serializedProperty = serializedObject.FindProperty("m_NodeMeshRenderer");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Node Mesh Renderer"));

        serializedProperty = serializedObject.FindProperty("m_EdgeMeshes");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Edge Meshes"));

        serializedProperty = serializedObject.FindProperty("m_TileMeshRenderer");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Tile Mesh Renderer"));

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }

    private void DropdownClickHandler(object levelObject)
    {
        if (levelObject == null)
            return;

        LevelObject levelObjectPrefab = (LevelObject)levelObject;

        if (levelObjectPrefab == null)
            return;

        //Create the element
        Node currentNode = (Node)target;

        LevelObject newLevelObject = PrefabUtility.InstantiatePrefab(levelObjectPrefab) as LevelObject;
        newLevelObject.transform.parent = currentNode.transform;

        //Add a new element to the list
        int index = m_LevelObjectReorderableList.serializedProperty.arraySize;
        m_LevelObjectReorderableList.serializedProperty.arraySize++;
        m_LevelObjectReorderableList.index = index;

        //Modify it's properties
        SerializedProperty element = m_LevelObjectReorderableList.serializedProperty.GetArrayElementAtIndex(index);

        element.FindPropertyRelative("m_Direction").enumValueIndex = (int)Direction.Center + 1;
        element.FindPropertyRelative("m_LevelObject").objectReferenceValue = newLevelObject;

        serializedObject.ApplyModifiedProperties();
    }
}