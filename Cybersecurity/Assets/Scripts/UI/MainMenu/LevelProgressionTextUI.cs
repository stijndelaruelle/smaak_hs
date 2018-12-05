using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressionTextUI : MonoBehaviour
{
    [SerializeField]
    private CampaignCarouselPageUI m_CampaignCarouselPage;

    [SerializeField]
    private Text m_Text;

    [SerializeField]
    [LocalizationID]
    private string m_LocalizationID;

    private void Start()
    {
        SaveGameManager.VariableChangedEvent += OnSaveGameVariableChanged;
        SaveGameManager.SaveGameDeletedEvent += OnSaveGameDeletedEvent;

        if (m_CampaignCarouselPage != null)
        {
            m_CampaignCarouselPage.InitializedEvent += OnCarouselPageInitialized;
        }

        //Because the event may fire before we can subscribe
        UpdateLabel();
    }

    private void OnDestroy()
    {
        SaveGameManager.VariableChangedEvent -= OnSaveGameVariableChanged;
        SaveGameManager.SaveGameDeletedEvent -= OnSaveGameDeletedEvent;

        if (m_CampaignCarouselPage != null)
            m_CampaignCarouselPage.InitializedEvent -= OnCarouselPageInitialized;
    }

    private void UpdateLabel()
    {
        CampaignDataDefinition campaignData = m_CampaignCarouselPage.Campaign;

        if (campaignData == null)
            return;

        int numberOfLevels = campaignData.GetTotalNumberOfLevels();
        int numberOfLevelsCompleted = campaignData.GetTotalNumberOfLevelsCompleted();

        m_Text.text = LocalizationManager.GetText(m_LocalizationID, numberOfLevelsCompleted, numberOfLevels);

        //Only show this label when we've completed at least 1 level. It looks quite sad otherwise.
        m_Text.enabled = (numberOfLevelsCompleted > 0);
    }

    private void OnCarouselPageInitialized()
    {
        UpdateLabel();
    }

    private void OnSaveGameVariableChanged(string key, object value)
    {
        UpdateLabel();
    }

    private void OnSaveGameDeletedEvent()
    {
        UpdateLabel();
    }
}
