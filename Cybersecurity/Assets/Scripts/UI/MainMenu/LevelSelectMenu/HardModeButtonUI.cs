using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HardModeButtonUI : MonoBehaviour
{
    [Header("Disabled Info")]
    [SerializeField]
    private Sprite m_DisabledSprite;

    [SerializeField]
    [LocalizationID]
    private string m_DisabledText;

    [Space(5)]
    [Header("Enabled Info")]
    [SerializeField]
    private Sprite m_EnabledSprite;

    [SerializeField]
    [LocalizationID]
    private string m_EnabledText;

    [Header("Required References")]
    [SerializeField]
    private Image m_Image;

    [SerializeField]
    private Text m_Text;

    private void Start()
    {
        SaveGameManager.BoolChangedEvent += OnSaveGameBoolVariableChanged;
        SaveGameManager.SaveGameDeletedEvent += OnSaveGameDeletedEvent;

        UpdateButton();
    }

    private void OnDestroy()
    {
        SaveGameManager.BoolChangedEvent -= OnSaveGameBoolVariableChanged;
        SaveGameManager.SaveGameDeletedEvent -= OnSaveGameDeletedEvent;
    }

    private void UpdateButton()
    {
        bool isHardMode = SaveGameManager.GetBool(SaveGameManager.SAVE_HARDMODE, false); //By default hard mode is disabled

        //Set the correct sprite
        Sprite currentSprite = m_DisabledSprite;
        if (isHardMode) { currentSprite = m_EnabledSprite; }

        if (m_Image != null)
            m_Image.sprite = currentSprite;

        //Set tge cyrrebt text
        string currentText = m_DisabledText;
        if (isHardMode) { currentText = m_EnabledText; }

        if (m_Text != null)
            m_Text.text = LocalizationManager.GetText(currentText);
    }

    public void Click()
    {
        //Toggle Hard Mode
        bool isHardMode = SaveGameManager.GetBool(SaveGameManager.SAVE_HARDMODE, false); //By default hard mode is disabled
        SaveGameManager.SetBool(SaveGameManager.SAVE_HARDMODE, !isHardMode);
    }

    private void OnSaveGameBoolVariableChanged(string key, bool value)
    {
        if (key == SaveGameManager.SAVE_HARDMODE)
            UpdateButton();
    }

    private void OnSaveGameDeletedEvent()
    {
        UpdateButton();
    }
}
