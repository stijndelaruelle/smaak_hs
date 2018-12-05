using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueLabelUI : MonoBehaviour
{
    [SerializeField]
    private DialogueUI m_DialogueUI;

    [SerializeField]
    private Text m_Label;

    [SerializeField]
    [Tooltip("Checking this will always have priority over the text")]
    private bool m_ShowName = false;

    [SerializeField]
    private DialogueTypeDefinition.DialogueLine.Side m_Side = DialogueTypeDefinition.DialogueLine.Side.Left;

    private void Start()
    {
        if (m_DialogueUI != null)
            m_DialogueUI.DialogueNewLineEvent += OnDialogueNewLine;
    }

    private void OnDestroy()
    {
        if (m_DialogueUI != null)
            m_DialogueUI.DialogueNewLineEvent -= OnDialogueNewLine;
    }

    private void OnDialogueNewLine(int dialogueLineID, DialogueTypeDefinition.DialogueLine dialogueLine)
    {
        m_Label.enabled = (m_Side == dialogueLine.SideOfPanel);

        if (m_Label.enabled == false)
            return;

        //Color label & change font (only when it's the name)
        if (m_ShowName)
        {
            m_Label.color = dialogueLine.Character.TextColor;
            m_Label.font = dialogueLine.Character.TextFont;
        }

        //Fill label
        if (m_ShowName == true)
        {
            m_Label.text = dialogueLine.Character.GetName();
            return;
        }

        m_Label.text = dialogueLine.GetText();
    }
}
