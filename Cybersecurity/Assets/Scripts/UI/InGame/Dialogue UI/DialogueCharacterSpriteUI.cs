using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueCharacterSpriteUI : MonoBehaviour
{
    [SerializeField]
    private DialogueUI m_DialogueUI;

    [SerializeField]
    private Image m_Image;

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
        m_Image.enabled = (m_Side == dialogueLine.SideOfPanel);

        if (m_Image.enabled == false)
            return;

        m_Image.sprite = dialogueLine.Character.DialogueSprite;
    }
}
