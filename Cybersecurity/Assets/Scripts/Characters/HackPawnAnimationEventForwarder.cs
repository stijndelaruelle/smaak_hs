using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackPawnAnimationEventForwarder : MonoBehaviour
{
    [SerializeField]
    private HackPawn m_HackPawn;

    public void OnStep()
    {
        //Play sound effect
    }

    public void OnMoveAnimationStart()
    {
        m_HackPawn.OnMoveAnimationStart();
    }

    public void OnMoveAnimationEnd()
    {
        m_HackPawn.OnMoveAnimationEnd();
    }

    public void OnAlertAnimationEnd()
    {
        m_HackPawn.OnAlertAnimationEnd();
    }
}
