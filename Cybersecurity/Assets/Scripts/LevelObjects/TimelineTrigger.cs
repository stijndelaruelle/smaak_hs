using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineTrigger : LevelObject
{
    [Space(5)]
    [Header("Specific - Timeline Trigger")]

    [SerializeField]
    private FactionTypeDefinition m_AllowedFaction;

    [SerializeField]
    private PlayableDirector m_PlayableDirector;

    [SerializeField]
    protected bool m_TriggerOnEnter = false;

    [SerializeField]
    protected bool m_TriggerOnLeave = false;

    [SerializeField]
    private bool m_DisableAfterTrigger = false;

    //LevelObject
    public override void OnCharacterDirectionEnter(Character character)
    {
        if (m_TriggerOnEnter == false)
            return;

        if (character.Faction != m_AllowedFaction)
            return;

        PlayTimeLine();
    }

    public override void OnCharacterDirectionLeave(Character character)
    {
        if (m_TriggerOnLeave == false)
            return;

        if (character.Faction != m_AllowedFaction)
            return;

        PlayTimeLine();
    }

    private void PlayTimeLine()
    {
        if (gameObject.activeSelf == false)
            return;

        if (m_IsEnabled == false)
            return;

        //Start Timeline
        m_PlayableDirector.Stop();
        m_PlayableDirector.Play();

        if (m_DisableAfterTrigger)
            SetEnabled(false);
    }

    //ResetableObject
    protected override void OnReset()
    {
        base.OnReset();

        if (m_PlayableDirector != null)
            m_PlayableDirector.Stop();
    }
}
