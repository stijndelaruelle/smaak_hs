using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    [SerializeField]
    private Transform m_RootObject;

    [SerializeField]
    private LevelEditorSettings.LevelTheme m_LevelTheme;

    private LevelEditorSettings m_LevelEditorSettings;
    private Texture2D m_Merlin;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Cyber Security/Level Wizard")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow levelEditorWindow = EditorWindow.GetWindow(typeof(LevelEditorWindow));
        levelEditorWindow.titleContent.text = "Level Wizard";
    }

    private void OnEnable()
    {
        //Load the wizard!
        m_LevelEditorSettings = (LevelEditorSettings)Resources.Load("LevelEditorSettings", typeof(LevelEditorSettings));

        if (m_LevelEditorSettings == null)
        {
            Debug.LogError("No Level editor settings found, using a temporary one to avoid crashes!");
            m_LevelEditorSettings = ScriptableObject.CreateInstance<LevelEditorSettings>();
        }
    }

    private void OnGUI()
    {
        Color prevColor = GUI.backgroundColor;

        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 100, Screen.height));

            GUILayout.Label("Base Settings", EditorStyles.boldLabel);

            //Root object
            GUILayout.BeginHorizontal();
                GUILayout.Label("Root Object");

                //Make it red when none has been selected
                if (m_RootObject == null)
                    GUI.backgroundColor = new Color(1.0f, 0.75f, 0.75f);
                else
                    GUI.backgroundColor = new Color(0.75f, 1.0f, 0.75f);

                m_RootObject = (Transform)EditorGUILayout.ObjectField(m_RootObject, typeof(Transform), true);

                GUI.backgroundColor = prevColor;

                if (GUILayout.Button("Find")) { FindRootObject(); }
            GUILayout.EndHorizontal();

            //Level theme
            GUILayout.BeginHorizontal();

                GUILayout.Label("Level Theme");

                m_LevelTheme = (LevelEditorSettings.LevelTheme)EditorGUILayout.EnumPopup(m_LevelTheme);
                if (GUILayout.Button("Assign")) { AssignLevelTheme(); }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            //Buttons
            GUI.backgroundColor = new Color(0.75f, 1.0f, 0.75f);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fixedHeight = 30;

            if (GUILayout.Button("Snap Nodes", buttonStyle)) { SnapNodes(); }
            if (GUILayout.Button("Rename Nodes", buttonStyle)) { RenameNodes(); }
            if (GUILayout.Button("Assign Neighbours", buttonStyle)) { AssignNeighbours(); }
            if (GUILayout.Button("Assign Direction Meshes", buttonStyle)) { AssignDirectionMeshes(); }
            if (GUILayout.Button("Assign Edge Meshes", buttonStyle)) { AssignEdgeMeshes(); }

            GUILayout.Space(10);

            GUI.backgroundColor = new Color(1.0f, 0.75f, 0.75f);
            if (GUILayout.Button("Execute All", buttonStyle)) { ExecuteAll(); }

            GUILayout.Space(10);

            GUI.backgroundColor = new Color(0.75f, 0.75f, 1.0f);
            if (GUILayout.Button("Spawn Tile", buttonStyle)) { SpawnTilePrefab(); }
            if (GUILayout.Button("Load Global Scene", buttonStyle)) { LoadGlobalScene(); }

            GUI.backgroundColor = prevColor;

        GUILayout.EndArea();

        //The wizard!
        GUILayout.BeginArea(new Rect(Screen.width - 100 + 20, 10, 100, 128));
            GUILayout.Label(m_LevelEditorSettings.Merlin);
        GUILayout.EndArea();

        GUI.backgroundColor = prevColor;
    }

    private void FindRootObject()
    {
        Transform levelTransform = GameObject.Find("Level").transform;

        if (levelTransform != null)
            m_RootObject = levelTransform;
    }

    private void SpawnTilePrefab()
    {
        if (m_RootObject == null)
        {
            Debug.LogWarning("No root level object found!");
            return;
        }

        if (m_LevelEditorSettings.TilePrefab == null)
            return; 

        GameObject newTile = PrefabUtility.InstantiatePrefab(m_LevelEditorSettings.TilePrefab) as GameObject;
        Transform newTileTransform = newTile.transform;

        newTileTransform.parent = m_RootObject;
        newTileTransform.position = Vector3.zero;
        newTileTransform.rotation = Quaternion.identity;
        newTileTransform.localScale = Vector3.one;
    }

    private void LoadGlobalScene()
    {
        if (m_LevelEditorSettings == null)
            return;

        EditorSceneManager.OpenScene(m_LevelEditorSettings.GlobalScenePath, OpenSceneMode.Additive);
    }

    private void ExecuteAll()
    {
        List<Node> nodes = GetNodes();

        SnapNodes(nodes);
        RenameNodes(nodes);
        AssignNeighbours(nodes);
        AssignDirectionMeshes(nodes);
        AssignEdgeMeshes(nodes);
    }

    private void SnapNodes()
    {
        SnapNodes(GetNodes());
    }

    private void SnapNodes(List<Node> nodes)
    {
        if (nodes == null)
            return;

        if (nodes.Count <= 0)
            return;

        float gridSize = m_LevelEditorSettings.GridSize;

        foreach (Node node in nodes)
        {
            //Round everything to the nearest multiple of our size
            Vector3 position = node.transform.position;
            position.x = Mathf.Round(position.x / gridSize) * gridSize;
            position.y = Mathf.Round(position.y / gridSize) * gridSize;
            position.z = Mathf.Round(position.z / gridSize) * gridSize;

            node.transform.position = position;
        }
    }

    private void RenameNodes()
    {
        RenameNodes(GetNodes());
    }

    private void RenameNodes(List<Node> nodes)
    {
        if (nodes == null)
            return;

        if (nodes.Count <= 0)
            return;

        //Change their order in the hierarchy
        nodes.Sort();

        for (int i = 0; i < nodes.Count; ++i)
        {
            Node node = nodes[i];
            nodes[i].transform.SetSiblingIndex(i);

            string newNodeName = "Tile ";
        
            //Start with "Tile x-y"
            Vector3 position = node.transform.position;

            newNodeName += "(" + position.x + " / " + position.z + ")";

            //Add the first linked levelObject to the name between brackets
            string levelObjectName = node.GetFirstLevelObjectName();

            if (levelObjectName != "")
                newNodeName += " - " + levelObjectName;

            node.name = newNodeName;
        }
    }

    private void RenameNodesRelative(List<Node> nodes)
    {
        if (nodes == null)
            return;

        if (nodes.Count <= 0)
            return;

        float minX = nodes[0].transform.position.x;
        float minZ = nodes[0].transform.position.z;

        //Loop trough all the nodes just to figure out leftmost & bottom most id's are
        foreach (Node node in nodes)
        {
            Vector3 position = node.transform.position;

            if (position.x < minX)
                minX = position.x;

            if (position.z < minZ)
                minZ = position.z;
        }

        //Once we know this, we can rename everything
        foreach (Node node in nodes)
        {
            string newNodeName = "Tile ";

            //Start with "Tile x-y"
            Vector3 position = node.transform.position;

            float diffX = Mathf.Abs(position.x - minX);
            float diffZ = Mathf.Abs(position.z - minZ);

            //Change the diff to "amount of tiles"
            diffX = Mathf.Round(diffX / m_LevelEditorSettings.GridSize);
            diffZ = Mathf.Round(diffZ / m_LevelEditorSettings.GridSize);

            newNodeName += diffX + "-" + diffZ;

            //Add the first linked levelObject to the name between brackets
            string levelObjectName = node.GetFirstLevelObjectName();

            if (levelObjectName != "")
                newNodeName += " (" + levelObjectName + ")";

            node.name = newNodeName;
        }
    }

    private void AssignNeighbours()
    {
        AssignNeighbours(GetNodes());
    }

    private void AssignNeighbours(List<Node> nodes)
    {
        if (m_RootObject == null)
            return;

        if (nodes == null)
            return;

        //Get all nodes
        List<Node> allNodes = GetNodesFromRootObject();

        float halfSize = m_LevelEditorSettings.GridSize * 0.5f;

        //Loop trough all nodes and check for neighbours
        foreach(Node node in nodes)
        {
            SerializedObject serializedObject = new SerializedObject(node);
            
            //Clear Neighbours
            node.SetNeighbour(Direction.North, null);
            node.SetNeighbour(Direction.East,  null);
            node.SetNeighbour(Direction.South, null);
            node.SetNeighbour(Direction.West,  null);

            Vector3 position = node.transform.position;
            
            foreach (Node otherNode in allNodes) //Important! We look trough all nodes here.
            {
                Vector3 otherPosition = otherNode.transform.position;
                Vector3 diff = otherPosition - position;

                //North
                if (diff.z > halfSize && diff.z < (m_LevelEditorSettings.GridSize + halfSize) &&
                    diff.x < halfSize && diff.x > -halfSize)
                {
                    SetNeighbour(node, Direction.North, otherNode);
                }

                //South
                if (diff.z < -halfSize && diff.z > (-m_LevelEditorSettings.GridSize - halfSize) &&
                    diff.x < halfSize  && diff.x > -halfSize)
                {
                    SetNeighbour(node, Direction.South, otherNode);
                }

                //East
                if (diff.x > halfSize && diff.x < (m_LevelEditorSettings.GridSize + halfSize) &&
                    diff.z < halfSize && diff.z > -halfSize)
                {
                    SetNeighbour(node, Direction.East, otherNode);
                }

                //West
                if (diff.x < -halfSize && diff.x > (-m_LevelEditorSettings.GridSize - halfSize) &&
                    diff.z < halfSize  && diff.z > -halfSize)
                {
                    SetNeighbour(node, Direction.West, otherNode);
                }

                //Remember to apply modified properties
                serializedObject.ApplyModifiedProperties(); 
            }
        }
    }

    private void SetNeighbour(Node node, Direction direction, Node neighbour)
    {
        //I didn't want to clutter the Node script with these editor specific calls
        SerializedObject serializedObject = new SerializedObject(node);
        SerializedProperty serializedProperty = serializedObject.FindProperty("m_Neighbours");

        int dirInt = (int)direction;
        if (dirInt < 0 || dirInt >= serializedProperty.arraySize)
            return;

        SerializedProperty subSerializedProperty = serializedProperty.GetArrayElementAtIndex(dirInt);
        subSerializedProperty.objectReferenceValue = neighbour;

        //Remember to apply modified properties
        serializedObject.ApplyModifiedProperties();
    }

    private void AssignDirectionMeshes()
    {
        AssignDirectionMeshes(GetNodes());
    }

    private void AssignDirectionMeshes(List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            Node[] neighbours = node.Neighbours;
            Direction firstDirection = Direction.None; //Speeds up the process
            Direction secondDirection = Direction.None;
            Direction thirdDirection = Direction.None;

            //Get the number of neighbours
            int amountOfNeighbours = 0;
            for (int i = 0; i < neighbours.Length; ++i)
            {
                if (neighbours[i] != null)
                {
                    if (firstDirection == Direction.None) { firstDirection = (Direction)i; }
                    else if (secondDirection == Direction.None) { secondDirection = (Direction)i; }
                    else if (thirdDirection == Direction.None) { thirdDirection = (Direction)i; }

                    amountOfNeighbours++;
                }
            }

            float rotation = -90.0f; //Rotate to meshes to the north
            float rotationStep = 90.0f;

            switch (amountOfNeighbours)
            {
                case 0:
                {
                    node.SetNodeMesh(null);
                    break;
                }

                case 1:
                {
                    rotation += (rotationStep * (int)firstDirection);
                    node.RotateNodeMesh(new Vector3(-90.0f, 0.0f, rotation));
                    node.SetNodeMesh(m_LevelEditorSettings.GetDirectionMesh(DirectionConnection.One));
                    break;
                }

                case 2:
                {
                    int stepDifference = UtilityMethods.GetDirectionStepDifference(firstDirection, secondDirection);
                    rotation += (rotationStep * (int)firstDirection);

                    //Straight line
                    if (stepDifference == 2)
                    {
                        node.RotateNodeMesh(new Vector3(-90.0f, 0.0f, rotation));
                        node.SetNodeMesh(m_LevelEditorSettings.GetDirectionMesh(DirectionConnection.TwoStraight));
                    }

                    //Corner
                    else
                    {
                        //Default is North-West, so in case of North-East add another rotation step
                        if (stepDifference == 1)
                            rotation += rotationStep;

                        node.RotateNodeMesh(new Vector3(-90.0f, 0.0f, rotation));
                        node.SetNodeMesh(m_LevelEditorSettings.GetDirectionMesh(DirectionConnection.TwoCorner));
                    }

                    break;
                }

                case 3:
                {
                    int stepDifference1 = UtilityMethods.GetDirectionStepDifference(firstDirection, secondDirection);
                    int stepDifference2 = UtilityMethods.GetDirectionStepDifference(secondDirection, thirdDirection);

                    rotation += (rotationStep * (int)firstDirection);

                    //Default is North-East-West, so in case of North-East-South  or North-West-South add more steps
                    if (stepDifference1 == 1)
                    {
                        if (stepDifference2 == 1)  
                        {
                            rotation += rotationStep * 2;
                        }
                        else
                        {
                            rotation += rotationStep;
                        }    
                    }

                    node.RotateNodeMesh(new Vector3(-90.0f, 0.0f, rotation));
                    node.SetNodeMesh(m_LevelEditorSettings.GetDirectionMesh(DirectionConnection.Three));
                    break;
                }

                case 4:
                {
                    node.RotateNodeMesh(new Vector3(-90.0f, 0.0f, rotation));
                    node.SetNodeMesh(m_LevelEditorSettings.GetDirectionMesh(DirectionConnection.Four));
                    break;
                }

                default:
                    break;
            }
        }
    }

    private void AssignEdgeMeshes()
    {
        AssignEdgeMeshes(GetNodes());
    }

    private void AssignEdgeMeshes(List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            node.DisableAllEdgeMeshes();

            Node northNeighbour = node.GetNeighbour(Direction.North);
            Node southNeighbour = node.GetNeighbour(Direction.South);
            Node eastNeighbour = node.GetNeighbour(Direction.East);
            Node westNeighbour = node.GetNeighbour(Direction.West);

            //Edges
            if (northNeighbour == null) { node.EnableEdgeMesh((int)Direction.North); }
            if (southNeighbour == null) { node.EnableEdgeMesh((int)Direction.South); }
            if (eastNeighbour == null)  { node.EnableEdgeMesh((int)Direction.East);  }
            if (westNeighbour == null)  { node.EnableEdgeMesh((int)Direction.West);  }

            //Corners (that are 100% ours)
            if (northNeighbour == null && eastNeighbour == null) { node.EnableEdgeMesh(4); }
            if (northNeighbour == null && westNeighbour == null) { node.EnableEdgeMesh(5); }
            if (southNeighbour == null && eastNeighbour == null) { node.EnableEdgeMesh(6); }
            if (southNeighbour == null && westNeighbour == null) { node.EnableEdgeMesh(7); }

            //Corners (that we share).
            //If we only do North & East, our neighbours will use their North & East (which is our South & West)
            if (northNeighbour != null || eastNeighbour != null)
            {
                //Avoid Corner stones in squares
                if (!((northNeighbour != null && northNeighbour.GetNeighbour(Direction.East) != null) &&
                    (eastNeighbour != null && eastNeighbour.GetNeighbour(Direction.North) != null)))
                {
                    node.EnableEdgeMesh(4);
                }
            }
            if (northNeighbour != null && westNeighbour == null) { node.EnableEdgeMesh(5); }
            if (southNeighbour == null && eastNeighbour != null) { node.EnableEdgeMesh(6); }

            //We only get overlap when 
        }
    }

    private void AssignLevelTheme()
    {
        AssignLevelTheme(GetNodes());
    }

    private void AssignLevelTheme(List<Node> nodes)
    {
        LevelThemeDefinition levelTheme = m_LevelEditorSettings.GetLevelTheme(m_LevelTheme);

        foreach(Node node in nodes)
        {
            node.SetTileMeshMaterial(levelTheme.TileMaterial);
            node.SetEdgeMeshMaterial(levelTheme.EdgeMaterial);
            node.SetNodeMeshMaterial(levelTheme.NodeMaterial);

            //Make one big list with all the materials (MAKE SURE THE ENUM SUPPORTS ALL THIS CONVERSION!)
            Material[] materials = new Material[(int)MaterialType.Node + 1];

            for (int i = 0; i < levelTheme.LevelObjectMaterials.Count; ++i)
            {
                materials[((int)MaterialType.Primary) + i] = levelTheme.LevelObjectMaterials[i];
            }

            for (int i = 0; i < levelTheme.InteractableMaterials.Count; ++i)
            {
                materials[((int)MaterialType.Interactive1) + i] = levelTheme.InteractableMaterials[i];
            }

            materials[(int)MaterialType.Tile] = levelTheme.TileMaterial;
            materials[(int)MaterialType.Edge] = levelTheme.EdgeMaterial;
            materials[(int)MaterialType.Node] = levelTheme.NodeMaterial;

            node.SetLevelObjectMaterials(materials);
        }
    }

    private List<Node> GetNodes()
    {
        List<Node> nodes = new List<Node>();

        //Get all the selected nodes
        Transform[] selection = Selection.transforms;

        if (selection.Length > 0)
        {
            //Add node objects on us or or as children
            for (int i = 0; i < selection.Length; ++i)
            {
                nodes.AddRange(selection[i].GetComponents<Node>());
                nodes.AddRange(selection[i].GetComponentsInChildren<Node>());
            }

            //If that didn't result in anything, check our direct parent (it happens a lot that we select a subobject
            if (nodes.Count == 0)
            {
                Transform parent = selection[0].parent;

                if (parent != null)
                    nodes.AddRange(parent.GetComponents<Node>());
            }
        }
        else
        {
            nodes = GetNodesFromRootObject();
        }

        return nodes;
    }

    private List<Node> GetNodesFromRootObject()
    {
        //If none are selected, select everything from the rootObject
        if (m_RootObject == null)
            return null;

        List<Node> nodes = new List<Node>();
        nodes.AddRange(m_RootObject.GetComponentsInChildren<Node>());

        return nodes;
    }
}