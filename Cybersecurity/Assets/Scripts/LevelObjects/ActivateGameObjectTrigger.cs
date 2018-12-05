using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ActivateGameObjectTrigger : LevelObject
{
    //Couldn't think of another name right now
    public enum ActivateGameObjectTriggeBehaviour
    {
        On,
        Off,
        Toggle
    }

    [Space(5)]
    [Header("Specific - Activate GameObject Trigger")]

    [SerializeField]
    private FactionTypeDefinition m_AllowedFaction;

    [SerializeField]
    private GameObject m_TargetGameObject;
    private bool m_OriginalGameObjectState = false;

    [SerializeField]
    private ActivateGameObjectTriggeBehaviour m_Behaviour;

    [SerializeField]
    protected bool m_TriggerOnEnter = false;

    [SerializeField]
    protected bool m_TriggerOnLeave = false;

    [SerializeField]
    private bool m_DisableAfterTrigger = false;

    protected override void Start()
    {
        base.Start();

        if (m_TargetGameObject != null)
            m_OriginalGameObjectState = m_TargetGameObject.activeSelf;
    }

    //LevelObject
    public override void OnCharacterDirectionEnter(Character character)
    {
        if (m_TriggerOnEnter == false)
            return;

        if (character.Faction != m_AllowedFaction)
            return;

        ActivateGameObject();
    }

    public override void OnCharacterDirectionLeave(Character character)
    {
        if (m_TriggerOnLeave == false)
            return;

        if (character.Faction != m_AllowedFaction)
            return;

        ActivateGameObject();
    }

    private void ActivateGameObject()
    {
        //Check our own state
        if (gameObject.activeSelf == false)
            return;

        if (m_IsEnabled == false)
            return;

        //Check our parameter state
        if (m_TargetGameObject == null)
            return;

        //Actually do what we're supposed to do
        switch (m_Behaviour)
        {
            case ActivateGameObjectTriggeBehaviour.On:
                m_TargetGameObject.SetActive(true);
                break;

            case ActivateGameObjectTriggeBehaviour.Off:
                m_TargetGameObject.SetActive(false);
                break;

            case ActivateGameObjectTriggeBehaviour.Toggle:
                m_TargetGameObject.SetActive(!m_TargetGameObject.activeSelf);
                break;

            default:
                break;
        }

        //Disable ourselves if needed
        if (m_DisableAfterTrigger)
            SetEnabled(false);
    }

    //ResetableObject
    protected override void OnReset()
    {
        base.OnReset();

        if (m_TargetGameObject != null)
            m_TargetGameObject.SetActive(m_OriginalGameObjectState);
    }
}