using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSliderUI : MonoBehaviour
{
    [SerializeField]
    private Slider m_VolumeSlider;

    [SerializeField]
    private AudioMixer m_Mixer;

    [SerializeField]
    private AudioMixerSnapshot m_Snapshot;

    [SerializeField]
    private string m_OptionVariable;

    private void Start()
    {
        SaveGameManager.SaveGameDeletedEvent += OnSaveGameDeletedEvent;
    }

    private void OnDestroy()
    {
        SaveGameManager.SaveGameDeletedEvent -= OnSaveGameDeletedEvent;
    }

    private void OnEnable()
    {
        float db = -80.0f;
        m_Mixer.GetFloat(m_OptionVariable, out db);
        Debug.Log("Loaded " + m_OptionVariable + " db:" + db);

        float value = Mathf.Pow(10, ((db / 2.0f) / 20.0f));
        m_VolumeSlider.value = value;
    }

    public void SetVolume(float value)
    {
        float db = -80.0f;
        if (value > 0)
            db = (Mathf.Log10(value) * 20.0f) * 2; //*2 to make it fade faster

        m_Mixer.SetFloat(m_OptionVariable, db);

        //Save it
        if (m_OptionVariable == "MusicVolume")
            SaveGameManager.SetFloat(SaveGameManager.SAVE_VOLUME_MUSIC, db);

        if (m_OptionVariable == "SFXVolume")
            SaveGameManager.SetFloat(SaveGameManager.SAVE_VOLUME_SFX, db);

        if (m_OptionVariable == "VideoVolume")
            SaveGameManager.SetFloat(SaveGameManager.SAVE_VOLUME_VIDEO, db);
    }

    private void OnSaveGameDeletedEvent()
    {
        //Reset
        SetVolume(1.0f);
        m_VolumeSlider.value = 1.0f;
    }
}
