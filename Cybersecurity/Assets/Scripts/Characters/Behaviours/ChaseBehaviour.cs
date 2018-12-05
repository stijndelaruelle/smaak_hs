using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBehaviour : EnemyBehaviour
{
    public class AStarNode
    {
        private Node m_WorldNode;
        public Node WorldNode
        {
            get { return m_WorldNode; }
            set { m_WorldNode = value; }
        }

        private AStarNode m_Parent;
        public AStarNode Parent
        {
            get { return m_Parent; }
            set
            {
                m_Parent = value;
                m_DirectionFromParent = GetDirectionFromParent();
            }
        }

        //Cached for when we need to retrace the path
        private Direction m_DirectionFromParent;
        public Direction DirectionFromParent
        {
            get { return m_DirectionFromParent; }
        }

        private float m_G; //Total effort it takes to get to this tile
        public float G
        {
            get { return m_G; }
        }

        private float m_H; //Heuristic, block distance to target
        public float H
        {
            get { return m_H; }
        }

        public float F
        {
            get { return (m_G + m_H); }
        }


        //Constructors
        public AStarNode(Node worldNode, AStarNode parent)
        {
            m_WorldNode = worldNode;
            m_Parent = parent;
            m_G = 0;
            m_H = 0;
            m_DirectionFromParent = Direction.None;
        }

        public AStarNode(Node worldNode, AStarNode parent, Direction directionFromParent)
        {
            m_WorldNode = worldNode;
            m_Parent = parent;
            m_G = 0;
            m_H = 0;
            m_DirectionFromParent = directionFromParent;
        }

        public AStarNode(AStarNode aStarNode)
        {
            CopyFrom(aStarNode);
        }

        //Mutators
        public void CopyFrom(AStarNode aStarNode)
        {
            m_WorldNode = aStarNode.WorldNode;
            m_Parent = aStarNode.Parent;
            m_G = aStarNode.G;
            m_H = aStarNode.H;
            m_DirectionFromParent = aStarNode.DirectionFromParent;
        }

        public void CalculateValues(Node targetNode)
        {
            //G: Movement cost to get to this tile from the start point
            m_G = 0;
            if (m_Parent != null)
            {
                //1 as we the distance is always equal (1 tile)
                m_G = m_Parent.G + 1; 

                //If we change direction add a small extra cost (so we walk in straight lines when possible)
                //if (m_Parent.m_DirectionFromParent != Direction.None)
                //{
                    //if (m_DirectionFromParent != m_Parent.m_DirectionFromParent)
                    //{
                    //    m_G += 0.5f;
                    //}
                //}
            }

            //H: Estimated cost to get to the target tile from here. (manhattan distance)
            Vector3 diff = (m_WorldNode.GetPosition() - targetNode.GetPosition());
            m_H = diff.x + diff.z;
        }

        //Accessors
        public List<AStarNode> GetNeighbours()
        {
            List<AStarNode> neighbours = new List<AStarNode>();

            Node[] worldNeighbours = m_WorldNode.Neighbours;
            for (int i = 0; i < worldNeighbours.Length; ++i)
            {
                Node worldNeighbour = worldNeighbours[i];

                if (worldNeighbour == null)
                    continue;

                //Check if this neighbour is walkable from here (blocked by gates etc...)
                Direction directionFromParent = ((Direction)i);
                if (m_WorldNode.CanEnter(directionFromParent) == false)
                    continue;

                //If so add it to the list!
                neighbours.Add(new AStarNode(worldNeighbour, this, directionFromParent));
            }

            return neighbours;
        }

        public Direction GetDirectionFromParent()
        {
            Node[] neighbours = m_Parent.WorldNode.Neighbours;
            for (int i = 0; i < neighbours.Length; ++i)
            {
                if (m_WorldNode == neighbours[i])
                {
                    return ((Direction)i);
                }
            }

            return Direction.None;
        }

        public Direction GetDirectionToParent()
        {
            Node[] neighbours = m_WorldNode.Neighbours;
            for (int i = 0; i < neighbours.Length; ++i)
            {
                if (m_Parent.WorldNode == neighbours[i])
                {
                    return ((Direction)i);
                }
            }

            return Direction.None;
        }
    }

    private Character m_Character;

    [SerializeField]
    //Can get this from the hacker?
    private Character m_Target;

    [Header("Arrow")]
    [SerializeField]
    private SpriteRenderer m_ArrowSprite;

    [SerializeField]
    private Color m_ArrowColor;

    //Calculating the path before moving makes the AI "smarter".
    //But when it is looking in a certain direction (like the guard) it feels very unexpected and unfair to the players.
    //The "hack" on the other hand doesn't have a direction and can therefore move in the best direction without "suprising" the player.
    private bool m_CalculatePathBeforeMove = false;
    private bool m_CalculatePathAfterMove = false;

    private List<Direction> m_Path;

    public override void Initialize(Character character)
    {
        Initialize(character, true, true);
    }

    public void Initialize(Character character, bool calculateBeforeMove, bool calculateAfterMove)
    {
        m_Character = character;
        m_CalculatePathBeforeMove = calculateBeforeMove;
        m_CalculatePathAfterMove = calculateAfterMove;
    }

    public override void OnEnter()
    {
        m_ArrowSprite.color = m_ArrowColor;
    }

    public override void OnLeave()
    {
        if (m_Path != null)
            m_Path.Clear();
    }

    public override void FrameUpdate()
    {

    }

    public override void LevelUpdate()
    {
        if (m_Target == null)
        {
            Debug.LogError(m_Character.name + "'s ChaseBehaviour doesn't have a target!");
        }
        //Calculate the path if we're allowed to
        if (m_CalculatePathBeforeMove == true)
        {
            CalculatePath();
        }
        else
        {
            //Only calculate one if we don't have one yet.
            if (m_Path == null)
                CalculatePath();

            else if (m_Path.Count <= 0)
                CalculatePath();
        }

        //Move towards our target
        Move();
    }

    private void Move()
    {
        //Move to the first tile of the calculated path
        if (m_Path == null)
            return;

        if (m_Path.Count < 1)
            return;

        Direction nextDirection = m_Path[0];

        if (nextDirection != Direction.None)
        {
            //Actually move the character
            m_Character.LookAt(nextDirection);
            m_Character.MoveToNode(m_Character.CurrentNode.GetNeighbour(nextDirection), nextDirection, false);
        }
    }

    public override void OnMoveEnd()
    {
        //Calculate our path again. We will execute the first move of this path next frame.
        //Calculating the path before moving is obviously better, but this way we communicate very clearly to the player what we're going to do and allow him to "outwit" the AI.
        if (m_CalculatePathAfterMove == true)
        {
            CalculatePath();
        }

        LookAtNextNode();
    }

    private void LookAtNextNode()
    {
        if (m_Path == null)
            return;

        if (m_Path.Count < 1)
            return;

        Direction nextNextDirection = m_Path[0];
        m_Character.LookAt(nextNextDirection);
    }

    private void CalculatePath()
    {
        if (m_Path == null)
            m_Path = new List<Direction>();

        m_Path.Clear();

        //Checks
        if (m_Target == null) { return; }

        Node targetNode = m_Target.CurrentNode;
        if (targetNode == null) { return; }


        //Find the path
        AStarNode finalNode = FindPath(targetNode);
        if (finalNode == null) { return; }


        //Retrace the path
        List<Direction> path = RetracePath(finalNode);
        if (path.Count <= 0) { return; }

        m_Path.AddRange(path);
    }

    private AStarNode FindPath(Node targetNode)
    {
        if (targetNode == null)
            return null;

        //A* pathfinding
        List<AStarNode> openList = new List<AStarNode>();
        List<AStarNode> closedList = new List<AStarNode>();

        //Add the start node to the open list
        AStarNode currentNode = new AStarNode(m_Character.CurrentNode, null);
        openList.Add(currentNode);

        //Loop for as long as there are nodes in the open list
        do
        {
            //Find new currentNode: is the node with the lowest F score (G + H)
            currentNode = openList[0];
            for (int i = 1; i < openList.Count; ++i)
            {
                if (openList[i].F < currentNode.F)
                {
                    currentNode = openList[i];
                }
            }

            //If the current Node is the targetNode, we found it!
            if (currentNode.WorldNode == targetNode)
            {
                //Finish looping, we found it
                return currentNode;
            }

            //Add current node to the closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //Add all neighbours
            foreach (AStarNode neighbour in currentNode.GetNeighbours())
            {
                //If this neighbour is already in the closed list, continue
                if (AStarNodeListContains(closedList, neighbour) != null)
                {
                    continue;
                }

                neighbour.CalculateValues(targetNode);

                //If this neighbour is not in the open list, add it.
                AStarNode foundNode = AStarNodeListContains(openList, neighbour);
                if (foundNode == null)
                {
                    openList.Add(neighbour);
                }

                //If this neighbour was already in the open list, check if getting here this way is more efficient (compare F)
                else
                {
                    if (neighbour.F < foundNode.F)
                    {
                        //If so, overwrite the old one with a new parent & 
                        foundNode.CopyFrom(neighbour);
                    }
                }
            }
        }
        while (openList.Count > 0);

        return null;
    }

    private List<Direction> RetracePath(AStarNode node)
    {
        List<Direction> path = new List<Direction>();
        AStarNode currentNode = node;

        while (currentNode.Parent != null)
        {
            path.Add(currentNode.DirectionFromParent);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    private AStarNode AStarNodeListContains(List<AStarNode> list, AStarNode node)
    {
        foreach (AStarNode currentNode in list)
        {
            if (currentNode.WorldNode == node.WorldNode)
                return currentNode;
        }

        return null;
    }

    public override bool HasPath()
    {
        AStarNode finalNode = FindPath(m_Target.CurrentNode);
        return (finalNode != null);
    }

    public override Node GetNextNode()
    {
        if (m_Path == null)
            return null;

        if (m_Path.Count < 1)
            return null;

        return m_Character.CurrentNode.GetNeighbour(m_Path[0]);
    }

    protected override void OnReset()
    {
        //Forget everything!
        if (m_Path != null)
            m_Path.Clear();
    }

    private void OnDrawGizmos()
    {
        if (enabled == false)
            return;

        if (m_Path == null)
            return;

        //https://forum.unity.com/threads/debug-drawarrow.85980/

        //Only to be used as reference when placing the line renderers

        //Render path
        Node currentNode = GetComponent<Character>().CurrentNode; //GetComponent because we are in editor mode.
        if (currentNode == null)
            return;

        for (int i = 0; i < m_Path.Count; ++i)
        {
            Node nextNode = currentNode.GetNeighbour(m_Path[i]);

            if (currentNode == null || nextNode == null)
                continue;

            Vector3 currentPosition = currentNode.transform.position;
            currentPosition.y += 2;

            Vector3 nextPosition = nextNode.transform.position;
            nextPosition.y += 2;

            Vector3 direction = nextPosition - currentPosition;

            //Line
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(currentPosition, direction);

            //Arrow
            float arrowHeadAngle = 25.0f;
            float arrowHeadLength = 0.5f;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);

            Gizmos.DrawRay(currentPosition + direction, left * arrowHeadLength);
            Gizmos.DrawRay(currentPosition + direction, right * arrowHeadLength);

            currentNode = nextNode;
        }
    }
}
