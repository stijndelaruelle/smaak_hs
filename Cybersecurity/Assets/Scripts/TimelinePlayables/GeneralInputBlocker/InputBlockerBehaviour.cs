using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class InputBlockerBehaviour : PlayableBehaviour
{
    public enum InputBlockerState
    {
        None,
        AddGeneralBlocker,
        RemoveGeneralBlocker,
        AddPlayerBlocker,
        RemovePlayerBlocker
    }

    private InputBlockerState m_State;
    public InputBlockerState State
    {
        get { return m_State; }
        set { m_State = value; }
    }

    private bool m_TriggerSet;
    public bool TriggerSet
    {
        get { return m_TriggerSet; }
        set { m_TriggerSet = value; }
    }

    public override void OnPlayableCreate(Playable playable)
    {
        m_TriggerSet = false;
    }
}
