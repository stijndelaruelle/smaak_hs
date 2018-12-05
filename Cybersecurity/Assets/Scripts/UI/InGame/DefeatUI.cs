using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatUI : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Panel;

    [SerializeField]
    private AudioClip m_DefeatSound;

    private void Start()
    {
        LevelDirector levelManager = LevelDirector.Instance;
        if (levelManager != null)
        {
            levelManager.LevelEndDefeatEvent += OnDefeat;
            levelManager.LevelResetEvent += OnReset;
        }
    }

    private void OnDestroy()
    {
        LevelDirector levelManager = LevelDirector.Instance;

        if (levelManager == null)
            return;

        levelManager.LevelEndDefeatEvent -= OnDefeat;
        levelManager.LevelResetEvent -= OnReset;
    }

    private void Update()
    {
        if (m_Panel.activeInHierarchy == false)
            return;

        if (Input.anyKeyDown)
        {
            LevelDirector.Instance.ResetLevel();
        }
    }

    private void OnDefeat()
    {
        m_Panel.SetActive(true);

        //SFX
        if (AudioPlayer.Instance != null)
            AudioPlayer.Instance.PlaySFXOneShot(m_DefeatSound);
    }

    private void OnReset()
    {
        m_Panel.SetActive(false);
    }
}
