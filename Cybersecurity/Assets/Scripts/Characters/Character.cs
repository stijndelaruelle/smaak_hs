using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : ResetableObject
{
    [Header("General (Character)")]
    [Space(5)]
    [SerializeField]
    private Node m_StartNode;
    public Node StartNode
    {
        get { return m_StartNode; }
    }

    protected Node m_CurrentNode;
    public Node CurrentNode
    {
        get { return m_CurrentNode; }
    }

    [SerializeField]
    private FactionTypeDefinition m_Faction;
    public FactionTypeDefinition Faction
    {
        get { return m_Faction; }
    }

    [SerializeField]
    protected Inventory m_Inventory;
    public Inventory Inventory
    {
        get { return m_Inventory; }
    }

    [SerializeField]
    protected Animator m_Animator;

    //TODO: Create a state machine?
    protected bool m_IsDead = false;
    protected bool m_IsMoving = false;
    protected bool m_IsUsing = false;

    private Node m_NextNode;
    private Direction m_NextDirection;

    protected Transform m_Transform;
    private Quaternion m_OriginalRotation; //Used OnReset
    private Transform m_OriginalParent; //Moving

    protected virtual void Awake()
    {
        m_Transform = transform;
        m_OriginalRotation = m_Transform.localRotation;
    }

    protected override void Start()
    {
        base.Start();

        LevelDirector.Instance.LevelStartEvent += OnLevelStart;

        m_OriginalParent = m_Transform.parent;

        if (m_Animator != null)
            m_Animator.SetTrigger("Idle");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (LevelDirector.Instance != null)
            LevelDirector.Instance.LevelStartEvent -= OnLevelStart;
    }

    private void OnLevelStart()
    {
        //Set to start position
        MoveToNode(m_StartNode, Direction.None, true);
    }


    public virtual void Die()
    {
        if (m_IsDead == true)
            return;

        //No complex damage system so far, it's INSTAGIB
        m_IsDead = true;

        if (m_Animator != null)
            m_Animator.SetTrigger("Die");
    }


    public void LookAt(Direction direction)
    {
        if (direction == Direction.None)
            return;

        Vector3 vecDir = Vector3.zero;

        if (direction == Direction.North) { vecDir = new Vector3(0, 0, 1);  }
        if (direction == Direction.South) { vecDir = new Vector3(0, 0, -1); }
        if (direction == Direction.West)  { vecDir = new Vector3(-1, 0, 0); }
        if (direction == Direction.East)  { vecDir = new Vector3(1, 0, 0);  }

        //Rotate character
        Vector3 normalDir = vecDir.normalized;
        float desired = Mathf.Atan2(normalDir.x, normalDir.z) * Mathf.Rad2Deg;
        m_Transform.rotation = Quaternion.Euler(new Vector3(0, desired, 0));

        //Reset animation (in case we started to use someting)
        if (m_IsUsing == true)
        {
            m_IsUsing = false;

            if (m_Animator != null)
                m_Animator.SetTrigger("Idle");
        }
    }

    public bool MoveToNode(Node node, Direction direction, bool snap = false)
    {
        if (node == null)
            return false;

        //Check if the animation is still running, if so: cancel out
        if (m_IsMoving)
            return false;

        //Can we go to this node?
        if (m_CurrentNode != null && m_CurrentNode.CanEnter(direction) == false)
        {
            //Sound
            return false;
        }

        LevelDirector.Instance.AddInputBlocker("Character: MoveToNode");

        //Leave current node
        if (m_CurrentNode != null)
            m_CurrentNode.OnLeave(this, direction);

        m_NextNode = node;
        m_NextDirection = direction;

        if (snap != true)
        {
            m_IsMoving = true;

            if (m_Animator != null)
                m_Animator.SetTrigger("Walk");
        }
        else
        {
            //Move character imediatly
            OnMoveEnd(snap);
        }

        m_IsUsing = false;
        return true;
    }

    protected virtual void OnMoveEnd(bool snap)
    {
        m_IsMoving = false;

        m_Transform.position = m_NextNode.GetPosition();
        m_CurrentNode = m_NextNode;

        if (m_CurrentNode != null)
            m_CurrentNode.OnEnter(this, m_NextDirection, snap);

        LevelDirector.Instance.RemoveInputBlocker("Character: OnMoveEnd");
    }

    public void TeleportToNode(Node node)
    {
        m_NextNode = node;

        //Start the teleport animation
        if (m_Animator != null)
            m_Animator.SetTrigger("TeleportUp");

        //Start the shader

        //Play a sound

        LevelDirector.Instance.AddInputBlocker("Character: Teleport To Node");
    }

    private void OnTeleportUpEnd()
    {
        //We are "in the void" at this point

        //Snap back
        MoveToNode(m_NextNode, Direction.None, true);

        //Play the move back down animation
        if (m_Animator != null)
            m_Animator.SetTrigger("TeleportDown");

        //Reverse the shader
    }

    private void OnTeleportDownEnd()
    {
        //we arrived back on the level at the point
        LevelDirector.Instance.RemoveInputBlocker("Character: OnTeleportDownEnd");
    }

    protected bool UseNode(Direction direction)
    {
        if (m_IsUsing)
            return false;

        if (m_CurrentNode == null)
            return false;

        bool canUse = m_CurrentNode.CanUse(this, direction);
        if (canUse == false)
            return false;

        m_NextDirection = direction;
        m_IsUsing = true;

        //Make sure we don't trigger the activate animation when there is nothing to activate
        if (m_Animator != null)
        {
            m_Animator.SetTrigger("Activate");
            return true;
        }

        return false;
    }

    private void OnStartUsing()
    {
        //Callback from the animation (when he actually starts hacking)
        if (m_CurrentNode == null)
            return;

        m_CurrentNode.OnUse(this, m_NextDirection);
        m_IsUsing = false;
    }

    public void MoveWithTransform(Transform newParent)
    {
        //We're already moving
        m_Transform.parent = newParent;
    }

    public void StopMoveWithTransform()
    {
        m_Transform.parent = m_OriginalParent;
    }

    //Utility
    public bool IsAlly(Character character)
    {
        return m_Faction.IsAlly(character.Faction);
    }

    public bool IsEnemy(Character character)
    {
        return m_Faction.IsEnemy(character.Faction);
    }

    public bool IsNeutral(Character character)
    {
        return m_Faction.IsNeutral(character.Faction);
    }


    //Animation Callbacks
    public void OnUseAnimationStartUsing()
    {
        OnStartUsing();
    }

    public void OnMoveAnimationEnd()
    {
        OnMoveEnd(false);
    }

    public void OnTeleportUpAnimationEnd()
    {
        //The "go up" teleport
        OnTeleportUpEnd();
    }

    public void OnTeleportDownAnimationEnd()
    {
        //The "go down" teleport
        OnTeleportDownEnd();
    }

    //ResetableObject
    protected override void OnReset()
    {
        m_IsDead = false;
        m_IsMoving = false;

        MoveToNode(m_StartNode, Direction.None, true);
        m_Transform.localRotation = m_OriginalRotation;

        //Reset parent
        m_Transform.parent = m_OriginalParent;

        if (m_Animator != null)
            m_Animator.SetTrigger("Idle");
    }
}

