using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkHacker : Hacker
{
    [SerializeField]
    private bool m_LaughAtStart = false;

    protected override void Start()
    {
        base.Start();

        if (LevelDirector.Instance == null)
            return;

        LevelDirector.Instance.LevelStartEvent += OnLevelStart;
        LevelDirector.Instance.LevelEndDefeatEvent += OnPlayerDefeat;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (LevelDirector.Instance == null)
            return;

        LevelDirector.Instance.LevelStartEvent -= OnLevelStart;
        LevelDirector.Instance.LevelEndDefeatEvent -= OnPlayerDefeat;
    }

    private void OnLevelStart()
    {
        if (m_LaughAtStart == false)
            return;

        if (m_Animator != null)
            m_Animator.SetTrigger("Laugh");
    }

    private void OnPlayerDefeat()
    {
        if (m_Animator != null)
            m_Animator.SetTrigger("Laugh");
    }
}
