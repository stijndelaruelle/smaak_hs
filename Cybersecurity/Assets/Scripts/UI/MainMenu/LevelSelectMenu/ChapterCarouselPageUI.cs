using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterCarouselPageUI : CarouselPageUI
{
    [SerializeField]
    private Text m_NameText;

    [SerializeField]
    private Text m_DescriptionText;

    [SerializeField]
    private Image m_Background;

    [SerializeField]
    private RectTransform m_ButtonParent;

    //If this get's even more out of hand I'll make it poolable etc. But for now instancing & destroying shouldn't hurt too much
    [SerializeField]
    private ChapterCarouselLevelButtonUI m_LevelButtonPrefab;

    [SerializeField]
    private ChapterCarouselVideoButtonUI m_VideoButtonPrefab;

    private ChapterDataDefinition m_ChapterData;

    public override void Initialize(ScriptableObject data)
    {
        //Slightly dirty cast, please don't feed this the wrong data!
        m_ChapterData = (ChapterDataDefinition)data;

        if (m_ChapterData == null)
        {
            m_NameText.text = "Invalid chapter";
            m_DescriptionText.text = "This didn't go according to plan!";
            return;
        }

        m_NameText.text = LocalizationManager.GetText(m_ChapterData.ChapterNameLocalizationID);
        m_DescriptionText.text = LocalizationManager.GetText(m_ChapterData.DescriptionLocalizationID);
        m_Background.sprite = m_ChapterData.Background;

        HandleLevelButtons();
    }

    private void HandleLevelButtons()
    {
        if (m_ButtonParent == null)
            return;

        if (m_LevelButtonPrefab == null)
            return;

        //Destroy all objects (hopefully only buttons?) (lame, should pool but there's no time)
        for (int i = 0; i < m_ButtonParent.childCount; ++i)
        {
            GameObject.Destroy(m_ButtonParent.GetChild(i).gameObject);
        }

        //Unlock all levels that were completed before AND the next one
        bool isUnlocked = true;

        //Add all the level buttons
        for (int i = 0; i < m_ChapterData.GetNumberOfLevels(); ++i)
        {
            LevelDataDefinition levelData = m_ChapterData.GetLevel(i);

            //Create a new level button
            ChapterCarouselLevelButtonUI newLevelButton = GameObject.Instantiate<ChapterCarouselLevelButtonUI>(m_LevelButtonPrefab, m_ButtonParent);
            newLevelButton.Initialize(levelData, isUnlocked);

            if (levelData == null || levelData.HasLevelBeenCompleted() == false)
                isUnlocked = false;

            //If the level unlocks a video, add a new video button next to it
            if (levelData != null && levelData.UnlockedVideo != null && m_VideoButtonPrefab != null)
            {
                ChapterCarouselVideoButtonUI newVideoButton = GameObject.Instantiate<ChapterCarouselVideoButtonUI>(m_VideoButtonPrefab, m_ButtonParent);
                newVideoButton.Initialize(levelData.UnlockedVideo, isUnlocked);
            }
        }
    }
}
