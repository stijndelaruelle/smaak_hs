using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(TileMover))]
[CanEditMultipleObjects]
public class CustomTileMoverInspector : CustomLevelObjectInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific - Tile Mover", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        //Node
        SerializedProperty serializedProperty = serializedObject.FindProperty("m_Node");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Node"));

        //New position
        serializedProperty = serializedObject.FindProperty("m_NewPosition");
        serializedProperty.vector3Value = EditorGUILayout.Vector3Field("New Position", serializedProperty.vector3Value);

        serializedProperty = serializedObject.FindProperty("m_EaseTimePerTile");
        serializedProperty.floatValue = EditorGUILayout.FloatField("Animation Time/Tile", serializedProperty.floatValue);

        EditorGUILayout.Space();

        //New neighbours
        EditorGUILayout.LabelField("New Neighbours", EditorStyles.boldLabel);

        serializedProperty = serializedObject.FindProperty("m_AutoAssignNeighbours");
        serializedProperty.boolValue = EditorGUILayout.Toggle("Auto Assign Neighbours", serializedProperty.boolValue);

        if (serializedProperty.boolValue == false)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            serializedProperty = serializedObject.FindProperty("m_NewNeighbours");
            for (int i = 0; i < serializedProperty.arraySize; ++i)
            {
                SerializedProperty subSerializedProperty = serializedProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.ObjectField(subSerializedProperty, new GUIContent(((Direction)i).ToString()));
            }

            EditorGUILayout.EndVertical();
        }

        //SFX
        EditorGUILayout.LabelField("Sound Effects", EditorStyles.boldLabel);

        serializedProperty = serializedObject.FindProperty("m_StartMoveSoundEffect");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Start Move Sound Effect"));

        serializedProperty = serializedObject.FindProperty("m_StopMoveSoundEffect");
        EditorGUILayout.ObjectField(serializedProperty, new GUIContent("Stop Move Sound Effect"));

        EditorGUILayout.EndVertical();

        //Apply changes
        serializedObject.ApplyModifiedProperties();

        DrawMaterialInspectorGUI();
    }
}