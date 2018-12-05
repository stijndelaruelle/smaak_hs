using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class FadeSpriteTrigger : LevelObject
{
    //Couldn't think of another name right now
    public enum FadeSpriteTriggerBehaviour
    {
        FadeIn,
        FadeOut,
        ToggleFade
    }

    [Space(5)]
    [Header("Specific - Activate GameObject Trigger")]

    [SerializeField]
    private FactionTypeDefinition m_AllowedFaction;

    [SerializeField]
    private SpriteRenderer m_SpriteRenderer;
    private float m_OriginalAlpha = 0.0f;
    private Tweener m_CurrentTween;

    [SerializeField]
    private FadeSpriteTriggerBehaviour m_Behaviour;

    [SerializeField]
    private float m_Duration;

    [SerializeField]
    protected bool m_TriggerOnEnter = false;

    [SerializeField]
    protected bool m_TriggerOnLeave = false;

    [SerializeField]
    private bool m_DisableAfterTrigger = false;

    protected override void Start()
    {
        base.Start();

        if (m_SpriteRenderer != null)
            m_OriginalAlpha = m_SpriteRenderer.color.a;
    }

    //LevelObject
    public override void OnCharacterDirectionEnter(Character character)
    {
        if (m_TriggerOnEnter == false)
            return;

        if (character.Faction != m_AllowedFaction)
            return;

        FadeGameObject();
    }

    public override void OnCharacterDirectionLeave(Character character)
    {
        if (m_TriggerOnLeave == false)
            return;

        if (character.Faction != m_AllowedFaction)
            return;

        FadeGameObject();
    }

    private void FadeGameObject()
    {
        //Check our own state
        if (gameObject.activeSelf == false)
            return;

        if (m_IsEnabled == false)
            return;

        //Check our parameter state
        if (m_SpriteRenderer == null)
            return;

        //Actually do what we're supposed to do
        switch (m_Behaviour)
        {
            case FadeSpriteTriggerBehaviour.FadeIn:
            {
                if (m_CurrentTween != null)
                    m_CurrentTween.Kill();

                m_CurrentTween = m_SpriteRenderer.DOFade(1.0f, m_Duration).OnComplete(OnFadeComplete);
                break;
            }

            case FadeSpriteTriggerBehaviour.FadeOut:
            {
                if (m_CurrentTween != null)
                    m_CurrentTween.Kill();

                m_CurrentTween = m_SpriteRenderer.DOFade(0.0f, m_Duration).OnComplete(OnFadeComplete);
                    break;
            }

            case FadeSpriteTriggerBehaviour.ToggleFade:
            {
                if (m_CurrentTween != null)
                    m_CurrentTween.Kill();

                float alpha = 0.0f;
                if (m_SpriteRenderer.color.a < 0.5f) { alpha = 1.0f; }

                m_CurrentTween = m_SpriteRenderer.DOFade(alpha, m_Duration).OnComplete(OnFadeComplete);

                break;
            }

            default:
                break;
        }

        //Disable ourselves if needed
        if (m_DisableAfterTrigger)
            SetEnabled(false);
    }

    private void OnFadeComplete()
    {
        m_CurrentTween = null;
    }

    //ResetableObject
    protected override void OnReset()
    {
        base.OnReset();

        if (m_SpriteRenderer != null)
            m_CurrentTween = m_SpriteRenderer.DOFade(m_OriginalAlpha, m_Duration).OnComplete(OnFadeComplete);
    }
}