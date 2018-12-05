using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : LevelObject
{
    [Header("Specific - Gate")]
    [SerializeField]
    private bool m_InverseAfterPassTrough = false;

    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private AudioClip m_OpenSoundEffect;

    [SerializeField]
    private AudioClip m_CloseSoundEffect;

    protected override void Awake()
    {
        base.Awake();
    }

    public override bool BlocksCharacters()
    {
        return (m_IsEnabled == false);
    }

    public override void OnCharacterDirectionEnter(Character character)
    {
        if (m_InverseAfterPassTrough == false)
            return;

        SetEnabled(!m_IsEnabled);
    }

    public override void SetEnabled(bool state)
    {
        if (gameObject.activeSelf == false)
            return;
        
        if (m_IsEnabled == state)
        {
            //Allow gates to animate move down at the beginning of a level (dirty fix)
            if (m_IsEnabled == false)
                return;
        }

        base.SetEnabled(state);

        if (m_Animator == null)
        {
            Debug.LogWarning(gameObject.name + " doesn't have an animator!");
            return;
        }

        //Reset triggers, otherwise stuff can stick around after resetting the game.
        m_Animator.ResetTrigger("Open");
        m_Animator.ResetTrigger("Close");

        if (m_IsEnabled)
        {
            m_Animator.SetTrigger("Open");

            if (AudioPlayer.Instance != null)
                AudioPlayer.Instance.PlaySFXOneShot(m_OpenSoundEffect);
        }
        else
        {
            m_Animator.SetTrigger("Close");

            if (AudioPlayer.Instance != null)
                AudioPlayer.Instance.PlaySFXOneShot(m_CloseSoundEffect);
        }

        //The level has changed, let everyone know!
        LevelDirector.Instance.RequestLevelUpdate();
    }
}
