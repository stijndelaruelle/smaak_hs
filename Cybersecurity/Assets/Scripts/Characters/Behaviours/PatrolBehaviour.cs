using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[RequireComponent (typeof(Character))]
public class PatrolBehaviour : EnemyBehaviour
{
    public enum LoopType
    {
        None = 0,
        Loop = 1, //When the character finished the path, they will start back from the beginning. (make sure the path is an actual loop or interesting things may occur!)
        PingPong = 2 //When this character reached the end of the path or get's stuck they will try to walk the path in reverse.
    }

    public struct PathInfo
    {
        private int m_PathID;
        public int PathID
        {
            get{return m_PathID; }
        }

        private bool m_IsReversing;
        public bool IsReversing
        {
            get { return m_IsReversing; }
        }

        public PathInfo(int pathID, bool isReversing)
        {
            m_PathID = pathID;
            m_IsReversing = isReversing;
        }
    }

    private Character m_Character;

    [SerializeField]
    private List<Direction> m_Path; //Could also list a list of tiles, but I think this will be easier in the end? (neighbours can change etc, and this way we never end up doing illigal moves)
    private int m_CurrentPathID = -1;

    [SerializeField]
    private LoopType m_LoopType = LoopType.None;

    [SerializeField]
    private bool m_ReverseWhenStuck = false;
    private bool m_IsReversing = false;

    [Header("Arrow")]
    [SerializeField]
    private SpriteRenderer m_ArrowSprite;

    [SerializeField]
    private Color m_ArrowColor;

    public override void Initialize(Character character)
    {
        m_Character = character;
    }

    public override void OnEnter()
    {
        m_CurrentPathID = -1;
        m_ArrowSprite.color = m_ArrowColor;
    }

    public override void OnLeave()
    {

    }

    public override void FrameUpdate()
    {

    }

    public override void LevelUpdate()
    {
        Move();
    }

    //Make an update for when the player uses a gate or something (the level updates, but the player didn't move. Like throwing a stone in Hitman go)
    //In this case we update our rotation to show where we will go (in case a gate opened)

    private void Move(bool recursive = false)
    {
        if (m_Path == null)
            return;

        if (m_Path.Count <= 0)
            return;

        PathInfo nextPathInfo = GetNextPathInfo(m_CurrentPathID, m_IsReversing);
        Direction nextDirection = Direction.None;

        if (nextPathInfo.PathID >= 0)
        {
            nextDirection = m_Path[nextPathInfo.PathID];
            if (nextPathInfo.IsReversing) { nextDirection = UtilityMethods.InverseDirection(m_Path[nextPathInfo.PathID]); }

            //Only update if valid, if we are stuck we want to try again with the same parameters
            m_CurrentPathID = nextPathInfo.PathID;
            m_IsReversing = nextPathInfo.IsReversing;
        }

        if (nextDirection != Direction.None)
        {
            //Actually move the character
            m_Character.LookAt(nextDirection);
            m_Character.MoveToNode(m_Character.CurrentNode.GetNeighbour(nextDirection), nextDirection, false);
        }
    }

    public override void OnMoveEnd()
    {
        //Rotate the character to face where we are going next
        LookAtNextNode();
    }

    private void LookAtNextNode()
    {
        PathInfo nextNextPathInfo = GetNextPathInfo(m_CurrentPathID, m_IsReversing);
        Direction nextNextDirection = Direction.None;

        if (nextNextPathInfo.PathID >= 0)
        {
            nextNextDirection = m_Path[nextNextPathInfo.PathID];
            if (nextNextPathInfo.IsReversing) { nextNextDirection = UtilityMethods.InverseDirection(m_Path[nextNextPathInfo.PathID]); }
        }

        if (nextNextDirection != Direction.None)
            m_Character.LookAt(nextNextDirection);
    }

    //Returns the NextPathID & wether or not we are reversing
    //This function does not change the state of this object!
    private PathInfo GetNextPathInfo(int startPathID, bool isReversing, bool recursive = false)
    {
        Direction nextDirection = Direction.None;
        int nextPathID = startPathID;

        //Find our next node (not checking obstacles)
        if (isReversing == false)
        {
            nextPathID += 1;

            //Check if we reached the end of the path
            if (nextPathID >= m_Path.Count)
            {
                if (m_LoopType == LoopType.Loop)
                {
                    nextPathID = 0;
                }
                else
                {
                    nextPathID = m_Path.Count - 1;

                    if (m_LoopType == LoopType.PingPong)
                    {
                        isReversing = true;
                    }
                }
            }
        }
        else
        {
            nextPathID -= 1;

            //Check if we reached the beginning of the path
            if (nextPathID < 0)
            {
                if (m_LoopType == LoopType.Loop)
                {
                    //Only works when the loop can always be completed!
                    nextPathID = m_Path.Count - 1;
                }
                else
                {
                    nextPathID = 0;

                    if (m_LoopType == LoopType.PingPong)
                    {
                        isReversing = false;
                    }
                }
            }
        }

        //Check if we can actually go there at this moment in time
        nextDirection = m_Path[nextPathID];
        if (isReversing) { nextDirection = UtilityMethods.InverseDirection(m_Path[nextPathID]); }

        bool canEnter = m_Character.CurrentNode.CanEnter(nextDirection);

        if (canEnter == false)
        {
            //If not, (and we haven't tried before) reverse and try again.
            if (recursive == false && m_ReverseWhenStuck)
            {
                isReversing = !isReversing;
                return GetNextPathInfo(nextPathID, isReversing, true);
            }
            else
            {
                Debug.LogWarning("GetNextDirection: " + gameObject.name + " is stuck!");
                return new PathInfo(-1, false);
            }
        }

        return new PathInfo(nextPathID, isReversing);
    }

    public override bool HasPath()
    {
        if (m_Path == null)
            return false;

        if (m_Path.Count == 0)
            return false;

        return true;
    }

    public override Node GetNextNode()
    {
        PathInfo nextNextPathInfo = GetNextPathInfo(m_CurrentPathID, m_IsReversing);
        Direction nextNextDirection = Direction.None;

        if (nextNextPathInfo.PathID >= 0)
        {
            nextNextDirection = m_Path[nextNextPathInfo.PathID];
            if (nextNextPathInfo.IsReversing) { nextNextDirection = UtilityMethods.InverseDirection(m_Path[nextNextPathInfo.PathID]); }
        }

        if (nextNextDirection == Direction.None)
            return null;

        return m_Character.CurrentNode.GetNeighbour(nextNextDirection);
    }

    protected override void OnReset()
    {
        //Forget everything!
        m_CurrentPathID = -1;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (enabled == false)
            return;

        //https://forum.unity.com/threads/debug-drawarrow.85980/

        //Only to be used as reference when placing the line renderers

        //Render path
        Node currentNode = GetComponent<Character>().StartNode; //GetComponent because we are in editor mode.
        if (currentNode == null)
            return;

        if (m_Path == null)
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
            Gizmos.color = Color.red;
            Gizmos.DrawRay(currentPosition, direction);

            //Arrow
            float arrowHeadAngle = 25.0f;
            float arrowHeadLength = 0.5f;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);

            Gizmos.DrawRay(currentPosition + direction, left * arrowHeadLength);
            Gizmos.DrawRay(currentPosition + direction, right * arrowHeadLength);

            currentNode = nextNode;

            //Draw the loop type at the last node
            if (i >= m_Path.Count -1 && m_LoopType != LoopType.None)
            {
                Handles.Label(nextPosition, m_LoopType.ToString());
            }
        }
    }
#endif
}