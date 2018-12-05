using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LoadSettings : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField]
    private AudioMixer m_MainAudioMixer;

    private void Start()
    {
        //Music Volume
        float musicVolumeInDB = 0.0f;
        if (SaveGameManager.HasKey(SaveGameManager.SAVE_VOLUME_MUSIC))
            musicVolumeInDB = SaveGameManager.GetFloat(SaveGameManager.SAVE_VOLUME_MUSIC);

        m_MainAudioMixer.SetFloat("MusicVolume", musicVolumeInDB);

        //SFX Volume
        float SFXVolumeInDB = 0.0f;
        if (SaveGameManager.HasKey(SaveGameManager.SAVE_VOLUME_SFX))
            SFXVolumeInDB = SaveGameManager.GetFloat(SaveGameManager.SAVE_VOLUME_SFX);

        m_MainAudioMixer.SetFloat("SFXVolume", SFXVolumeInDB);

        //Video Volume
        float VideoVolumeInDB = 0.0f;
        if (SaveGameManager.HasKey(SaveGameManager.SAVE_VOLUME_VIDEO))
            SFXVolumeInDB = SaveGameManager.GetFloat(SaveGameManager.SAVE_VOLUME_VIDEO);

        m_MainAudioMixer.SetFloat("VideoVolume", VideoVolumeInDB);
    }
}
