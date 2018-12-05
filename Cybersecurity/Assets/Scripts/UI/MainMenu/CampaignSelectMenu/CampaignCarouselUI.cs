using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignCarouselUI : CarouselUI
{
    public delegate void CampaignDataDelegate(CampaignDataDefinition campaignData);

    [SerializeField]
    private List<CampaignDataDefinition> m_Campaigns;

    public event CampaignDataDelegate VisibleCampaignChangedEvent;

    protected override void Start()
    {
        base.Start();

        m_CurrentPageID = SaveGameManager.GetInt(SaveGameManager.SAVE_LAST_CAMPAIGN, 0);
        m_MaxPageID = m_Campaigns.Count;

        if  (m_MaxPageID <= 0)
        {
            Debug.LogWarning("The campaign carousel doesn't contain any campaigns!");
            return;
        }

        //Load all our pages
        for (int i = 0; i < m_CarouselPages.Count; ++i)
        {
            //Make sure we loop around
            int chapterID = (m_CurrentPageID + i) % m_MaxPageID;
            m_CarouselPages[i].Initialize(m_Campaigns[chapterID]);
        }

        SetCampaignData();
    }

    public void OnShow()
    {
        SetCampaignData();
    }

    public override void NextPage()
    {
        base.NextPage();

        SetCampaignData();
        SaveGameManager.SetInt(SaveGameManager.SAVE_LAST_CAMPAIGN, m_CurrentPageID);
        SaveGameManager.SetInt(SaveGameManager.SAVE_LAST_CHAPTER, 0); //If we close the game when selecting a new campaign but still have the chapter ID of another campaign.
    }

    protected override void NextPageMoveComplete()
    {
        base.NextPageMoveComplete();

        //Initialize the moved page
        int chapterIDToLoad = m_CurrentPageID + (m_CarouselPages.Count - 1);
        chapterIDToLoad %= m_MaxPageID;

        m_CarouselPages[m_CarouselPages.Count - 1].Initialize(m_Campaigns[chapterIDToLoad]);
    }

    public override void PreviousPage()
    {
        base.PreviousPage();

        //Update the moved page
        m_CarouselPages[0].Initialize(m_Campaigns[m_CurrentPageID]);

        SetCampaignData();
        SaveGameManager.SetInt(SaveGameManager.SAVE_LAST_CAMPAIGN, m_CurrentPageID);
        SaveGameManager.SetInt(SaveGameManager.SAVE_LAST_CHAPTER, 0); //If we close the game when selecting a new campaign but still have the chapter ID of another campaign.
    }

    public CampaignDataDefinition GetCurrentCampaign()
    {
        return m_Campaigns[m_CurrentPageID];
    }

    private void SetCampaignData()
    {
        CampaignDataDefinition campaignData = m_Campaigns[m_CurrentPageID];

        if (LevelManager.Instance != null)
            LevelManager.Instance.CurrentCampaign = campaignData;

        if (AudioPlayer.Instance != null)
            AudioPlayer.Instance.PlayMusic(campaignData.Music);

        if (VisibleCampaignChangedEvent != null)
            VisibleCampaignChangedEvent(m_Campaigns[m_CurrentPageID]);
    }
}
