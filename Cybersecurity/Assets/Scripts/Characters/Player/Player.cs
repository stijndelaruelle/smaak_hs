using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : Character
{
    private Direction m_Direction = Direction.North;

    [Header("Player")]
    [Space(5)]
    [SerializeField]
    private Direction m_StartDirection = Direction.North;

    [SerializeField]
    private float m_MinIdleVariationTime;

    [SerializeField]
    private float m_MaxIdleVariationTime;
    private float m_CurrentIdleVariationTime;
    private float m_IdleVariationTimer;

    //Touch controls
    private Vector3 m_StartTouchPosition;
    private float m_MinDragDistance;
    private float m_MinOffsetDistance;

    //Events
    public event Action RespawnEvent;
    public event Action IntoAnimationEndEvent;

    protected override void Start()
    {
        base.Start();

        m_MinDragDistance = (Screen.height * 5) / 100; //Only register moves if the player has swiped at least 10% of the screen height

        m_Direction = m_StartDirection;
        LookAt(m_Direction);

        ResetRandomIdleVariation();

        if (LevelDirector.Instance != null)
            LevelDirector.Instance.LevelEndVictoryEvent += OnVictory;

        if (m_Inventory != null)
            m_Inventory.ItemChangedEvent += OnDatablockPickup; //Not really, but it's literally the only item we have left in the game.

        PlayIntroAnimation();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (LevelDirector.Instance != null)
            LevelDirector.Instance.LevelEndVictoryEvent -= OnVictory;

        if (m_Inventory != null)
            m_Inventory.ItemChangedEvent -= OnDatablockPickup;
    }

    private void Update()
    {
        if (m_IsMoving == true || m_IsDead == true)
            return;

        HandleKeyboardInput();
        HandleTouchInput();
        HandleIdleAnimation();
    }

    private void HandleKeyboardInput()
    {
        //Check if we are allowed to do anything
        if (LevelDirector.Instance.HasPlayerInput() == false)
            return;

        //Keyboard input can be used again, but we're not teaching the player. So they always have the mouse to fall back to
        if (Input.GetKeyDown(KeyCode.UpArrow))    { Move(Direction.North); }
        if (Input.GetKeyDown(KeyCode.DownArrow))  { Move(Direction.South); }
        if (Input.GetKeyDown(KeyCode.LeftArrow))  { Move(Direction.West); }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { Move(Direction.East); }

        //Use
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            //Transitioning from death is a bit weird. Can be fixed, by not allowing to activate on the first frame of being reset.
            UseNode(m_Direction);
        }

        //Debug
        if (SaveGameManager.GetBool(SaveGameManager.SAVE_CHEATS) == true)
        {
            
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
            {
                Die();
            }
        }
    }

    private void HandleTouchInput()
    {
        //Check if we are allowed to do anything
        if (LevelDirector.Instance.HasPlayerInput() == false)
            return;

        Vector3 lastTouchPosition = Vector3.zero;
        bool analyseSwipe = false;

        //Detault to touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            //Start touching
            if (touch.phase == TouchPhase.Began)
            {
                m_StartTouchPosition = touch.position;
            }

            //Stop touching
            else if (touch.phase == TouchPhase.Ended)
            {
                lastTouchPosition = touch.position;
                analyseSwipe = true;
            }
        }

        //If no touch can be found, fallback to mouse
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_StartTouchPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                lastTouchPosition = Input.mousePosition;
                analyseSwipe = true;
            }
        }

        //Analyse
        if (analyseSwipe)
        {
            Vector3 diff = lastTouchPosition - m_StartTouchPosition;

            if (Mathf.Abs(diff.x) >= m_MinDragDistance && Mathf.Abs(diff.y) >= m_MinDragDistance)
            {
                //Debug.Log("Start drag: " + m_StartTouchPosition + " end drag: " + lastTouchPosition + " min drag distance on both axis: " + m_MinDragDistance);

                //Top Left
                if (diff.x < 0 && diff.y >= 0)
                {
                    Move(Direction.West);
                }

                //Top Right
                else if (diff.x >= 0 && diff.y >= 0)
                {
                    Move(Direction.North);
                }

                //Bottom Left
                else if(diff.x < 0 && diff.y < 0)
                {
                    Move(Direction.South);
                }

                //Bottom right
                else if(diff.x >= 0 && diff.y < 0)
                {
                    Move(Direction.East);
                }

                //else
                //{
                //    Debug.Log("Too straight of a line, lower that min offset distance");
                //}
            }
            else
            {
                //Debug.Log("Lower than min drag distance, swipe ignored");
            }
        }
    }

    private void HandleIdleAnimation()
    {
        //If we are idling, sometimes change to a variation
        m_IdleVariationTimer += Time.deltaTime;

        if (m_IdleVariationTimer >= m_CurrentIdleVariationTime)
        {
            m_Animator.SetTrigger("IdleVariation");
            ResetRandomIdleVariation();
        }
    }

    private void ResetRandomIdleVariation()
    {
        m_IdleVariationTimer = 0;
        m_CurrentIdleVariationTime = Random.Range(m_MinIdleVariationTime, m_MaxIdleVariationTime);
    }

    public void Move(Direction direction)
    {
        //Check if we are allowed to do anything
        if (LevelDirector.Instance.HasPlayerInput() == false)
            return;

        m_Direction = direction;

        //Use
        LookAt(m_Direction);

        //Try to move
        Node targetNode = m_CurrentNode.GetNeighbour(m_Direction);
        MoveToNode(targetNode, m_Direction, false);

        ResetRandomIdleVariation();
    }

    public void Move(RelativeDirection relativeDirection)
    {
        //Check if we are allowed to do anything
        if (LevelDirector.Instance.HasPlayerInput() == false)
            return;

        switch (relativeDirection)
        {
            case RelativeDirection.Forward:
            {
                Move(m_Direction);
                break;
            }

            case RelativeDirection.Backward:
            {
                Move(UtilityMethods.RotateDirection(m_Direction, 2));
                break;
            }

            case RelativeDirection.Left:
            {
                Move(UtilityMethods.RotateDirection(m_Direction, -1));
                break;
            }

            case RelativeDirection.Right:
            {
                Move(UtilityMethods.RotateDirection(m_Direction, 1));
                break;
            }

            default:
                break;
        }
    }

    protected override void OnMoveEnd(bool snap)
    {
        base.OnMoveEnd(snap);

        //Make sure respawning & teleporting doesn't cause the game to update
        if (snap == false) { LevelDirector.Instance.PlayerMoved(); }
        else               { LevelDirector.Instance.RequestLevelUpdate(); }

        ResetRandomIdleVariation();
    }

    public void Use(Direction direction)
    {
        //Check if we are allowed to do anything
        if (LevelDirector.Instance.HasPlayerInput() == false)
            return;

        if (direction != m_Direction)
            LookAt(direction);

        m_Direction = direction;

        UseNode(m_Direction);
        ResetRandomIdleVariation();
    }

    private void PlayIntroAnimation()
    {
        LevelDirector.Instance.AddInputBlocker("Player: Play Intro Animation");
        //Respawn animation
        if (m_Animator != null)
            m_Animator.SetTrigger("Intro");
    }

    public void OnIntroAnimationEnd()
    {
        LevelDirector.Instance.RemoveInputBlocker("Player: Intro Animation End");

        if (IntoAnimationEndEvent != null)
            IntoAnimationEndEvent();
    }

    public override void Die()
    {
        if (m_IsDead == true)
            return;

        base.Die();
        LevelDirector.Instance.PlayerDied();
    }

    //LevelManager events
    private void OnVictory()
    {
        //ALL VICTORY STUFF HAS BEEN MOVED TO A PLAYABLE DIRECTOR!

        //Play victory animation!
        /*if (m_Animator != null)
        {
            //90% to cheer, 10% chance to dance
            int rand = Random.Range(0, 100);

            if (rand < 90)
                m_Animator.SetTrigger("Victory");
            else
                m_Animator.SetTrigger("Dance");
        }*/

        //Rotate to the camera
        //m_Transform.DORotate(new Vector3(0.0f, 120.0f, 0.0f), 0.5f, RotateMode.Fast);
    }

    //Inventory events
    private void OnDatablockPickup(int itemSlot, ItemAmountPair itemAmountPair)
    {
        //LAME
        if  (itemAmountPair.Item.ReferenceID == "DATABLOCK" && itemAmountPair.Amount > 0)
        {
            if (m_Animator != null)
                m_Animator.SetTrigger("Pickup");
        }
    }

    //Resetable Object
    protected override void OnReset()
    {
        base.OnReset();

        m_Direction = m_StartDirection;
        PlayIntroAnimation();
    }
}
