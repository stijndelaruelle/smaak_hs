using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : LevelObject
{
    [Space(5)]
    [Header("Specific - Dialogue Trigger")]
    [SerializeField]
    private Dialogue m_Dialogue;

    [SerializeField]
    private FactionTypeDefinition m_AllowedFaction;

    //LevelObject
    public override void OnCharacterEnter(Character character, Direction direction, bool snap)
    {
        if (gameObject.activeSelf == false)
            return;

        if (m_IsEnabled == false)
            return;

        if (m_Dialogue == null)
            return;

        if (character.Faction != m_AllowedFaction)
            return;

        //Start Dialogue
        m_Dialogue.StartDialogue();

        SetEnabled(false);
    }

    protected override void OnReset()
    {
        //Do not reset the trigger. We don't want players to read the same dialogue twice (and softlock the game in some cases)
    }
}
