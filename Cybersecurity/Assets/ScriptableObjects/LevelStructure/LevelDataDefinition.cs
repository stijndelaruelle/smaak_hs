using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cyber Security/Game Structure/Level")]
public class LevelDataDefinition : ScriptableObject
{
    [SerializeField]
    [LocalizationID]
    private string m_LevelNameLocalizationID;
    public string LevelNameLocalizationID
    {
        get { return m_LevelNameLocalizationID; }
    }

    [SerializeField]
    private string m_SceneName;

    [SerializeField]
    private string m_SceneNameEasy;

    [SerializeField]
    private string m_SaveGameVariableName;

    //Not yet used, will be in the future?
    [SerializeField]
    [LocalizationID]
    private string m_LoadingScreenInfoLocalizationID;
    public string LoadingScreenInfoLocalizationID
    {
        get { return m_LoadingScreenInfoLocalizationID; }
    }

    [Header("Video")]
    [SerializeField]
    private VideoDataDefinition m_UnlockedVideo;
    public VideoDataDefinition UnlockedVideo
    {
        get { return m_UnlockedVideo; }
    }

    //Save game
    public string GetSceneName()
    {
        string levelName = m_SceneNameEasy;

        if (levelName == null || levelName == "" || SaveGameManager.GetBool(SaveGameManager.SAVE_HARDMODE, false) == true)
        {
            levelName = m_SceneName;
        }

        return levelName;
    }

    public bool HasLevelBeenCompleted()
    {
        bool success = SaveGameManager.GetBool(m_SaveGameVariableName, false);

        if (success == false)
            success = SaveGameManager.GetBool(m_SaveGameVariableName + "_Easy", false);

        return success;
    }

    public void CompleteLevel()
    {
        if (m_SceneNameEasy == null || m_SceneNameEasy == "" || SaveGameManager.GetBool(SaveGameManager.SAVE_HARDMODE, false) == true)
        {
            SaveGameManager.SetBool(m_SaveGameVariableName, true);
        }
        else
        {
            SaveGameManager.SetBool(m_SaveGameVariableName + "_Easy", true);
        }
    }
}