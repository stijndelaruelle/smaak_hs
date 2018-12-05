using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cyber Security/Game Structure/Chapter")]
public class ChapterDataDefinition : ScriptableObject
{
    [SerializeField]
    [LocalizationID]
    private string m_ChapterNameLocalizationID;
    public string ChapterNameLocalizationID
    {
        get { return m_ChapterNameLocalizationID; }
    }

    [SerializeField]
    [LocalizationID]
    private string m_DescriptionLocalizationID;
    public string DescriptionLocalizationID
    {
        get { return m_DescriptionLocalizationID; }
    }

    [SerializeField]
    [Tooltip("Every scene has their own loading scene, allows for more variety")]
    private string m_LoadingScreenSceneName;
    public string LoadingScreenSceneName
    {
        get { return m_LoadingScreenSceneName; }
    }

    [SerializeField]
    [Tooltip("Fallback - The image displayed in the level loader & loading screen")]
    private Sprite m_Background;
    public Sprite Background
    {
        get { return m_Background; }
    }

    [SerializeField]
    [Tooltip("The audio that starts playing once we show this chapter in main & loading screen")]
    private AudioClip m_Music;
    public AudioClip Music
    {
        get { return m_Music; }
    }

    [SerializeField]
    private List<LevelDataDefinition> m_Levels;
    public List<LevelDataDefinition> Levels
    {
        get { return m_Levels; }
    }

    public LevelDataDefinition GetLevel(int id)
    {
        if (id < 0 || id >= m_Levels.Count)
            return null;

        return m_Levels[id];
    }

    public int GetLevelID(LevelDataDefinition levelData)
    {
        return m_Levels.IndexOf(levelData);
    }

    public bool HasLevel(LevelDataDefinition levelData)
    {
        return m_Levels.Contains(levelData);
    }

    public int GetNumberOfLevels()
    {
        return m_Levels.Count;
    }

    public int GetNumberOfLevelsCompleted()
    {
        int numCompleted = 0;

        foreach (LevelDataDefinition level in m_Levels)
        {
            if (level != null && level.HasLevelBeenCompleted())
                numCompleted += 1;
        }

        return numCompleted;
    }

    public bool HasChapterBeenCompleted()
    {
        foreach (LevelDataDefinition level in m_Levels)
        {
            if (level == null || level.HasLevelBeenCompleted() == false)
                return false;
        }

        return true;
    }

    public void CompleteChapter()
    {
        //Debug
        foreach (LevelDataDefinition level in m_Levels)
        {
            if (level != null)
                level.CompleteLevel();
        }
    }
}