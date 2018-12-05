using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class AnimationTriggerChancePair
{
    [SerializeField]
    private string m_TriggerName;
    public string TriggerName
    {
        get { return m_TriggerName; }
        set { m_TriggerName = value; }
    }

    [SerializeField]
    private int m_Chance;
    public int Chance
    {
        get { return m_Chance; }
        set { m_Chance = value; }
    }
}

[Serializable]
public class AnimatorTriggerClip : PlayableAsset, ITimelineClipAsset
{
    public List<AnimationTriggerChancePair> m_Triggers; //ExposedReference<string>
    [HideInInspector]
    public AnimatorTriggerBehaviour template = new AnimatorTriggerBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<AnimatorTriggerBehaviour>.Create (graph, template);
        AnimatorTriggerBehaviour clone = playable.GetBehaviour ();

        clone.Triggers = m_Triggers; //.Resolve (graph.GetResolver ());
        return playable;
    }
}
