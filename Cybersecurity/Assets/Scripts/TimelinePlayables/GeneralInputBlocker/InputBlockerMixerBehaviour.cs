using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class InputBlockerMixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        LevelDirector trackBinding = playerData as LevelDirector;

        if (!trackBinding)
            return;

        int inputCount = playable.GetInputCount();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<InputBlockerBehaviour> inputPlayable = (ScriptPlayable<InputBlockerBehaviour>)playable.GetInput(i);
            InputBlockerBehaviour input = inputPlayable.GetBehaviour();

            if (inputWeight > 0.5f && !input.TriggerSet)
            {
                if (trackBinding != null)
                {
                    switch(input.State)
                    {
                        case InputBlockerBehaviour.InputBlockerState.AddGeneralBlocker:
                        {
                            trackBinding.AddGeneralInputBlocker("Timeline");
                            break;
                        }

                        case InputBlockerBehaviour.InputBlockerState.RemoveGeneralBlocker:
                        {
                            trackBinding.RemoveGeneralInputBlocker("Timeline");
                            break;
                        }

                        case InputBlockerBehaviour.InputBlockerState.AddPlayerBlocker:
                        {
                            trackBinding.AddInputBlocker("Timeline");
                            break;
                        }

                        case InputBlockerBehaviour.InputBlockerState.RemovePlayerBlocker:
                        {
                            trackBinding.RemoveInputBlocker("Timeline");
                            break;
                        }

                        default:
                            break;
                    }
                }

                input.TriggerSet = true;
            }
        }
    }
}
