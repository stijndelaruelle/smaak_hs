using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacker : Character
{
    [Header("Hacker")]
    [Space(5)]
    //Stands on the sidelines, and looks in a straight line for the player
    //One he sees the player, he put's his "hack" in chase mode
    //Once the hack doesn't find a path to the player anymore, the hack is reset.
    [SerializeField]
    private HackPawn m_HackPawn;

    [SerializeField]
    private Character m_Target;

    [SerializeField]
    //Moveable tiles can jump in and out of sight, for this reason we have to know where we are looking (so we can check wether or not they are within our line of sight)
    private Direction m_Direction;

    //I would like to have used a generic "range" number so we can potentionally rotate the hacker or something similar
    //This is not possible as the hacker can look over gaps to spot the player.
    //These gaps are not neighbours of nodes.
    [SerializeField]
    [Tooltip("Please order them in the order the hacker can see them. As another character or visionblocking level object can stop his line of sight.")]
    private List<Node> m_VisionRange;

    protected override void Start()
    {
        base.Start();

        LevelDirector.Instance.LevelUpdateEvent += OnLevelUpdate;
        LevelDirector.Instance.LevelPlayerUpdateEvent += OnLevelPlayerUpdate;

        if (m_Target == null)
            Debug.Log(gameObject.name + ": Hacker doesn't have a target!");

        if (m_HackPawn == null)
        {
            Debug.Log(gameObject.name + ": Hacker doesn't have a HackPawn!");
            return;
        }

        m_HackPawn.StartMoveEvent += OnPawnStartMove;
        m_HackPawn.StopMoveEvent += OnPawnStopMove;
        m_HackPawn.SwitchedToPatrolEvent += OnPawnSwitchedToPatrol;
        m_HackPawn.SwitchedToChaseEvent += OnPawnSwitchedToChase;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        LevelDirector levelManager = LevelDirector.Instance;

        if (levelManager != null)
        {
            levelManager.LevelUpdateEvent -= OnLevelUpdate;
            levelManager.LevelPlayerUpdateEvent -= OnLevelPlayerUpdate;
        }

        if (m_HackPawn != null)
        {
            m_HackPawn.StartMoveEvent -= OnPawnStartMove;
            m_HackPawn.StopMoveEvent -= OnPawnStopMove;
            m_HackPawn.SwitchedToPatrolEvent -= OnPawnSwitchedToPatrol;
            m_HackPawn.SwitchedToChaseEvent -= OnPawnSwitchedToChase;
        }
    }

    private void OnLevelUpdate()
    {
        if (m_HackPawn == null)
            return;

        //If our hack is not yet in chase mode
        if (m_HackPawn.IsChasing() == false)
        {
            //Check if we can see our target
            foreach (Node node in m_VisionRange)
            {
                //Check if this node is within our line of sight (mostly for moveable tiles)
                bool isWithinView = IsNodeWithinView(node);
                if (isWithinView && node.ContainsCharacter(m_Target) == true)
                {
                    //If so, set our hackpawn to chase mode
                    m_HackPawn.SwitchToChase();
                }
            }
        }

        m_HackPawn.LevelUpdateFromHacker();
    }

    private void OnLevelPlayerUpdate()
    {
        OnLevelUpdate();

        //The game could have ended by now
        if (LevelDirector.Instance.HasGameEnded() == true)
            return;

        m_HackPawn.LevelPlayerUpdateFromHacker();
    }

    private bool IsNodeWithinView(Node node)
    {
        //The dot product would be 0, edge case.
        if (m_CurrentNode == node)
            return true;

        Vector3 ourPosition = Vector3.zero;
        if (m_CurrentNode == null)
        {
            ourPosition = transform.position;
        }
        else
        {
            ourPosition = m_CurrentNode.GetPosition();
        }

        Vector3 normalDiff = (node.GetPosition() - ourPosition).normalized;

        switch (m_Direction)
        {
            case Direction.North:
            {
                float dot = Vector3.Dot(new Vector3(0, 0, 1), normalDiff);
                return (dot > 0.9f); //Allow a little wiggle room.   
            }

            case Direction.South:
            {
                float dot = Vector3.Dot(new Vector3(0, 0, -1), normalDiff);
                return (dot > 0.9f);
            }

            case Direction.East:
            {
                float dot = Vector3.Dot(new Vector3(1, 0, 0), normalDiff);
                return (dot > 0.9f);
            }

            case Direction.West:
            {
                float dot = Vector3.Dot(new Vector3(-1, 0, 0), normalDiff);
                return (dot > 0.9f);
            }

            default:
                return false;
        }
    }

    //Pawn events
    private void OnPawnStartMove()
    {
        if (m_HackPawn.IsChasing() && m_Animator != null)
        {   
            m_Animator.SetTrigger("HeavyHacking");
        }
    }

    private void OnPawnStopMove()
    {
        if (m_HackPawn.IsChasing() && m_Animator != null)
        {
            m_Animator.SetTrigger("IdleHacking");
        }
    }

    private void OnPawnSwitchedToPatrol()
    {
        if (m_Animator != null)
            m_Animator.SetTrigger("PlayerLost");
    }

    private void OnPawnSwitchedToChase()
    {
        if (m_Animator != null)
            m_Animator.SetTrigger("PlayerSeen");
    }

    //ResetableObject
    protected override void OnReset()
    {
        base.OnReset();
    }

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

        for (int i = 1; i < m_VisionRange.Count; ++i)
        {
            Node nextNode = m_VisionRange[i];

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
