﻿using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Puetsua.VRCButtonWizard.Editor
{
    internal static class AnimatorStateMachineUtil
    {
        internal static AnimatorStateMachine ToggleCreate(string parentAssetPath,
            AnimatorState stateOff, AnimatorState stateOn, string name)
        {
            var stateMachine = new AnimatorStateMachine
            {
                name = name,
                hideFlags = HideFlags.HideInHierarchy,
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

            AssetDatabase.AddObjectToAsset(stateMachine, parentAssetPath);
            return stateMachine;
        }
    }
}