using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cyber Security/Story/Character")]
public class CharacterTypeDefinition : ScriptableObject
{
    [SerializeField]
    private string m_NameLocalizationID;

    [SerializeField]
    private Sprite m_DialogueSprite;
    public Sprite DialogueSprite
    {
        get { return m_DialogueSprite; }
    }

    [SerializeField]
    private Font m_TextFont;
    public Font TextFont
    {
        get { return m_TextFont; }
    }

    [SerializeField]
    private Color m_TextColor;
    public Color TextColor
    {
        get { return m_TextColor; }
    }

    //private float m_TextSpeed

    public string GetName()
    {
        return LocalizationManager.GetText(m_NameLocalizationID);
    }
}