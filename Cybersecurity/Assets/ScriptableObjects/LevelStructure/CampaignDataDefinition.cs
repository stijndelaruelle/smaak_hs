using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cyber Security/Game Structure/Campaign")]
public class CampaignDataDefinition : ScriptableObject
{
    [SerializeField]
    [LocalizationID]
    private string m_CampaignNameLocalizationID;
    public string CampaignNameLocalizationID
    {
        get { return m_CampaignNameLocalizationID; }
    }

    [SerializeField]
    [LocalizationID]
    private string m_DescriptionLocalizationID;
    public string DescriptionLocalizationID
    {
        get { return m_DescriptionLocalizationID; }
    }

    [SerializeField]
    [Tooltip("The image displayed in the main menu")]
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
    private List<ChapterDataDefinition> m_Chapters;
    public List<ChapterDataDefinition> Chapters
    {
        get { return m_Chapters; }
    }

    public ChapterDataDefinition GetChapter(int id)
    {
        if (id < 0 || id >= m_Chapters.Count)
            return null;

        return m_Chapters[id];
    }

    public bool HasChapter(ChapterDataDefinition chapterData)
    {
        return m_Chapters.Contains(chapterData);
    }


    public int GetNumberOfChapters()
    {
        return m_Chapters.Count;
    }

    public int GetNumberOfChaptersCompleted()
    {
        int numCompleted = 0;

        foreach(ChapterDataDefinition chapter in m_Chapters)
        {
            if (chapter.HasChapterBeenCompleted())
                numCompleted += 1;
        }

        return numCompleted;
    }

    public int GetTotalNumberOfLevels()
    {
        int totalLevels = 0;

        foreach (ChapterDataDefinition chapter in m_Chapters)
        {
            if (chapter != null)
                totalLevels += chapter.GetNumberOfLevels();
        }

        return totalLevels;
    }

    public int GetTotalNumberOfLevelsCompleted()
    {
        int totalLevelsCompleted = 0;

        foreach (ChapterDataDefinition chapter in m_Chapters)
        {
            if (chapter != null)
                totalLevelsCompleted += chapter.GetNumberOfLevelsCompleted();
        }

        return totalLevelsCompleted;
    }
}