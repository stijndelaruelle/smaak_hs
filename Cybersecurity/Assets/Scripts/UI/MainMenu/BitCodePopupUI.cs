using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BitCodePopupUI : MonoBehaviour
{
    [Header("Requirements")]
    [SerializeField]
    private ChapterDataDefinition m_ShowAtChapter;

    [SerializeField]
    private LevelDataDefinition m_LastCompletedLevel;

    [SerializeField]
    private LevelDataDefinition m_FirstIncompleteLevel;

    [Space(5)]
    [Header("Required References")]
    [SerializeField]
    private CanvasGroup m_CanvasGroup;

    [SerializeField]
    private PlayableDirector m_ShowDirector;

    [SerializeField]
    private ChapterCarouselUI m_ChapterCarousel;
    private bool m_HasClosed = false;

    private void Start()
    {
        m_CanvasGroup.Show(false);

        if (m_ChapterCarousel == null)
            return;

        m_ChapterCarousel.ChapterChangedEvent += OnChapterChanged;
    }

    private void OnDestroy()
    {
        if (m_ChapterCarousel == null)
            return;

        m_ChapterCarousel.ChapterChangedEvent -= OnChapterChanged;
    }

    private void OnChapterChanged(ChapterDataDefinition chapterData)
    {
        if (m_HasClosed)
            return;

        //Check if we need to show ourselves
        m_CanvasGroup.Show(false);

        if (chapterData != m_ShowAtChapter)
            return;

        if (m_LastCompletedLevel.HasLevelBeenCompleted() == false)
            return;

        if (m_FirstIncompleteLevel.HasLevelBeenCompleted() == true)
            return;

        m_CanvasGroup.Show(true);

        m_ShowDirector.Play();
    }

    public void Close()
    {
        m_CanvasGroup.Show(false);
        m_HasClosed = true;
    }
}
