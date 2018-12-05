using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicOnAwake : MonoBehaviour
{
    [SerializeField]
    private AudioClip m_AudioClip;

    [SerializeField]
    private LevelThemeDefinition m_LevelThemeDefinition;

    private void Start()
    {
        AudioClip clip = m_AudioClip;

        if (clip == null)
            clip = m_LevelThemeDefinition.Music;

        if (clip == null)
            return;

        if (AudioPlayer.Instance == null)
            return;

        AudioPlayer.Instance.PlayMusic(clip);
    }
}
