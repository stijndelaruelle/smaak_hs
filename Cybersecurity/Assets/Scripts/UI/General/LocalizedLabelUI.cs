using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedLabelUI : LocalizedObject
{
    [SerializeField]
    [LocalizationID]
    private string m_LocalizationID;

    [SerializeField]
    private List<string> m_TokensUsed;

    private Text m_Label;
    private InputField m_InputField;
    private TextMeshProUGUI m_TextMeshProLabel;

    protected override void Start()
    {
        base.Start();

        //Little bit dirty, but this saves us so much editor trouble
        m_Label = gameObject.GetComponent<Text>();
        m_InputField = gameObject.GetComponent<InputField>();
        m_TextMeshProLabel = gameObject.GetComponent<TextMeshProUGUI>();

        UpdateLabel();
    }

    private void UpdateLabel()
    {
        //Actually set the text
        if (m_Label != null)
            m_Label.text = LocalizationManager.GetText(m_LocalizationID);

        if (m_InputField != null)
            m_InputField.text = LocalizationManager.GetText(m_LocalizationID);

        if (m_TextMeshProLabel != null)
            m_TextMeshProLabel.text = LocalizationManager.GetText(m_LocalizationID);
    }

    protected override void OnLanguageChanged(LocalizationManager.Language language)
    {
        UpdateLabel();
    }

    protected override void OnTokenChanged(string key)
    {
        //One of the tokens has been updated
        if (m_TokensUsed.Contains(key))
        {
            UpdateLabel();
        }
    }
}
