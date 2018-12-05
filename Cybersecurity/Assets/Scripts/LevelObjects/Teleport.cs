using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : LevelObject
{
    [Space(5)]
    [Header("Specific - Teleport")]
    [SerializeField]
    private Node m_TargetNode;

    [SerializeField]
    private FactionTypeDefinition m_AllowedFaction;

    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private Renderer m_ColorModel;

    [SerializeField]
    private Material m_EnabledMaterial;

    [SerializeField]
    private Material m_DisabledMaterial;

    public override void OnCharacterEnter(Character character, Direction direction, bool snap)
    {
        base.OnCharacterEnter(character, direction, snap);

        if (CanUse(character) == false)
            return;

        //We just teleported here, don't teleport back we'll get a stack overflow.
        if (snap == true)
            return;

        character.TeleportToNode(m_TargetNode);
        m_Animator.SetTrigger("Teleport");
        return;
    }

    public override bool CanUse(Character character)
    {
        if (character.Faction != m_AllowedFaction)
            return false;

        return m_IsEnabled;
    }

    public override bool CanUse(Character character, out string errorMessage)
    {
        errorMessage = "";

        if (m_IsEnabled == false)
        {
            errorMessage = "Can't use Teleporter: It is disabled";
            return false;
        }

        return true;
    }

    public override void SetEnabled(bool state)
    {
        base.SetEnabled(state);

        if (m_ColorModel == null)
            return;

        if (m_IsEnabled)
        {
            if (m_EnabledMaterial != null)
                m_ColorModel.material = m_EnabledMaterial;
        }
        else
        {
            if (m_DisabledMaterial != null)
                m_ColorModel.material = m_DisabledMaterial;
        }
    }
}
