using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChapterCarouselVideoButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //Instead of changing the backgound & icon we just swap buttons entirily.
    //This looks "ineficcient" but otherwise we have to manually correct all the icon sizes etc. (they aren't all the same size)
    //This basically causes more clutter in the editor and less in code
    [SerializeField]
    private GameObject m_PlayButton;

    //Maybe later
    //[SerializeField]
    //private GameObject m_CompletedButton;

    [SerializeField]
    private GameObject m_LockedButton;

    private VideoDataDefinition m_VideoData;

    //Used for debug unlocking
    private bool m_IsHovering;

    public void Initialize(VideoDataDefinition video, bool isUnlocked)
    {
        m_VideoData = video;

        //Deactivate all of them (even happens when it's an invalid level)
        m_PlayButton.SetActive(false);
        //m_CompletedButton.SetActive(false);
        m_LockedButton.SetActive(false);

        //Change our appearance depending on if the level is completed/locked/ready to be played
        if (video == null)
            return;

        //if (m_VideoData.HasVideoBeenWatched())
            //m_CompletedButton.SetActive(true);

        if (isUnlocked)
            m_PlayButton.SetActive(true);
        else
            m_LockedButton.SetActive(true);
    }

    public void Click()
    {
        if (VideoPlayerUI.Instance != null)
        {
            VideoPlayerUI.Instance.ShowVideo(m_VideoData);

            //Analytics
            AnalyticsManager.StartVideoEvent(UtilityMethods.RemoveExtention(m_VideoData.VideoPath), true, false);
        }  
    }

    private void Update()
    {
        //if (m_IsHovering && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N))
        //{
        //    m_VideoData.SetVideoWatched();
        //}
    }

    //IPointerEnterHandler
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        m_IsHovering = true;
    }

    //IPointerExitHandler
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        m_IsHovering = false;
    }
}
