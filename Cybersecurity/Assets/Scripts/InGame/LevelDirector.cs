using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LevelDirector : Singleton<LevelDirector>
{
    //Central communication piece within a level (mainly spits out & listens to events)
    public delegate void LevelManagerDelegate();

    [SerializeField]
    private LevelDataDefinition m_LevelData;
    public LevelDataDefinition LevelData
    {
        get
        {
            if (m_LevelData != null)
            {
                return m_LevelData;
            }
            else
            {
                if (LevelManager.Instance != null)
                {
                    return LevelManager.Instance.CurrentLevel;
                }
            }

            return null;
        }
    }

    //Not quite sure if this is the right place for this variable
    private int m_GeneralInputBlockers = 0;
    private int m_PlayerInputBlockers = 0; //Amount of items blocking player input, player has input when this variable hits 0

    private bool m_HasLevelStarted = false;
    private bool m_HasLevelEnded = false;
    private bool m_LevelUpdateRequested = false; //Multiple objects can request a Level Update at the same time, let's execute it ONCE the followig frame to avoid sequencing issues

    //Analytics Data
    private int m_NumberOfTries = 0;
    
    private Stopwatch m_CurrentAnalyticsStopwatch; //Current time spent on this run
    private Stopwatch m_TotalAnalyticsStopwatch; //Total time spent on this level

    private Coroutine m_IgnoreSFXRoutine;

    public event LevelManagerDelegate LevelStartEvent;
    public event LevelManagerDelegate LevelStopEvent;
    public event LevelManagerDelegate LevelUpdateEvent; //The level changed
    public event LevelManagerDelegate LevelPlayerUpdateEvent; //The player moved on purpose
    public event LevelManagerDelegate LevelResetEvent;
    public event LevelManagerDelegate LevelEndVictoryEvent;
    public event LevelManagerDelegate LevelEndDefeatEvent;

    public void Update()
    {
        //First frame start the game (all objects have had their "Start" called)
        if (m_HasLevelStarted == false)
        {
            if (LevelStartEvent != null)
                LevelStartEvent();

            m_HasLevelStarted = true;

            //Analytics stopwatches
            m_CurrentAnalyticsStopwatch = new Stopwatch();
            m_CurrentAnalyticsStopwatch.Start();

            m_TotalAnalyticsStopwatch = new Stopwatch();
            m_TotalAnalyticsStopwatch.Start();

            //Analytics event
            CallLevelStartAnalyticsEvent();
        }

        if (m_LevelUpdateRequested)
        {
            UpdateLevel();
            m_LevelUpdateRequested = false;
        }

        //Debug
        if (SaveGameManager.GetBool(SaveGameManager.SAVE_CHEATS) == true)
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.U))
            {
                UpdateLevel();
                PlayerMoved();
            }
        }
    }

    //General input
    public void AddGeneralInputBlocker(string debugReason = "")
    {
        m_GeneralInputBlockers += 1;
        UnityEngine.Debug.Log("AddGeneralInputBlocker (" + m_GeneralInputBlockers + ") - " + debugReason);
    }

    public void RemoveGeneralInputBlocker(string debugReason = "")
    {
        m_GeneralInputBlockers -= 1;

        if (m_GeneralInputBlockers < 0)
            m_GeneralInputBlockers = 0;

        UnityEngine.Debug.Log("RemoveGeneralInputBlocker (" + m_GeneralInputBlockers + ") - " + debugReason);
    }

    public bool HasGeneralInput()
    {
        return (m_GeneralInputBlockers == 0);
    }


    //Player input
    public void AddInputBlocker(string debugReason = "")
    {
        m_PlayerInputBlockers += 1;
        //UnityEngine.Debug.Log("AddInputBlocker (" + m_PlayerInputBlockers + ") - " + debugReason);
    }

    public void RemoveInputBlocker(string debugReason = "")
    {
        m_PlayerInputBlockers -= 1;

        if (m_PlayerInputBlockers < 0)
            m_PlayerInputBlockers = 0;

        //UnityEngine.Debug.Log("RemoveInputBlocker (" + m_PlayerInputBlockers + ") - " + debugReason);
    }

    public bool HasPlayerInput()
    {
        return (HasGeneralInput() && m_PlayerInputBlockers == 0);
    }

    public bool HasGameEnded()
    {
        return m_HasLevelEnded;
    }

    //Called by the Exit upon a player exiting
    public void PlayerVictory()
    {
        AddInputBlocker("LevelDirector: Player Victory");

        m_HasLevelEnded = true;

        if (LevelEndVictoryEvent != null)
            LevelEndVictoryEvent();

        //Analytics
        CallLevelCompleteAnalyticsEvent();

        //Save game
        if (LevelManager.Instance != null)
            LevelManager.Instance.CompleteCurrentLevel();

        m_NumberOfTries = 0;

        m_CurrentAnalyticsStopwatch.Stop();

        m_TotalAnalyticsStopwatch.Stop();
        m_TotalAnalyticsStopwatch.Reset();
    }

    //Called by the player when he didn't make it
    public void PlayerDied()
    {
        AddInputBlocker("LevelDirector: Player Died");

        m_HasLevelEnded = true;

        if (LevelEndDefeatEvent != null)
            LevelEndDefeatEvent();
    }

    //Called by the Level has updated (tiles have moved, gates have opened, etc...)
    public void RequestLevelUpdate()
    {
        m_LevelUpdateRequested = true;
    }

    private void UpdateLevel()
    {
        //Check trick to not let the game upate when we just won/lost
        if (m_HasLevelEnded)
            return;

        if (LevelUpdateEvent != null)
            LevelUpdateEvent(); 
    }

    //Called when the player moved by input (not by teleport or moving himself via a moveable tile)
    public void PlayerMoved()
    {
        if (m_HasLevelEnded)
            return;

        if (LevelPlayerUpdateEvent != null)
            LevelPlayerUpdateEvent();
    }

    //Called by the Game Over UI when pressing any button
    public void ResetLevel()
    {
        m_GeneralInputBlockers = 0;
        m_PlayerInputBlockers = 0;
        //Don't reset m_HasLevelStarted
        m_HasLevelEnded = false;

        //Ignore all SFX calls for a frame
        if (m_IgnoreSFXRoutine != null)
            StopCoroutine(m_IgnoreSFXRoutine);

        m_IgnoreSFXRoutine = StartCoroutine(IgnoreSFXRoutine());

        if (LevelResetEvent != null)
            LevelResetEvent();

        //Increase the amount of times we tried
        m_NumberOfTries += 1;
        m_CurrentAnalyticsStopwatch.Stop();
        m_CurrentAnalyticsStopwatch.Reset();
        m_CurrentAnalyticsStopwatch.Start();

        if (m_TotalAnalyticsStopwatch.IsRunning == false)
        {
            m_TotalAnalyticsStopwatch.Start();
        }
    }

    public LevelDataDefinition GetLevelData()
    {
        return m_LevelData;
    }

    private IEnumerator IgnoreSFXRoutine()
    {
        //Ignore all SFX calls for a frame (otherwise a lot of doors will blast your ears at once)

        if (AudioPlayer.Instance != null)
            AudioPlayer.Instance.IngoreSFX(true);

        yield return new WaitForEndOfFrame();

        if (AudioPlayer.Instance != null)
            AudioPlayer.Instance.IngoreSFX(false);

        m_IgnoreSFXRoutine = null;
        yield return null;
    }




    //Analytics calls (Not happy with this solution, but I want all my level based analytics in 1 place.)
    private void CallLevelStartAnalyticsEvent()
    {
        if (m_LevelData == null)
            return;

        bool firstTime = (m_LevelData.HasLevelBeenCompleted() == false);
        AnalyticsManager.LevelStartEvent(m_LevelData.GetSceneName(), firstTime);
    }

    private void CallLevelCompleteAnalyticsEvent()
    {
        if (m_LevelData == null)
            return;

        bool firstTime = (m_LevelData.HasLevelBeenCompleted() == false);
        AnalyticsManager.LevelCompleteEvent(m_LevelData.GetSceneName(), firstTime, m_NumberOfTries, m_CurrentAnalyticsStopwatch.Elapsed.TotalSeconds, m_TotalAnalyticsStopwatch.Elapsed.TotalSeconds);
    }

    public void CallLevelFailAnalyticsEvent(bool deathByEnemy, bool deathByQuiz, bool resetByUI, bool resetByEndUI)
    {
        if (m_LevelData == null)
            return;

        AnalyticsManager.LevelFailEvent(m_LevelData.GetSceneName(), m_NumberOfTries, m_CurrentAnalyticsStopwatch.Elapsed.TotalSeconds, deathByEnemy, deathByQuiz, resetByUI, resetByEndUI);
    }

    public void CallLevelQuitAnalyticsEvent(bool quitByVictory, bool quitByUI)
    {
        if (m_LevelData == null)
            return;

        AnalyticsManager.LevelQuitEvent(m_LevelData.GetSceneName(), m_NumberOfTries, m_CurrentAnalyticsStopwatch.Elapsed.TotalSeconds, quitByVictory, quitByUI);
    }

    public void CallHintsAnalyticsEvent()
    {
        if (m_LevelData == null)
            return;

        AnalyticsManager.HintEnabledEvent(m_LevelData.GetSceneName(), m_NumberOfTries, m_CurrentAnalyticsStopwatch.Elapsed.TotalSeconds, m_TotalAnalyticsStopwatch.Elapsed.TotalSeconds);
    }
}
