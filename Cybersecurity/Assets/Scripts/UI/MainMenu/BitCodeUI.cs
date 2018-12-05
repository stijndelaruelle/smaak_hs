using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitCodeUI : MonoBehaviour
{
    [Header("Requirements")]

    [SerializeField]
    private LevelDataDefinition m_RequiredLevel;

    [Space(5)]
    [Header("Required References")]
    [SerializeField]
    private CanvasGroup m_CanvasGroup;

    private void Start()
    {
        SaveGameManager.VariableChangedEvent += OnSaveGameVariableChanged;
        SaveGameManager.SaveGameDeletedEvent += OnSaveGameDeletedEvent;

        UpdateLabel();
    }

    private void OnDestroy()
    {
        SaveGameManager.VariableChangedEvent -= OnSaveGameVariableChanged;
        SaveGameManager.SaveGameDeletedEvent -= OnSaveGameDeletedEvent;
    }

    private void UpdateLabel()
    {
        if (m_RequiredLevel == null)
            return;

        bool showBitCode = m_RequiredLevel.HasLevelBeenCompleted();
        m_CanvasGroup.Show(showBitCode);
    }

    private void OnSaveGameVariableChanged(string key, object value)
    {
        UpdateLabel();
    }

    private void OnSaveGameDeletedEvent()
    {
        UpdateLabel();
    }
}
