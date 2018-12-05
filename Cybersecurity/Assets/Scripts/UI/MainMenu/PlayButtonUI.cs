using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayButtonUI : LocalizedObject
{
    [SerializeField]
    [Tooltip("Localization ID")]
    private string m_PlayText;

    //[SerializeField]
    //private string m_ContinueText;

    [Header("Required references")]
    [SerializeField]
    private Text m_Label;

    [SerializeField]
    private CampaignCarouselUI m_CampaignCarousel;

    [SerializeField]
    private ChapterCarouselUI m_ChapterCarousel;

    [SerializeField]
    private MainMenuPannerUI m_Panner;

    private Button m_Button;

    private void Awake()
    {
        m_Button = GetComponent<Button>();
    }

    protected override void Start()
    {
        base.Start();

        if (m_CampaignCarousel != null)
        {
            m_CampaignCarousel.VisibleCampaignChangedEvent += OnVisibleCampaignChanged;

            //Event may fire before the carousel has time to subscribe
            OnVisibleCampaignChanged(m_CampaignCarousel.GetCurrentCampaign());
        }

        if (m_Label == null)
        {
            Debug.LogWarning("Play button doesn't have a label assigned!");
            return;
        }

        UpdateButton();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (m_CampaignCarousel != null)
            m_CampaignCarousel.VisibleCampaignChangedEvent -= OnVisibleCampaignChanged;
    }

    private void UpdateButton()
    {
        string labeltext = m_PlayText;

        /*
        //Check if we already have a save game.
        LevelManager levelManager = LevelManager.Instance;
        if (levelManager != null)
        {
            LevelDataDefinition levelData = levelManager.CurrentLevel;

            if (levelData != null)
                labeltext = m_ContinueText;
        }
        */

        //Actually set the text
        m_Label.text = LocalizationManager.GetText(labeltext);
    }

    public void Click()
    {
        if (m_CampaignCarousel.GetCurrentCampaign().GetNumberOfChapters() <= 0)
            return;

        m_ChapterCarousel.SetCampaign(m_CampaignCarousel.GetCurrentCampaign());
        m_Panner.PanDown();
    }

    private void OnVisibleCampaignChanged(CampaignDataDefinition campaignData)
    {
        m_Button.interactable = (campaignData.GetNumberOfChapters() > 0);
    }

    protected override void OnLanguageChanged(LocalizationManager.Language language)
    {
        UpdateButton();
    }
}
