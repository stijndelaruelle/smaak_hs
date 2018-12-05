using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatsEnabledLabelUI : MonoBehaviour
{
    [SerializeField]
    private Text m_Text;

    private void Start()
    {
        SaveGameManager.VariableChangedEvent += OnSaveGameVariableChanged;
        SaveGameManager.SaveGameDeletedEvent += OnSaveGameDeletedEvent;

        bool cheatsEnabled = SaveGameManager.GetBool(SaveGameManager.SAVE_CHEATS);
        EnableLabel(cheatsEnabled);
    }

    private void OnDestroy()
    {
        SaveGameManager.VariableChangedEvent -= OnSaveGameVariableChanged;
        SaveGameManager.SaveGameDeletedEvent -= OnSaveGameDeletedEvent;
    }

    private void EnableLabel(bool state)
    {
        m_Text.enabled = state;
    }

    private void OnSaveGameVariableChanged(string key, object value)
    {
        if (key == SaveGameManager.SAVE_CHEATS)
            EnableLabel((bool)value);
    }

    private void OnSaveGameDeletedEvent()
    {
        EnableLabel(false);
    }
}
