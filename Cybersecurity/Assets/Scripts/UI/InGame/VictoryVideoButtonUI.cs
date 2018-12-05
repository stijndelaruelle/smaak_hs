using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryVideoButtonUI : MonoBehaviour
{
    [SerializeField]
    private VictoryUI m_VictoryUI;

    [SerializeField]
    private bool m_FinishLevelAfterViewing = true;

    private VideoDataDefinition m_VideoData;

    private void Start()
    {
        LevelDataDefinition levelData = null;

        //Try our own level director first
        if (LevelDirector.Instance != null)
            levelData = LevelDirector.Instance.GetLevelData();

        //If that doesn't work, fall back on what the levelmanager thinks
        if (levelData == null && LevelManager.Instance != null)
            levelData = LevelManager.Instance.CurrentLevel;

        if (levelData != null)
            m_VideoData = levelData.UnlockedVideo;

        //Enable or disable ourselves based on wether there is a video linked or not
        gameObject.SetActive(m_VideoData != null);

        if (VideoPlayerUI.Instance == null)
            return;

        VideoPlayerUI.Instance.VideoHideEvent += OnVideoHide;
    }

    private void OnDestroy()
    {
        if (VideoPlayerUI.Instance == null)
            return;

        VideoPlayerUI.Instance.VideoHideEvent -= OnVideoHide;
    }

    public void Click()
    {
        if (m_VideoData == null)
            return;

        if (VideoPlayerUI.Instance == null)
            return;

        if (m_VictoryUI != null && m_VictoryUI.RequestInProgress)
            return;

        VideoPlayerUI.Instance.ShowVideo(m_VideoData);

        //Analytics
        AnalyticsManager.StartVideoEvent(UtilityMethods.RemoveExtention(m_VideoData.VideoPath), false, true);
    }

    //Callback
    private void OnVideoHide()
    {
        if (m_FinishLevelAfterViewing && m_VictoryUI != null)
            m_VictoryUI.RequestNextLevel();
    }
}
