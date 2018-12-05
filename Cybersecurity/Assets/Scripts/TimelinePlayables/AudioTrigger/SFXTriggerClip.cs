using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class SFXTriggerClip : PlayableAsset, ITimelineClipAsset
{
    [HideInInspector]
    public SFXTriggerBehaviour template = new SFXTriggerBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SFXTriggerBehaviour>.Create (graph, template);
        SFXTriggerBehaviour clone = playable.GetBehaviour ();

        return playable;
    }
}
