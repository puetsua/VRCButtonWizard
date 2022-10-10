using System;
using UnityEditor.Animations;
using UnityEngine;

namespace Puetsua.VRCButtonWizard.Editor
{
    internal static class AnimatorStateUtil
    {
        internal static AnimatorState ToggleCreate(Motion motion, bool isOn)
        {
            return new AnimatorState
            {
                name = isOn ? "On" : "Off",
                hideFlags = HideFlags.None,
                motion = motion,
                writeDefaultValues = false,
                tag = null,
                transitions = Array.Empty<AnimatorStateTransition>(),
                behaviours = Array.Empty<StateMachineBehaviour>()
            };
        }

        internal static void ToggleLink(AnimatorState stateOn, AnimatorState stateOff, string parameterName)
        {
            stateOn.transitions = new[]
            {
                new AnimatorStateTransition
                {
                    hideFlags = HideFlags.None,
                    isExit = false,
                    destinationState = stateOff,
                    conditions = new[]
                    {
                        new AnimatorCondition
                        {
                            mode = AnimatorConditionMode.IfNot,
                            parameter = parameterName,
                        }
                    },
                    duration = 0,
                    offset = 0,
                    exitTime = 0,
                    hasExitTime = false,
                    hasFixedDuration = true,
                }
            };

            stateOff.transitions = new[]
            {
                new AnimatorStateTransition
                {
                    hideFlags = HideFlags.None,
                    isExit = false,
                    destinationState = stateOn,
                    conditions = new[]
                    {
                        new AnimatorCondition
                        {
                            mode = AnimatorConditionMode.If,
                            parameter = parameterName,
                        }
                    },
                    duration = 0,
                    offset = 0,
                    exitTime = 0,
                    hasExitTime = false,
                    hasFixedDuration = true,
                }
            };
        }
    }
}