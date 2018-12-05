using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChapterCarouselUI : CarouselUI
{
    public delegate void ChapterDataDefinitionDelegate(ChapterDataDefinition data);

    private CampaignDataDefinition m_Campaign;
    private ChapterDataDefinition m_CurrentCapterData;

    public event ChapterDataDefinitionDelegate ChapterChangedEvent;

    protected override void Start()
    {
        base.Start();

        if (LevelManager.Instance != null)
            SetCampaign(LevelManager.Instance.CurrentCampaign);
    }

    public void SetCampaign(CampaignDataDefinition campaign)
    {
        if (campaign == null)
            return;

        m_Campaign = campaign;

        m_CurrentPageID = SaveGameManager.GetInt(SaveGameManager.SAVE_LAST_CHAPTER, 0);
        m_MaxPageID = m_Campaign.GetNumberOfChaptersCompleted() + 2; //+1 is the current chapter, +1 is the chapter after that (we can see the border, but not quite reach it)
        if (m_MaxPageID > m_Campaign.GetNumberOfChapters())
            m_MaxPageID = m_Campaign.GetNumberOfChapters();

        if (m_MaxPageID <= 0)
        {
            Debug.LogWarning("The chapter carousel doesn't contain any chapters!");
            return;
        }

        //Load all our pages
        for (int i = 0; i < m_CarouselPages.Count; ++i)
        {
            //Make sure we loop around
            int chapterID = (m_CurrentPageID + i) % m_MaxPageID;
            m_CarouselPages[i].Initialize(m_Campaign.GetChapter(chapterID));
        }

        SetChapterData();
        HandleProgressionButtons();
    }

    private void Update()
    {
        //Debug
        if (SaveGameManager.GetBool(SaveGameManager.SAVE_CHEATS) == true)
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.U))
            {
                m_Campaign.GetChapter(m_CurrentPageID).CompleteChapter();
            }
        }
    }

    public override void NextPage()
    {
        base.NextPage();

        SetChapterData();
        SaveGameManager.SetInt(SaveGameManager.SAVE_LAST_CHAPTER, m_CurrentPageID);
    }

    protected override void NextPageMoveComplete()
    {
        base.NextPageMoveComplete();

        //Initialize the moved page
        int chapterIDToLoad = m_CurrentPageID + (m_CarouselPages.Count - 1);
        chapterIDToLoad %= m_MaxPageID;

        m_CarouselPages[m_CarouselPages.Count - 1].Initialize(m_Campaign.GetChapter(chapterIDToLoad));
    }

    public override void PreviousPage()
    {
        base.PreviousPage();

        //Update the moved page
        m_CarouselPages[0].Initialize(m_Campaign.GetChapter(m_CurrentPageID));
        SetChapterData();

        SaveGameManager.SetInt(SaveGameManager.SAVE_LAST_CHAPTER, m_CurrentPageID);
    }

    private void SetChapterData()
    {
        m_CurrentCapterData = m_Campaign.GetChapter(m_CurrentPageID);
        
        if (LevelManager.Instance != null)
            LevelManager.Instance.CurrentChapter = m_CurrentCapterData;

        if (ChapterChangedEvent != null)
            ChapterChangedEvent(m_CurrentCapterData);

        //Request from Lex, don't play the theme music in the level select menu.
        //if (AudioPlayer.Instance != null)
        //    AudioPlayer.Instance.PlayMusic(chapterData.Music);
    }

    protected override void HandleProgressionButtons()
    {
        bool canSeeAllChapters = (m_Campaign.GetNumberOfChaptersCompleted() + 1 >= m_Campaign.GetNumberOfChapters());
        bool finishedThisChapter = m_Campaign.GetChapter(m_CurrentPageID).HasChapterBeenCompleted();

        bool showPreviousButton = canSeeAllChapters || (m_CurrentPageID > 0);
        bool showNextButton = canSeeAllChapters || finishedThisChapter; //Only ALWAYS enable the next button when we can see everything anyway. (it will loop around)

        m_PreviousButton.SetActive(showPreviousButton);
        m_NextButton.SetActive(showNextButton);
    }
}