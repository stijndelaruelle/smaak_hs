using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatablockPickup : Pickup
{
    public delegate void DatablockPickupEvent(DatablockPickup datablockPickup, Character character);

    [SerializeField]
    private QuestionPoolDefinition m_QuestionPool;
    public QuestionPoolDefinition QuestionPool
    {
        get { return m_QuestionPool; }
    }

    [SerializeField]
    private Animator m_Animator;

    //Temp wiggle animation
    private Vector3 m_OriginalPosition;
    private Quaternion m_OriginalRotation;

    private float m_AnimationTimer;
    private Character m_CurrentCharacter;

    public event DatablockPickupEvent PickupEvent;

    protected override void Awake()
    {
        base.Awake();
        //m_OriginalPosition = m_Transform.position;
        //m_OriginalRotation = m_Transform.rotation;
    }

    protected override void Start()
    {
        base.Start();
        m_Animator.SetTrigger("Idle");
    }

    private void Update()
    {
        /*
        m_AnimationTimer += Time.deltaTime;

        if (m_AnimationTimer > (Mathf.PI * 2))
            m_AnimationTimer -= (Mathf.PI * 2);


        //Temp animation
        m_Transform.position = new Vector3(m_Transform.position.x, m_OriginalPosition.y + Mathf.Sin(m_AnimationTimer) * 0.5f, m_Transform.position.z);
        m_Transform.rotation = m_OriginalRotation * Quaternion.Euler(0, Mathf.Sin(2 * m_AnimationTimer) * 20, 0);
        */
    }

    public override void OnCharacterEnter(Character character, Direction direction, bool snap)
    {
        if (gameObject.activeSelf == false)
            return;

        if (IsEnabled() == false)
            return;

        m_CurrentCharacter = character;

        if (PickupEvent != null)
            PickupEvent(this, character);
        else
            OnQuizCompleted(true);
    }

    public void OnQuizCompleted(bool success)
    {
        if (m_CurrentCharacter == null)
            return;

        if (gameObject.activeSelf == false)
        {
            m_CurrentCharacter = null;
            return;
        }

        if (success)
        {
            //Add to inventory
            Inventory inventory = m_CurrentCharacter.Inventory;

            if (inventory != null)
            {
                inventory.AddItem(m_Item, m_Amount);
                m_IsEnabled = false;

                //Animate the datablock
                m_Animator.SetTrigger("Disappear");

                //Tell the hint system
                FireHintUsedEvent(m_CurrentCharacter);
            }

            m_CurrentCharacter = null;
        }
        else
        {
            //Analytics
            if (LevelDirector.Instance != null)
                LevelDirector.Instance.CallLevelFailAnalyticsEvent(false, true, false, false);

            m_CurrentCharacter.Die();
            return;
        }
    }

    protected override void OnReset()
    {
        base.OnReset();

        /*
        m_Transform.position = m_OriginalPosition;
        m_Transform.rotation = m_OriginalRotation;
        m_AnimationTimer = 0.0f;
        */

        m_CurrentCharacter = null;
        m_Animator.SetTrigger("Idle");
    }
}
