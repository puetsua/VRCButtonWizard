using System;
using UnityEditor.Animations;
using UnityEngine;

namespace Puetsua.VRCButtonWizard.Editor
{
    internal static class AnimatorStateMachineUtil
    {
        internal static AnimatorStateMachine ToggleCreate(Motion clipOn, Motion clipOff,
            string name, string parameterName)
        {
            var stateOn = AnimatorStateUtil.ToggleCreate(clipOn, true);
            var stateOff = AnimatorStateUtil.ToggleCreate(clipOff, false);
            AnimatorStateUtil.ToggleLink(stateOn, stateOff, parameterName);
            return new AnimatorStateMachine
            {
                name = name,
                hideFlags = HideFlags.None,
                states = new[]
                {
                    new ChildAnimatorState
                    {
                        state = stateOn,
                        position = new Vector3(0, 160)
                    },
                    new ChildAnimatorState
                    {
                        state = stateOff,
                        position = new Vector3(0, 60)
                    },
                },
                stateMachines = Array.Empty<ChildAnimatorStateMachine>(),
                defaultState = stateOff,
                anyStatePosition = new Vector3(20, -100),
                entryPosition = new Vector3(20, -30),
                exitPosition = new Vector3(220, -30),
                anyStateTransitions = Array.Empty<AnimatorStateTransition>(),
                entryTransitions = Array.Empty<AnimatorTransition>(),
                behaviours = Array.Empty<StateMachineBehaviour>()
            };
        }
    }
}