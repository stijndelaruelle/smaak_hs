using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenBackgroundUI : MonoBehaviour
{
    [SerializeField]
    private Image m_Image;

    private void Start()
    {
        LevelManager levelManager = LevelManager.Instance;
        if (levelManager == null)
        {
            return;
        }

        ChapterDataDefinition chapterData = levelManager.CurrentChapter;

        if (chapterData == null)
            return;

        //Loading screen will come from the campaign
        Sprite sprite = chapterData.Background;

        if (sprite != null)
            m_Image.sprite = sprite;
    }
}
