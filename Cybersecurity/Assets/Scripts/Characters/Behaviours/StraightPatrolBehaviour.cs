using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[RequireComponent (typeof(Character))]
public class StraightPatrolBehaviour : EnemyBehaviour
{
    [SerializeField]
    private Direction m_StartDirection;
    private Direction m_CurrentDirection;

    [Header("Arrow")]
    [SerializeField]
    private SpriteRenderer m_ArrowSprite;

    [SerializeField]
    private Color m_ArrowColor;

    private Character m_Character;

    public override void Initialize(Character character)
    {
        m_Character = character;
        m_CurrentDirection = m_StartDirection;
    }

    public override void OnEnter()
    {
        m_ArrowSprite.color = m_ArrowColor;
    }

    public override void OnLeave()
    {
        
    }

    public override void FrameUpdate()
    {
        
    }

    public override bool HasPath()
    {
        Direction nextDirection = GetNextDirection();
        return (nextDirection != Direction.None);
    }

    public override Node GetNextNode()
    {
        return m_Character.CurrentNode.GetNeighbour(m_CurrentDirection);
    }

    public override void LevelUpdate()
    {
        Move();
    }

    //Make an update for when the player uses a gate or something (the level updates, but the player didn't move. Like throwing a stone in Hitman go)
    //In this case we update our rotation to show where we will go (in case a gate opened)

    private void Move()
    {
        //Find the next node
        Direction nextDirection = GetNextDirection();

        if (nextDirection == Direction.None)
        {
            //Debug.LogWarning(gameObject.name + " is stuck!");
            return;
        }

        m_CurrentDirection = nextDirection;

        //Actually move the character
        m_Character.LookAt(m_CurrentDirection);
        m_Character.MoveToNode(m_Character.CurrentNode.GetNeighbour(m_CurrentDirection), m_CurrentDirection, false);
    }

    public override void OnMoveEnd()
    {
        //Rotate the character to face where we are going next
        Direction nextNextDirection = GetNextDirection();

        if (nextNextDirection == Direction.None)
            return;

        m_Character.LookAt(nextNextDirection);
        m_CurrentDirection = nextNextDirection;
    }

    private Direction GetNextDirection()
    {
        Direction nextDirection = m_CurrentDirection;

        //Find the next node
        bool canEnter = m_Character.CurrentNode.CanEnter(m_CurrentDirection);

        if (canEnter == false)
        {
            //Turn around
            Direction tempDirection = UtilityMethods.InverseDirection(m_CurrentDirection);

            //Can we enter now?
            canEnter = m_Character.CurrentNode.CanEnter(tempDirection);

            if (canEnter == false)
            {
                return Direction.None;
            }

            nextDirection = tempDirection;
        }

        //Actually move the character
        return nextDirection;
    }

    protected override void OnReset()
    {
        //Forget everything!
        m_CurrentDirection = m_StartDirection;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (enabled == false)
            return;

        //https://forum.unity.com/threads/debug-drawarrow.85980/

        //Only to be used as reference when placing the line renderers

        //Render path
        Node currentNode = GetComponent<Character>().CurrentNode; //GetComponent because we are in editor mode.
        if (currentNode == null)
            return;

        Node nextNode = currentNode.GetNeighbour(m_CurrentDirection);

        if (currentNode == null || nextNode == null)
            return;

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
        Handles.Label(nextPosition, "Straight Patrol");
    }
#endif
}