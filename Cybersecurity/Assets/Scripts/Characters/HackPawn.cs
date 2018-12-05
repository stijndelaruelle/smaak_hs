using System;
using UnityEngine;

public class HackPawn : Character
{
    private EnemyBehaviour m_CurrentBehaviour;
    private EnemyBehaviour m_OriginalBehaviour;
    //private EnemyBehaviour m_NextBehaviour; //We wait one "level tick" before switching so we don't move "unexpectidly"

    [Header("Hack Pawn")]
    [Space(5)]
    [SerializeField]
    private EnemyBehaviour m_PatrolBehaviour;

    [SerializeField]
    private ChaseBehaviour m_ChaseBehaviour;

    [SerializeField]
    private bool m_HasHacker = true;

    //Alert & move animation sequencing issue
    private bool m_IsAlertAnimationPlaying = false;
    private bool m_HasLevelUpdateQueued = false;
    private bool m_HasLevelPlayerUpdateQueued = false;

    public event Action StartMoveEvent;
    public event Action StopMoveEvent;

    public event Action SwitchedToPatrolEvent;
    public event Action SwitchedToChaseEvent;

    protected override void Start()
    {
        base.Start();

        //Assign all behaviors
        if (m_PatrolBehaviour != null)
            m_PatrolBehaviour.Initialize(this);

        if (m_ChaseBehaviour != null)
            m_ChaseBehaviour.Initialize(this, false, true);

        //Assign default behaviour
        m_CurrentBehaviour = m_PatrolBehaviour;

        if (m_CurrentBehaviour == null)
            m_CurrentBehaviour = m_ChaseBehaviour;

        if (m_CurrentBehaviour == null)
            Debug.LogWarning(gameObject.name + " doesn't have any behaviors!");

        m_CurrentBehaviour.OnEnter();
        m_OriginalBehaviour = m_CurrentBehaviour;

        //Listen to events
        LevelDirector.Instance.LevelUpdateEvent += OnLevelUpdate;
        LevelDirector.Instance.LevelPlayerUpdateEvent += OnLevelPlayerUpdate;
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
    }

    private void Update()
    {
        if (m_CurrentBehaviour != null)
            m_CurrentBehaviour.FrameUpdate();
    }

    private void OnLevelUpdate()
    {
        //We only move ourselves when there is no hacker. Otherwise the update order can get mixed up!
        if (m_HasHacker == false)
        {
            LevelUpdateFromHacker();
        }
    }

    public void LevelUpdateFromHacker()
    {
        //Check if we only have to add it to the queue
        if (m_IsAlertAnimationPlaying == true)
        {
            m_HasLevelUpdateQueued = true;
            return;
        }

        //Check if there are players in range
        KillCharactersInRange();

        //Check if we still have a path
        if (m_CurrentBehaviour.HasPath() == false)
        {
            SwitchToPatrol(); //Quick shortcut to also fire the event.
        }
    }

    private void OnLevelPlayerUpdate()
    {
        //We only move ourselves when there is no hacker. Otherwise the update order can get mixed up!
        if (m_HasHacker == false)
        {
            LevelPlayerUpdateFromHacker();
        }
    }

    public void LevelPlayerUpdateFromHacker()
    {
        //Check if we only have to add it to the queue
        if (m_IsAlertAnimationPlaying == true)
        {
            m_HasLevelPlayerUpdateQueued = true;
            return;
        }

        LevelUpdateFromHacker();

        //The game could have ended by now
        if (LevelDirector.Instance.HasGameEnded() == true)
            return;

        m_CurrentBehaviour.LevelUpdate();

        //Check if we lost the player
        if  (m_CurrentBehaviour.HasPath() == false)
        {
            //Respawn
            SwitchToPatrol(); //Start patrolling again
        }

        //Switch behaviour (check reasoning above)
        //if (m_NextBehaviour != null)
        //{
        //    SetBehaviour(m_NextBehaviour);
        //    m_NextBehaviour = null;
        //}
    }

    private bool KillCharactersInRange()
    {
        if (LevelDirector.Instance.HasGameEnded() == true)
            return false;

        //"Kill 'Em All" on your tile! 
        bool success = false;
        foreach (Character character in m_CurrentNode.Characters)
        {
            //Only slay our enemies!
            if (IsEnemy(character))
            {
                //Analytics
                if (LevelDirector.Instance != null)
                    LevelDirector.Instance.CallLevelFailAnalyticsEvent(true, false, false, false);

                character.Die();
                success = true;
            }
        }

        return success;
    }

    private void OnMoveStart()
    {
        //Fire event
        if (StartMoveEvent != null)
            StartMoveEvent();

        //Lame, but we want the game logic to move before the animation. (so we can speed up the game flow)
        base.OnMoveEnd(false);
        KillCharactersInRange();
    }

    protected override void OnMoveEnd(bool snap)
    {
        if (snap)
        {
            base.OnMoveEnd(snap);
            KillCharactersInRange();
        }

        if (m_CurrentBehaviour != null)
            m_CurrentBehaviour.OnMoveEnd();

        //Fire event
        if (StopMoveEvent != null)
            StopMoveEvent();
    }

    //Called by the "hacker"
    public void SwitchToChase()
    {
        if (m_CurrentBehaviour == m_ChaseBehaviour)
            return;

        //Animation etc.
        //m_NextBehaviour = m_ChaseBehaviour;
        SetBehaviour(m_ChaseBehaviour);

        //Animation
        if (m_Animator != null)
        {
            m_Animator.SetTrigger("Alert");
            m_IsAlertAnimationPlaying = true;

            if (LevelDirector.Instance != null)
                LevelDirector.Instance.AddInputBlocker("HackPawn: Switch To Chase (Alert)");
        }

        if (SwitchedToChaseEvent != null)
            SwitchedToChaseEvent();
    }

    public void SwitchToPatrol()
    {
        if (m_CurrentBehaviour == m_PatrolBehaviour)
            return;

        //m_NextBehaviour = m_PatrolBehaviour;
        SetBehaviour(m_PatrolBehaviour);

        if (SwitchedToPatrolEvent != null)
            SwitchedToPatrolEvent();
    }

    private void SetBehaviour(EnemyBehaviour behaviour)
    {
        if (m_CurrentBehaviour != null)
            m_CurrentBehaviour.OnLeave();

        m_CurrentBehaviour = behaviour;

        m_CurrentBehaviour.OnEnter();
    }

    public bool IsChasing()
    {
        return (m_CurrentBehaviour == m_ChaseBehaviour);
    }

    //Called by the animation forwarder
    public void OnMoveAnimationStart()
    {
        OnMoveStart();
    }

    public void OnAlertAnimationEnd()
    {
        m_IsAlertAnimationPlaying = false;

        //Execute levelupdates that were left hanging
        if (m_HasLevelUpdateQueued == true)
        {
            LevelUpdateFromHacker();
            m_HasLevelUpdateQueued = false;
        }

        if  (m_HasLevelPlayerUpdateQueued == true)
        {
            LevelPlayerUpdateFromHacker();
            m_HasLevelPlayerUpdateQueued = false;
        }

        if (LevelDirector.Instance != null)
            LevelDirector.Instance.RemoveInputBlocker("HackPawn: On Alert Animation End");
    }

    //ResetableObject
    protected override void OnReset()
    {
        base.OnReset();

        SetBehaviour(m_OriginalBehaviour);
        //m_NextBehaviour = null;
    }
}
