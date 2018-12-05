using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : Switch
{
    [Space(5)]
    [Header("Specific - Pressure Plate")]

    [SerializeField]
    private Animator m_Animator;

    public override void OnCharacterEnter(Character character, Direction direction, bool snap)
    {
        bool wasOn = m_IsOn;

        base.OnCharacterEnter(character, direction, snap);

        if (m_Animator == null)
            return;

        //We just went on!
        if (m_IsOn == true && wasOn == false)
        {
            m_Animator.SetTrigger("Press");
        }
    }

    public override void OnCharacterLeave(Character character, Direction direction)
    {
        bool wasOn = m_IsOn;

        base.OnCharacterLeave(character, direction);

        if (m_Animator == null)
            return;

        //We just went off!
        if (m_IsOn == false && wasOn == true)
        {
            m_Animator.SetTrigger("Release");
        }
    }
}
