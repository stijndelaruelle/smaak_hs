using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum Direction
{
    None = -1,
    North = 0,
    East = 1,
    South = 2,
    West = 3,
    Center = 4,
    Max = 5
}

public enum RelativeDirection
{
    //Doesn't follow the North-East-South-West order as, even tough it looks logical, doesn't write/read very easy in code. I'd rather group them like this
    None = -1,
    Forward = 0,
    Backward = 1, 
    Left = 2,
    Right = 3,
    Max = 4
}

public enum DirectionConnection
{
    None = -1,
    One = 0,
    TwoStraight = 1,
    TwoCorner = 2,
    Three = 3,
    Four = 4,
    Max = 5
}

public enum DirectionAxis
{
    None = -1,
    Horizontal = 0,
    Vertical = 1
}

[System.Serializable]
public class LevelObjectDirectionPair //Would love for it to be a struct, but i need a reference type.
{
    [SerializeField]
    private LevelObject m_LevelObject;
    public LevelObject LevelObject
    {
        get { return m_LevelObject; }
    }

    [SerializeField]
    private Direction m_Direction = Direction.Center;
    public Direction Direction
    {
        get { return m_Direction; }
    }

    public LevelObjectDirectionPair()
    {
        m_LevelObject = null;
        m_Direction = Direction.Center;
    }

    public LevelObjectDirectionPair(LevelObject levelObject, Direction direction)
    {
        m_LevelObject = levelObject;
        m_Direction = direction;
    }
}

public class Node : ResetableObject, IHintable, IComparable
{
    public delegate void NodeDelegate();

    [SerializeField]
    //Could make this into a list at some point to potentionally add more neighbours.
    private Node[] m_Neighbours = new Node[4]; //0 = North, 1 = East, 2 = South, 3 = West
    public Node[] Neighbours
    {
        get { return m_Neighbours; }
    }

    [SerializeField]
    //As unity can't serialize a list of an interface, we convert. The original list is ONLY used here and should never be referenced to in code.
    //It's a small waste of memory (as these lists will never get big), but any other solution requires a lot of workarounds which makes the whole thing less readable.
    private List<LevelObjectDirectionPair> m_LevelObjects;
    private List<Character> m_Characters;
    public List<Character> Characters
    {
        get { return m_Characters; }
    }

    private Transform m_Transform;

    //Used for reset
    private Node[] m_NeighboursReset = new Node[4];
    private Vector3 m_PositionReset;

    [Header("Materials (Level Wizard)")]
    [Space(5)]
    [SerializeField]
    private MeshFilter m_NodeMesh;

    [SerializeField]
    private MeshRenderer m_NodeMeshRenderer;

    [SerializeField]
    private Transform m_EdgeMeshes;

    [SerializeField]
    private MeshRenderer m_TileMeshRenderer;

    public event NodeDelegate NodeStartMoveEvent;
    public event NodeDelegate NodeEndMoveEvent;
    public event HintDelegate HintUsedEvent;

    private void Awake()
    {
        m_Transform = transform;
        m_Characters = new List<Character>();

        //For reset
        m_PositionReset = m_Transform.position;

        for (int i = 0; i < m_Neighbours.Length; ++i)
        {
            m_NeighboursReset[i] = m_Neighbours[i];
        }
    }


    public void OnEnter(Character character, Direction direction, bool snap)
    {
        if (m_Characters.Contains(character) == false)
            m_Characters.Add(character);

        if (HintUsedEvent != null)
            HintUsedEvent(this, character);

        if (m_LevelObjects == null)
            return;

        foreach (LevelObjectDirectionPair levelObjectDirectionPair in m_LevelObjects)
        {
            Direction inverseDir = UtilityMethods.InverseDirection(direction);

            if (levelObjectDirectionPair.LevelObject != null)
                levelObjectDirectionPair.LevelObject.OnCharacterEnter(character, inverseDir, snap);

            if (levelObjectDirectionPair.Direction == Direction.Center ||
                levelObjectDirectionPair.Direction == inverseDir)
            {
                if (levelObjectDirectionPair.LevelObject != null)
                    levelObjectDirectionPair.LevelObject.OnCharacterDirectionEnter(character);
            }
        }
    }

    public void OnLeave(Character character, Direction direction)
    {
        if (m_Characters.Contains(character) == true)
            m_Characters.Remove(character);
        
        if (m_LevelObjects == null)
            return;

        foreach (LevelObjectDirectionPair levelObjectDirectionPair in m_LevelObjects)
        {
            if (levelObjectDirectionPair.LevelObject != null)
                levelObjectDirectionPair.LevelObject.OnCharacterLeave(character, direction);

            if (levelObjectDirectionPair.Direction == Direction.Center ||
                levelObjectDirectionPair.Direction == direction)
            {
                if (levelObjectDirectionPair.LevelObject != null)
                    levelObjectDirectionPair.LevelObject.OnCharacterDirectionLeave(character);
            }
        }
    }

    public bool OnUse(Character character, Direction direction)
    {
        if (m_LevelObjects == null)
            return false;

        bool success = false;

        foreach (LevelObjectDirectionPair levelObjectDirectionPair in m_LevelObjects)
        {
            if (levelObjectDirectionPair.Direction == Direction.Center ||
                levelObjectDirectionPair.Direction == direction)
            {
                if (levelObjectDirectionPair.LevelObject != null)
                {
                    bool hasUsed = levelObjectDirectionPair.LevelObject.OnCharacterUse(character);

                    if (hasUsed == true)
                        success = true;
                }
            }
        }

        return success;
    }


    //Accessors & Mutators
    public bool CanEnter(Direction direction)
    {
        if (direction == Direction.None)
            return true;

        //Do we have a neighbour in this direction?
        if (m_Neighbours[(int)direction] == null)
            return false;

        //Go trough all our objects that are placed at this direction and check if they block the player
        foreach (LevelObjectDirectionPair levelObjectDirectionPair in m_LevelObjects)
        {
            if (levelObjectDirectionPair.LevelObject == null)
                continue;

            if (levelObjectDirectionPair.Direction == direction)
            {
                if (levelObjectDirectionPair.LevelObject.BlocksCharacters() == true)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool CanUse(Character character, Direction direction)
    {
        if (m_LevelObjects == null)
            return false;

        foreach (LevelObjectDirectionPair levelObjectDirectionPair in m_LevelObjects)
        {
            if (levelObjectDirectionPair.Direction == Direction.Center ||
                levelObjectDirectionPair.Direction == direction)
            {
                if (levelObjectDirectionPair.LevelObject != null)
                {
                    string errorMessage = "";
                    bool canUse = levelObjectDirectionPair.LevelObject.CanUse(character, out errorMessage);

                    if (canUse == true)
                        return true;
                    else
                    {
                        //Create a message system
                        if (errorMessage != "")
                            Debug.Log(errorMessage);
                    }
                }
            }
        }

        return false;
    }

    public Vector3 GetPosition()
    {
        return m_Transform.position;
    }

    public void SetPosition(Vector3 position)
    {
        MovePosition(position, 0.0f);
    }

    public void MovePosition(Vector3 newPosition, float time = 1.0f)
    {
        //Move
        m_Transform.DOMove(newPosition, time).SetEase(Ease.InOutQuad).OnComplete(OnMoveComplete);

        //Move along all our LevelObjects

        //Code works but please don't: Moveable tiles in combination with gates can give some very weird results
        /*
        foreach (LevelObjectDirectionPair levelObjectDirectionPair in m_LevelObjects)
        {
            if (levelObjectDirectionPair.LevelObject == null)
                continue;

            if (UtilityMethods.IsParent(levelObjectDirectionPair.LevelObject.GetTransform(), m_Transform) == true) //Will move automatically
                continue;

            //Only move the object if our neighbour in that direction isn't linked to it! (mostly goes wrong with gates)
            Node neighbour = GetNeighbour(levelObjectDirectionPair.Direction);
            if (neighbour != null && neighbour.ContainsLevelObject(levelObjectDirectionPair.LevelObject, UtilityMethods.InverseDirection(levelObjectDirectionPair.Direction)) == true)
                continue;

            levelObjectDirectionPair.LevelObject.MoveWithTransform(m_Transform);
        }
        */

        //Move along all our characters
        foreach (Character character in m_Characters)
        {
            character.MoveWithTransform(m_Transform);
        }

        //Fire event
        if (NodeStartMoveEvent != null)
            NodeStartMoveEvent();
    }

    private void OnMoveComplete()
    {
        //Let our levelObjects know we're done moving
        foreach (LevelObjectDirectionPair levelObjectDirectionPair in m_LevelObjects)
        {
            if (levelObjectDirectionPair.LevelObject != null && UtilityMethods.IsParent(levelObjectDirectionPair.LevelObject.GetTransform(), m_Transform) == false)
                levelObjectDirectionPair.LevelObject.StopMoveWithTransform();
        }

        //Move along all our characters
        foreach (Character character in m_Characters)
        {
            character.StopMoveWithTransform();
        }

        //Fire event
        if (NodeEndMoveEvent != null)
            NodeEndMoveEvent();
    }

    public Node GetNeighbour(Direction direction)
    {
        int dirInt = (int)direction;
        if (dirInt < 0 || dirInt >= m_Neighbours.Length)
            return null;

        return m_Neighbours[dirInt];
    }

    public void SetNeighbour(Direction direction, Node node)
    {
        int dirInt = (int)direction;
        if (dirInt < 0 || dirInt >= m_Neighbours.Length)
            return;

        m_Neighbours[dirInt] = node;
    }

    public bool ContainsCharacter(Character character)
    {
        return (m_Characters.Contains(character));
    }

    public bool ContainsLevelObject(LevelObject levelObject, Direction direction)
    {
        foreach (LevelObjectDirectionPair levelObjectDirectionPair in m_LevelObjects)
        {
            if (levelObjectDirectionPair.LevelObject == levelObject && levelObjectDirectionPair.Direction == direction)
                return true;
        }

        return false;
    }

    public string GetFirstLevelObjectName()
    {
        //Used in the level helper tool
        if (m_LevelObjects == null)
            return "";

        if (m_LevelObjects.Count <= 0)
            return "";

        if (m_LevelObjects[0].LevelObject != null)
            return m_LevelObjects[0].LevelObject.name;

        return "";
    }


    //Level wizard
    public void SetNodeMesh(Mesh mesh)
    {
        if (m_NodeMesh != null)
            m_NodeMesh.mesh = mesh;
    }

    public void RotateNodeMesh(Vector3 rotation)
    {
        if (m_NodeMesh == null)
            return;

        m_NodeMesh.transform.rotation = Quaternion.Euler(rotation);
    }

    public void EnableEdgeMesh(int id)
    {
        //0 = North, 1 = East, 2 = South, 3 = West // 4 = Corner NE, 5 = Corner SE, 6 = Corner SW, 7 = Corner NW
        if (id < 0 || id >= m_EdgeMeshes.childCount)
            return;

        m_EdgeMeshes.GetChild(id).gameObject.SetActive(true);
    }

    public void DisableAllEdgeMeshes()
    {
        for (int i = 0; i < m_EdgeMeshes.childCount; ++i)
        {
            m_EdgeMeshes.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void DetectLevelObjects()
    {
        LevelObject[] levelObjects = transform.GetComponentsInChildren<LevelObject>();

        foreach(LevelObject levelObject in levelObjects)
        {
            //Check if we already have this levelobject added
            bool found = false;
            foreach(LevelObjectDirectionPair otherLevelObject in m_LevelObjects)
            {
                if (levelObject == otherLevelObject.LevelObject)
                {
                    found = true;
                    break;
                }
            }

            //if not we can add it!
            if (found == false)
                m_LevelObjects.Add(new LevelObjectDirectionPair(levelObject, Direction.Center));
        }
    }

    //Level wizard - theme
    public void SetNodeMeshMaterial(Material material)
    {
        if (m_NodeMeshRenderer != null)
            m_NodeMeshRenderer.material = material;
    }

    public void SetEdgeMeshMaterial(Material material)
    {
        //Lame but only used in the editor
        MeshRenderer[] meshRenderers = m_EdgeMeshes.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material = material;
        }
    }

    public void SetTileMeshMaterial(Material material)
    {
        if (m_TileMeshRenderer != null)
            m_TileMeshRenderer.material = material;
    }

    public void SetLevelObjectMaterials(Material[] materials)
    {
        foreach(LevelObjectDirectionPair levelObjectDirectionPair in m_LevelObjects)
        {
            if (levelObjectDirectionPair.LevelObject != null)
                levelObjectDirectionPair.LevelObject.SetMaterials(materials);
        }
    }

    //ResetableObject
    protected override void OnReset()
    {
        //Reset position
        m_Transform.position = m_PositionReset;

        //Reset neighbours
        for (int i = 0; i < m_NeighboursReset.Length; ++i)
        {
            m_Neighbours[i] = m_NeighboursReset[i];
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //https://forum.unity.com/threads/debug-drawarrow.85980/

        //Only to be used as reference when placing the line renderers

        //Render neighbours
        Vector3 position = transform.position;

        for (int i = 0; i < m_Neighbours.Length; ++i)
        {
            Node node = m_Neighbours[i];
            if (node != null)
            {  
                Vector3 neighbourPosition = node.transform.position;
                Vector3 direction = neighbourPosition - position;

                //Move the arow a little bit to the right so multiples don't overlap
                Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x).normalized;
                position.x += perpendicular.x * 0.02f;
                position.z += perpendicular.z * 0.02f;
                neighbourPosition.x += perpendicular.x * 0.02f;
                neighbourPosition.z += perpendicular.z * 0.02f;
                
                //Line
                Gizmos.color = Color.white;
                Gizmos.DrawRay(position, direction);

                //Arrow
                float arrowHeadAngle = 25.0f;
                float arrowHeadLength = 0.5f;
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);

                Gizmos.DrawRay(position + direction, left * arrowHeadLength);
                Gizmos.DrawRay(position + direction, right * arrowHeadLength);

                //Label
                Handles.Label(neighbourPosition, ((Direction)i).ToString());
            }
        }

        //Render LevelObjects
        int[] directionCount = new int[(int)Direction.Max];

        for (int i = 0; i < m_LevelObjects.Count; ++i)
        {
            LevelObject levelObject = m_LevelObjects[i].LevelObject;

            if (levelObject != null)
            {
                Vector3 levelObjectPosition = levelObject.transform.position;
                Vector3 direction = levelObjectPosition - position;

                //Line
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(position, direction);

                //Cube
                int currentDirectionCount = directionCount[(int)m_LevelObjects[i].Direction];
                Gizmos.DrawWireCube(levelObjectPosition, new Vector3(0.2f + (0.05f * currentDirectionCount), 0.2f + (0.05f * currentDirectionCount), 0.2f + (0.05f * currentDirectionCount)));

                directionCount[(int)m_LevelObjects[i].Direction] = currentDirectionCount + 1;
            }
        }

        DebugDrawLevelObjectNames();
    }

    private void DebugDrawLevelObjectNames()
    {
        Vector3 position = transform.position;

        //Render LevelObject names
        for (int i = 0; i < m_LevelObjects.Count; ++i)
        {
            LevelObject levelObject = m_LevelObjects[i].LevelObject;

            if (levelObject != null)
            {
                Vector3 levelObjectPosition = levelObject.transform.position;
                
                //Name
                string name = gameObject.name + ": ";
                name += ObjectNames.GetInspectorTitle(levelObject);
                name = name.Replace("(Script)", "");
                name += "(" + m_LevelObjects[i].Direction + ")";

                Handles.Label(levelObjectPosition + new Vector3(0, (-i * 0.2f), 0), name);
            }
        }
    }
#endif

    //IComparer
    public int CompareTo(object obj)
    {
        Vector3 pos1 = transform.position;
        Vector3 pos2 = ((Node)obj).transform.position;

        //first compare the Z
        if (pos1.z < pos2.z)
        {
            return -1;
        }
        else if (pos1.z > pos2.z)
        {
            return 1;
        }
        else
        {
            //If they are equal compare the X
            if (pos1.x < pos2.x)
            {
                return -1;
            }
            else if (pos1.x > pos2.x)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}