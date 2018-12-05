using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerClickToUseCollider : MonoBehaviour
{
    [SerializeField]
    private Direction m_Direction;

    [Header("Required References")]
    [SerializeField]
    private Player m_Player;

    [SerializeField]
    private LevelObject m_LevelObject;
    private Collider m_Collider;

    private void Start()
    {
        m_Collider = GetComponent<Collider>();
        m_Collider.enabled = false;

        if (m_LevelObject == null)
        {
            Debug.LogWarning("'PlayerClickToUseCollider' not linked with a levelobject, will always be enabled!");
            return;
        }

        m_LevelObject.CharacterEnterEvent += OnCharacterEnterEvent;
        m_LevelObject.CharacterLeaveEvent += OnCharacterLeaveEvent;
    }

    private void OnDestroy()
    {
        if (m_LevelObject == null)
            return;

        m_LevelObject.CharacterEnterEvent -= OnCharacterEnterEvent;
        m_LevelObject.CharacterLeaveEvent -= OnCharacterLeaveEvent;
    }

    private void OnCharacterEnterEvent(Character character)
    {
        //Make sure the collider is only enabled when the player is standing on top of it.
        if (character != m_Player)
            return;

        m_Collider.enabled = true;
    }

    private void OnCharacterLeaveEvent(Character character)
    {
        //Make sure the collider is disabled when the player is not standing on top of it.
        if (character != m_Player)
            return;

        m_Collider.enabled = false;
    }

    private void OnMouseDown()
    {
        if (m_Player != null)
            m_Player.Use(m_Direction);
    }
}
