using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerUI : Singleton<VideoPlayerUI>
{
    [Header("Required References")]
    [SerializeField]
    private VideoPlayer m_VideoPlayerPrefab; //The object where we create the video player
    private VideoPlayer m_VideoPlayer;

    [SerializeField]
    private AudioSource m_VideoPlayerAudioSource;

    [SerializeField]
    [Tooltip("Is stitched in front of every filename that get's thrown at us")]
    private string m_RootURL;

    [Header("Audio")]
    [SerializeField]
    private AudioMixerSnapshot m_MainMenuAudioMixerSnapshot;

    [SerializeField]
    private AudioMixerSnapshot m_VideoAudioMixerSnapshot;

    [SerializeField]
    private float m_AudioTransitionDuration;

    [Header("Animation")]
    [SerializeField]
    private Canvas m_VideoCanvas;

    [SerializeField]
    private RectTransform m_VideoRectTransform;

    [SerializeField]
    private RawImage m_VideoImage;

    [SerializeField]
    private float m_FadeToBlackDuration;

    [SerializeField]
    private float m_ScaleDuration;

    private Sequence m_CurrentSequence;
    private string m_CurrentVideoName;

    public event Action VideoHideEvent;

    private void Start()
    {
        //Automatically close the video after playing
        //if (m_VideoPlayer != null)
        //    m_VideoPlayer.loopPointReached += OnLoopPointReached;

        //Default settings
        m_VideoCanvas.gameObject.SetActive(false);
        m_VideoImage.color = Color.black;
        m_VideoRectTransform.localScale = Vector3.zero;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        DestroyVideoPlayer();
    }

    //Show/Hide Animation
    public void ShowVideo(VideoDataDefinition videoData)
    {
        ShowVideo(videoData.VideoPath);
    }

    public void ShowVideo(string videoName)
    {
        if (m_VideoImage == null)
            return;

        if (m_CurrentSequence != null && m_CurrentSequence.IsPlaying())
            return;

        //We create a new video player every time, for reasoning check the function comment
        CreateVideoPlayer();

        //Transition Audio
        m_VideoAudioMixerSnapshot.TransitionTo(m_AudioTransitionDuration);

        //Animation
        m_VideoCanvas.gameObject.SetActive(true);

        m_CurrentSequence = DOTween.Sequence();
        m_CurrentSequence.Append(m_VideoRectTransform.DOScale(1, m_ScaleDuration));
        m_CurrentSequence.Append(m_VideoImage.DOColor(Color.white, m_FadeToBlackDuration));
        m_CurrentSequence.OnComplete(() => EndShowAnimation(videoName));

        m_CurrentSequence.PlayForward();
    }

    private void HideVideo()
    {
        if (m_VideoPlayer == null || m_VideoImage == null)
            return;

        if (m_CurrentSequence != null && m_CurrentSequence.IsPlaying())
            return;

        StopVideo();

        m_CurrentSequence = DOTween.Sequence();
        m_CurrentSequence.Append(m_VideoImage.DOColor(Color.black, m_FadeToBlackDuration));
        m_CurrentSequence.Append(m_VideoRectTransform.DOScale(0, m_ScaleDuration));
        m_CurrentSequence.OnComplete(EndHideAnimation);

        m_CurrentSequence.PlayForward();

        if (VideoHideEvent != null)
            VideoHideEvent();
    }

    private void EndShowAnimation(string videoName)
    {
        PlayVideo(videoName);
    }

    private void EndHideAnimation()
    {
        m_MainMenuAudioMixerSnapshot.TransitionTo(m_AudioTransitionDuration);
        m_VideoCanvas.gameObject.SetActive(false);
        m_CurrentSequence = null;

        DestroyVideoPlayer();
    }

    //Actually talking to the videoplayer
    private void PlayVideo(string videoName)
    {
        if (m_VideoPlayer.isPlaying)
            m_VideoPlayer.Stop();

        m_CurrentVideoName = UtilityMethods.RemoveExtention(videoName);

        m_VideoPlayer.url = m_RootURL + videoName;
        m_VideoPlayer.Play();

        m_CurrentSequence = null;
    }

    private void StopVideo()
    {
        m_VideoPlayer.Stop();

        m_VideoPlayer.targetTexture.DiscardContents();
        m_VideoPlayer.targetTexture.Release();

        m_CurrentVideoName = "";
    }

    //Callback from video player
    private void OnLoopPointReached(VideoPlayer source)
    {
        //Analytics
        AnalyticsManager.StopVideoEvent(m_CurrentVideoName, true, m_VideoPlayer.time);

        HideVideo();
    }

    //Close video via a button
    public void CloseVideo()
    {
        //Analytics
        AnalyticsManager.StopVideoEvent(m_CurrentVideoName, false, m_VideoPlayer.time);

        HideVideo();
    }


    //Unity 2018.2 bug: Video's will only play once on WebGL. Fixed in 2018.3 but there we get an integer overflow
    //https://issuetracker.unity3d.com/issues/webgl-videoplayer-when-a-movie-is-played-the-second-and-subsequent-times-the-screen-doesnt-render
    //This is meant to be a very cheap fix. We create a new videoplayer every time we play a video, so we play it for the first time... every time.
    private void CreateVideoPlayer()
    {
        if (m_VideoPlayer != null)
        {
            DestroyVideoPlayer();
        }

        m_VideoPlayer = GameObject.Instantiate(m_VideoPlayerPrefab, transform);
        m_VideoPlayer.name = "Video Player";
        m_VideoPlayer.transform.SetSiblingIndex(1);

        if (m_VideoPlayerAudioSource != null)
            m_VideoPlayer.SetTargetAudioSource(0, m_VideoPlayerAudioSource);

        if (m_VideoPlayer != null)
            m_VideoPlayer.loopPointReached += OnLoopPointReached;
    }

    private void DestroyVideoPlayer()
    {
        if (m_VideoPlayer != null)
        {
            if (m_VideoPlayer != null)
                m_VideoPlayer.loopPointReached -= OnLoopPointReached;

            GameObject.Destroy(m_VideoPlayer.gameObject);
        }
    }
}
