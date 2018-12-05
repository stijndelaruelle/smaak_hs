using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMover : LevelObject
{
    [Header("Specific - Tile Mover")]
    [Space(5)]
    [SerializeField]
    private Node m_Node;

    [SerializeField]
    private Vector3 m_NewPosition;
    private Vector3 m_OriginalPosition;

    [SerializeField]
    private float m_EaseTimePerTile = 0.25f;

    [SerializeField]
    //Immergency feature, different sets of movable tiles have difficulty parenting & unparenting when moved in a different order!
    //Works just like the "Assign Neighbours" button in the Level Editor
    private bool m_AutoAssignNeighbours = true;

    [SerializeField]
    private AudioClip m_StartMoveSoundEffect;

    [SerializeField]
    private AudioClip m_StopMoveSoundEffect;

    [SerializeField]
    private Node[] m_NewNeighbours = new Node[4]; //0 = North, 1 = East, 2 = South, 3 = West
    private Node[] m_OriginalNeighbours = new Node[4];

    protected override void Start()
    {
        base.Start();

        m_Node.NodeStartMoveEvent += OnNodeStartMove;
        m_Node.NodeEndMoveEvent += OnNodeEndMove;

        m_OriginalPosition = m_Node.GetPosition();
        
        for (int i = 0; i < 4; ++i)
        {
            m_OriginalNeighbours[i] = m_Node.GetNeighbour((Direction)i);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (m_Node == null)
            return;

        m_Node.NodeStartMoveEvent -= OnNodeStartMove;
        m_Node.NodeEndMoveEvent -= OnNodeEndMove;
    }

    public override bool CanUse(Character character)
    {
        return m_IsEnabled;
    }

    public override bool CanUse(Character character, out string errorMessage)
    {
        errorMessage = "";

        if (m_IsEnabled == false)
        {
            errorMessage = "Can't use TileMover: It is disabled";
            return false;
        }

        return true;
    }

    public override void ToggleEnabled()
    {
        base.ToggleEnabled();

        //Should cashe this somewhere
        float distance = (m_NewPosition - m_OriginalPosition).magnitude / 4; //4 = grid size hardcoded. HELP

        //Move back
        if (m_IsEnabled == false)
        {
            m_Node.MovePosition(m_OriginalPosition, m_EaseTimePerTile * distance);
        }

        //Move to our new position
        else
        {
            m_Node.MovePosition(m_NewPosition, m_EaseTimePerTile * distance);
        }
    }

    private void OnNodeStartMove()
    {
        //Make sure we/enemies can't move on a moving platform
        LevelDirector.Instance.AddInputBlocker("TileMover: On Node Start Move");

        //Play SFX
        if (AudioPlayer.Instance != null)
            AudioPlayer.Instance.PlaySFXOneShot(m_StartMoveSoundEffect);
    }

    private void OnNodeEndMove()
    {
        //Manage neighbours now as we can only look for new neighbours automatically once we arrived!

        //We arrived at our original position
        if (m_IsEnabled == false)
        {
            if (m_AutoAssignNeighbours) { AutoAssignNeighbours(); }
            else { SetNeighbours(m_NewNeighbours, m_OriginalNeighbours); }
        }

        //We arrived at our new position
        else
        {
            if (m_AutoAssignNeighbours) { AutoAssignNeighbours(); }
            else { SetNeighbours(m_OriginalNeighbours, m_NewNeighbours); }
        }

        //Play SFX
        if (AudioPlayer.Instance != null)
            AudioPlayer.Instance.PlaySFXOneShot(m_StopMoveSoundEffect);

        //All the player to interact again
        LevelDirector.Instance.RemoveInputBlocker("TileMover: On Node End Move");

        //The level has changed, let everyone know!
        LevelDirector.Instance.RequestLevelUpdate();
    }

    //Basically the same code as "AssignNeighbours" in the LevelEditor
    private void AutoAssignNeighbours()
    {
        float gridSize = 4.0f;

        //Get all nodes (super iffy, as a wrong hierchy messes everything up!)
        Transform rootObject = m_Node.transform.parent;

        if (rootObject == null)
            return;

        List<Node> allNodes = new List<Node>();
        allNodes.AddRange(rootObject.GetComponentsInChildren<Node>());

        float halfSize = gridSize * 0.5f;

        //Clear Neighbours
        Node[] newNeighbours = new Node[4];
        Node[] currentNeighbours = new Node[4];
        currentNeighbours[(int)Direction.North] = m_Node.GetNeighbour(Direction.North);
        currentNeighbours[(int)Direction.East]  = m_Node.GetNeighbour(Direction.East);
        currentNeighbours[(int)Direction.South] = m_Node.GetNeighbour(Direction.South);
        currentNeighbours[(int)Direction.West]  = m_Node.GetNeighbour(Direction.West);

        Vector3 position = m_Node.GetPosition();

        foreach (Node otherNode in allNodes) //Important! We look trough all nodes here.
        {
            Vector3 otherPosition = otherNode.GetPosition();
            Vector3 diff = otherPosition - position;

            //North
            if (diff.z > halfSize && diff.z < (gridSize + halfSize) &&
                diff.x < halfSize && diff.x > -halfSize)
            {
                newNeighbours[(int)Direction.North] = otherNode;
            }

            //South
            if (diff.z < -halfSize && diff.z > (-gridSize - halfSize) &&
                diff.x < halfSize && diff.x > -halfSize)
            {
                newNeighbours[(int)Direction.South] = otherNode;
            }

            //East
            if (diff.x > halfSize && diff.x < (gridSize + halfSize) &&
                diff.z < halfSize && diff.z > -halfSize)
            {
                newNeighbours[(int)Direction.East] = otherNode;
            }

            //West
            if (diff.x < -halfSize && diff.x > (-gridSize - halfSize) &&
                diff.z < halfSize && diff.z > -halfSize)
            {
                newNeighbours[(int)Direction.West] = otherNode;
            }
        }

        //Call the actual function
        SetNeighbours(currentNeighbours, newNeighbours);
    }

    private void SetNeighbours(Node[] currentNeighbours, Node[] newNeighbours)
    {
        //Set the new neighbour
        for (int i = 0; i < 4; ++i)
        {
            Direction direction = (Direction)i;

            //Attach ourself
            m_Node.SetNeighbour(direction, newNeighbours[i]);

            //Say bye to an old friend
            if (currentNeighbours[i] != null)
            {
                //Only override when we are the neighbour (this can have changed this very frame thanks to another moveable tile)
                if (currentNeighbours[i].GetNeighbour(UtilityMethods.InverseDirection(direction)) == m_Node)
                    currentNeighbours[i].SetNeighbour(UtilityMethods.InverseDirection(direction), null);
            }

            //Create a new friend
            if (newNeighbours[i] != null)
            {
                newNeighbours[i].SetNeighbour(UtilityMethods.InverseDirection(direction), m_Node);
            }
        }
    }

    protected override void OnReset()
    {
        base.OnReset();
    }
}
