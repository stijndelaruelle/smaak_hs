using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Cyber Security/Story/Dialogue")]
public class DialogueTypeDefinition : ScriptableObject
{
    [Serializable]
    public class DialogueLine
    {
        public enum Side
        {
            Left,
            Right
        }

        [SerializeField]
        private CharacterTypeDefinition m_Character;
        public CharacterTypeDefinition Character
        {
            get { return m_Character; }
        }

        [SerializeField]
        private Side m_SideOfPanel;
        public Side SideOfPanel
        {
            get { return m_SideOfPanel; }
        }

        [SerializeField]
        [LocalizationID]
        private string m_TextLocalizationID;

        public string GetText()
        {
            return LocalizationManager.GetText(m_TextLocalizationID);
        }
    }

    [SerializeField]
    private List<DialogueLine> m_DialogueLines;
    private int m_CurrentDialogueLine;

    public DialogueLine GetDialogueLine(int lineID)
    {
        if (lineID < 0 || lineID >= m_DialogueLines.Count)
            return null;

        return m_DialogueLines[lineID];
    }

    public int GetNumberOfLines()
    {
        return m_DialogueLines.Count;
    }
}