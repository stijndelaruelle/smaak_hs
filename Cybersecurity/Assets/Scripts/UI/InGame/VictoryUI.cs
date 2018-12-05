using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class VictoryUI : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Panel;

    [SerializeField]
    private PlayableDirector m_IntroDirector;

    [SerializeField]
    private PlayableDirector m_OutroDirector;

    [SerializeField]
    private AudioClip m_VictorySound;

    private Coroutine m_OutroCoroutine;
    private bool m_RequestInProgress = false;
    public bool RequestInProgress
    {
        get { return m_RequestInProgress; }
    }

    private void Start()
    {
        LevelDirector levelManager = LevelDirector.Instance;
        if (levelManager != null)
        {
            levelManager.LevelEndVictoryEvent += OnVictory;
            levelManager.LevelResetEvent += OnReset;
        }
    }

    private void OnDestroy()
    {
        LevelDirector levelManager = LevelDirector.Instance;

        if (levelManager != null)
        {
            levelManager.LevelEndVictoryEvent -= OnVictory;
            levelManager.LevelResetEvent -= OnReset;
        }
    }

    private void Update()
    {
        if (m_Panel.activeInHierarchy == false)
            return;

        //Click comes from a button
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.Return))
        {
            LoadNextLevel();
        }
    }

    public void RequestNextLevel()
    {
        if (m_RequestInProgress)
            return;

        if (m_OutroDirector != null)
        {
            m_RequestInProgress = true;

            m_OutroDirector.Stop();
            m_OutroDirector.Play();

            //We can't trust PlayableDirector.stopped (triggers when we don't want to to, so we'll time it manually)
            if (m_OutroCoroutine != null)
                StopCoroutine(m_OutroCoroutine);

            m_OutroCoroutine = StartCoroutine(OutroTimerRoutine(1.75f, LoadNextLevel));
        }
        else
        {
            LoadNextLevel();
        }
    }

    public void RequestResetLevel()
    {
        if (m_RequestInProgress)
            return;

        if (m_OutroDirector != null)
        {
            m_RequestInProgress = true;

            m_OutroDirector.Stop();
            m_OutroDirector.Play();

            //We can't trust PlayableDirector.stopped (triggers when we don't want to to, so we'll time it manually)
            if (m_OutroCoroutine != null)
                StopCoroutine(m_OutroCoroutine);

            m_OutroCoroutine = StartCoroutine(OutroTimerRoutine(1.75f, ResetLevel));
        }
        else
        {
            ResetLevel();
        }
    }

    private IEnumerator OutroTimerRoutine(float duration, Action callback)
    {
        //We can't trust PlayableDirector.stopped (triggers when we don't want to to, so we'll time it manually)
        yield return new WaitForSeconds(duration);

        callback();
        m_OutroCoroutine = null;

        yield return null;
    }

    private void LoadNextLevel()
    {
        m_RequestInProgress = false;
        LevelManager levelManager = LevelManager.Instance;

        if (levelManager != null)
        {
            if (LevelDirector.Instance != null)
                LevelDirector.Instance.CallLevelQuitAnalyticsEvent(true, false);

            levelManager.LoadNextLevel();
        }
        else
        {
            ResetLevel();
        }
    }
    
    private void ResetLevel()
    {
        m_RequestInProgress = false;

        if (LevelDirector.Instance != null)
        {   
            LevelDirector.Instance.CallLevelFailAnalyticsEvent(false, false, false, true);
            LevelDirector.Instance.ResetLevel();
        } 
    }

    //Callbacks
    private void OnVictory()
    {
        //Animation
        if (m_IntroDirector != null)
        {
            m_IntroDirector.Stop();
            m_IntroDirector.Play();
        }
        else
        {
            m_Panel.SetActive(true);
        }

        //SFX (We don't play this trough the timeline as we can't assign the SFX mixer to it(?)
        if (AudioPlayer.Instance != null)
            AudioPlayer.Instance.PlaySFXOneShot(m_VictorySound);
    }

    private void OnReset()
    {
        if (m_IntroDirector != null)
            m_IntroDirector.Stop();

        if (m_OutroDirector != null)
            m_OutroDirector.Stop();

        m_Panel.SetActive(false);
    }
}
