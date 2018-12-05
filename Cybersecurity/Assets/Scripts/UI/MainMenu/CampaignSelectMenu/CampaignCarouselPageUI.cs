using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignCarouselPageUI : CarouselPageUI
{
    public delegate void CampaignCarouselPageDelegate();
    [SerializeField]
    private Text m_NameText;

    [SerializeField]
    private Text m_DescriptionText;

    [SerializeField]
    private Image m_Background;

    private CampaignDataDefinition m_Campaign;
    public CampaignDataDefinition Campaign
    {
        get { return m_Campaign; }
    }

    public event CampaignCarouselPageDelegate InitializedEvent;

    public override void Initialize(ScriptableObject data)
    {
        //Slightly dirty cast, please don't feed this the wrong data!
        m_Campaign = (CampaignDataDefinition)data;

        if (m_Campaign == null)
        {
            m_NameText.text = "Invalid campaign";
            m_DescriptionText.text = "This didn't go according to plan!";
            return;
        }

        m_NameText.text = LocalizationManager.GetText(m_Campaign.CampaignNameLocalizationID);
        m_DescriptionText.text = LocalizationManager.GetText(m_Campaign.DescriptionLocalizationID);
        m_Background.sprite = m_Campaign.Background;

        if (InitializedEvent != null)
            InitializedEvent();
    }
}
