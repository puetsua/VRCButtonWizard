using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Puetsua.VRCButtonWizard.Editor
{
    internal static class AnimatorStateUtil
    {
        internal static AnimatorState ToggleCreate(string parentAssetPath, Motion motion, bool isOn)
        {
            var state = new AnimatorState
            {
                name = isOn ? "On" : "Off",
                hideFlags = HideFlags.HideInHierarchy,
                motion = motion,
                writeDefaultValues = true,
                tag = null,
                transitions = Array.Empty<AnimatorStateTransition>(),
                behaviours = Array.Empty<StateMachineBehaviour>()
            };
            AssetDatabase.AddObjectToAsset(state, parentAssetPath);
            return state;
        }

        internal static void ToggleLink(string parentAssetPath,
            AnimatorState stateOn, AnimatorState stateOff, string parameterName)
        {
            var transition = new AnimatorStateTransition
            {
                hideFlags = HideFlags.HideInHierarchy,
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
            };
            stateOn.transitions = new[] {transition};
            AssetDatabase.AddObjectToAsset(transition, parentAssetPath);

            transition = new AnimatorStateTransition
            {
                hideFlags = HideFlags.HideInHierarchy,
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
            };
            stateOff.transitions = new[] {transition};
            AssetDatabase.AddObjectToAsset(transition, parentAssetPath);
        }
    }
}