using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class AnimatorTriggerBehaviour : PlayableBehaviour
{
    private List<AnimationTriggerChancePair> m_Triggers;
    public List<AnimationTriggerChancePair> Triggers
    {
        get { return m_Triggers; }
        set { m_Triggers = value; }
    }

    private bool m_TriggerSet;
    public bool TriggerSet
    {
        get { return m_TriggerSet; }
        set { m_TriggerSet = value; }
    }

    public override void OnPlayableCreate (Playable playable)
    {
        m_TriggerSet = false;
    }
}
