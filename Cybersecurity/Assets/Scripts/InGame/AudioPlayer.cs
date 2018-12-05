using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioPlayer : Singleton<AudioPlayer>
{
    [Header("Music")]
    [SerializeField]
    private AudioSource m_MusicAudioSource;
    private AudioClip m_NextMusicClip;

    [Header("SFX")]
    [SerializeField]
    private AudioSource m_OneShotSFXAudioSource; //Fire and Forget, these can't be stopped!

    [SerializeField]
    private List<AudioSource> m_SFXAudioSources; //Limited pool (can make it dynamically grow later), use only if the sound effect should be able to be stopped!
    private int m_CurrentSFXAudioSource = 0;

    private bool m_MuteSFX = false;

    private List<AudioClip> m_AudioClipsPlayedThisFrame;

    protected override void Awake()
    {
        base.Awake();
        m_AudioClipsPlayedThisFrame = new List<AudioClip>();
    }

    private void Update()
    {
        m_AudioClipsPlayedThisFrame.Clear();
    }

    //Music
    public void PlayMusic(AudioClip audioClip)
    {
        //Make sure we don't restart playing the same clip
        if (m_MusicAudioSource.clip == audioClip)
            return;

        if (m_NextMusicClip == audioClip)
            return;

        m_NextMusicClip = audioClip;

        //Fade out
        m_MusicAudioSource.DOFade(0.0f, 0.5f).OnComplete(OnMusicFadeOutComplete);
    }

    private void OnMusicFadeOutComplete()
    {
        if (m_MusicAudioSource.clip != m_NextMusicClip)
        {
            //Change clip
            m_MusicAudioSource.Stop();
            m_MusicAudioSource.clip = m_NextMusicClip;
            m_MusicAudioSource.Play();
        }

        //Fade in
        m_MusicAudioSource.DOFade(1.0f, 0.5f);
    }

    //SFX
    public void PlaySFXOneShot(AudioClip audioClip)
    {
        if (m_MuteSFX)
            return;

        if (audioClip == null)
            return;

        //Avoid playing the same clip multiple times per frame (only increases volume (annoyingly)
        if (m_AudioClipsPlayedThisFrame.Contains(audioClip))
            return;

        m_AudioClipsPlayedThisFrame.Add(audioClip);

        //Actually play the clip
        m_OneShotSFXAudioSource.PlayOneShot(audioClip);
    }

    public int PlaySFX(AudioClip audioClip)
    {
        if (m_MuteSFX)
            return -1;

        if (audioClip == null)
            return -1;

        //Avoid playing the same clip multiple times per frame (only increases volume (annoyingly)
        if (m_AudioClipsPlayedThisFrame.Contains(audioClip))
            return -1;

        m_AudioClipsPlayedThisFrame.Add(audioClip);

        int audioSourceID = m_CurrentSFXAudioSource;

        //Pool size warning
        if (m_SFXAudioSources[audioSourceID].isPlaying)
        {
            Debug.LogWarning("Stopped playing a SFX that was still playing in order to play another one! Maybe increase the pool?");
        }

        //Use the audio source
        m_SFXAudioSources[audioSourceID].Stop();
        m_SFXAudioSources[audioSourceID].clip = audioClip;
        m_SFXAudioSources[audioSourceID].Play();

        m_CurrentSFXAudioSource += 1;

        if (m_CurrentSFXAudioSource >= m_SFXAudioSources.Count)
            m_CurrentSFXAudioSource = 0;

        return audioSourceID;
    }

    public void StopSFX(int audioSourceID, AudioClip audioClip)
    {
        if (audioSourceID < 0 || audioSourceID >= m_SFXAudioSources.Count)
            return;

        if (m_SFXAudioSources[audioSourceID].clip != audioClip)
            return;

        m_SFXAudioSources[audioSourceID].Stop();
    }

    public void IngoreSFX(bool state)
    {
        //Stop accepting any new SFX calls! (first frame of a level, too many calls are made (by mostly doors)
        m_MuteSFX = state;
    }
}
