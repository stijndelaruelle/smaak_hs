using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButtonUI : MonoBehaviour
{
    LocalizationManager m_LocalizationManager;

    [SerializeField]
    private LocalizationManager.Language m_Language;

    [SerializeField]
    private Button m_Button;

    private void Start()
    {
        m_LocalizationManager = LocalizationManager.Instance;

        if (m_LocalizationManager == null)
        {
            Debug.LogWarning("Language Button doens't have access to the LocalizationManager!");
            return;
        }

        m_LocalizationManager.LanguageChangedEvent += OnLanguageChanged;
        
        //Fake call
        OnLanguageChanged(m_LocalizationManager.CurrentLanguage);
    }

    private void OnDestroy()
    {
        if (m_LocalizationManager == null)
            return;

        m_LocalizationManager.LanguageChangedEvent -= OnLanguageChanged;
    }

    public void OnClick()
    {
        if (m_LocalizationManager != null)
            m_LocalizationManager.SetLanguage(m_Language);
    }

    //Callback from the LocalizationManager
    private void OnLanguageChanged(LocalizationManager.Language language)
    {
        bool isOurLanguage = (m_Language == language);
        m_Button.interactable = (!isOurLanguage);
    }
}
