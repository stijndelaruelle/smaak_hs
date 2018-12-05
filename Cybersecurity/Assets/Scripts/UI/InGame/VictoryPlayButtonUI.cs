using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryPlayButtonUI : MonoBehaviour
{
    [SerializeField]
    private VictoryUI m_VictoryUI;

    private void Start()
    {
        LevelDataDefinition levelData = null;

        //Try our own level director first
        if (LevelDirector.Instance != null)
            levelData = LevelDirector.Instance.GetLevelData();

        //If that doesn't work, fall back on what the levelmanager thinks
        if (levelData == null && LevelManager.Instance != null)
            levelData = LevelManager.Instance.CurrentLevel;

        VideoDataDefinition videoData = null;

        if (levelData != null)
            videoData = levelData.UnlockedVideo;

        //Only show ourselves when there is no video available
        gameObject.SetActive(videoData == null);
    }

    public void Click()
    {
        if (m_VictoryUI != null)
            m_VictoryUI.RequestNextLevel();
    }
}
