using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class InputBlockerClip : PlayableAsset, ITimelineClipAsset
{
    public InputBlockerBehaviour.InputBlockerState m_State;
    [HideInInspector]
    public InputBlockerBehaviour template = new InputBlockerBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<InputBlockerBehaviour>.Create (graph, template);
        InputBlockerBehaviour clone = playable.GetBehaviour ();

        clone.State = m_State;

        return playable;
    }
}
