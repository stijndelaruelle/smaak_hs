using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDiedError : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_Label;

    [SerializeField]
    [LocalizationID]
    private string m_NormalText;

    [SerializeField]
    [LocalizationID]
    private string m_DeathText;

    private void Start()
    {
        if (LevelDirector.Instance != null)
            LevelDirector.Instance.LevelEndDefeatEvent += OnPlayerDied;

        m_Label.text = LocalizationManager.GetText(m_NormalText);
    }

    private void OnDestroy()
    {
        if (LevelDirector.Instance != null)
            LevelDirector.Instance.LevelEndDefeatEvent -= OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        m_Label.text = LocalizationManager.GetText(m_DeathText);
        LevelDirector.Instance.LevelEndDefeatEvent -= OnPlayerDied;
    }
}
