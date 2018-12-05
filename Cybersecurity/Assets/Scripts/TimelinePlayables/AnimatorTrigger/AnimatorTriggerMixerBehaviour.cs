using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class AnimatorTriggerMixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Animator trackBinding = playerData as Animator;

        if (!trackBinding)
            return;

        int inputCount = playable.GetInputCount();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<AnimatorTriggerBehaviour> inputPlayable = (ScriptPlayable<AnimatorTriggerBehaviour>)playable.GetInput(i);
            AnimatorTriggerBehaviour input = inputPlayable.GetBehaviour();

            if (inputWeight > 0.5f && !input.TriggerSet)
            {
                if (input.Triggers.Count == 1)
                {
                    trackBinding.SetTrigger(input.Triggers[0].TriggerName);
                }

                else if (input.Triggers.Count > 1)
                {
                    //Choose a trigger depending on chance
                    int totalChance = 0;

                    foreach (AnimationTriggerChancePair pair in input.Triggers)
                    {
                        totalChance += pair.Chance;
                    }

                    int rand = UnityEngine.Random.Range(0, totalChance);

                    int offset = 0;
                    string triggerName = "";

                    foreach (AnimationTriggerChancePair pair in input.Triggers)
                    {
                        if (rand < (offset + pair.Chance))
                        {
                            triggerName = pair.TriggerName;
                            break;
                        }

                        offset += pair.Chance;
                    }

                    //Actually trigger the trigger!
                    trackBinding.SetTrigger(triggerName);
                }
                
                input.TriggerSet = true;
            }
        }
    }
}
