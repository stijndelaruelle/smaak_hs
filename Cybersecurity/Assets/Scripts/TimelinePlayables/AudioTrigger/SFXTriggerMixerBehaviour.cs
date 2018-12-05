using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SFXTriggerMixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        AudioClip trackBinding = playerData as AudioClip;

        if (!trackBinding)
            return;

        int inputCount = playable.GetInputCount();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<SFXTriggerBehaviour> inputPlayable = (ScriptPlayable<SFXTriggerBehaviour>)playable.GetInput(i);
            SFXTriggerBehaviour input = inputPlayable.GetBehaviour();

            if (inputWeight > 0.5f && !input.TriggerSet)
            {
                if (AudioPlayer.Instance != null)
                    AudioPlayer.Instance.PlaySFXOneShot(trackBinding);
                
                input.TriggerSet = true;
            }
        }
    }
}
