using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Puetsua.VRCButtonWizard.Editor
{
    internal static class AnimationClipUtil
    {
        private const string ClipNameSuffixShow = "Show";
        private const string ClipNameSuffixHide = "Hide";
        
        internal static AnimationClip ToggleCreate(string assetFolderPath, ToggleProperty[] properties,
            string animName, bool isShown)
        {
            var clipName = isShown
                ? $"{animName}{ClipNameSuffixShow}"
                : $"{animName}{ClipNameSuffixHide}";

            var guids = AssetDatabase.FindAssets($"t:AnimationClip {clipName}", new[] {assetFolderPath});
            if (guids.Length == 1)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var asset = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                Debug.Log($"Using existing AnimationClip '{clipName}' for toggle.");
                return asset;
            }

            var clip = new AnimationClip
            {
                name = clipName,
                frameRate = 60,
            };

            foreach (var property in properties)
            {
                if (property.binding.isPPtrCurve)
                {
                    AnimationUtility.SetObjectReferenceCurve(clip, property.binding,
                        new[] {ToObjectReferenceKeyframe(property, isShown)});
                }
                else
                {
                    AnimationUtility.SetEditorCurve(clip, property.binding,
                        ToAnimationCurve(property, isShown));
                }
            }

            AnimationUtility.SetAnimationClipSettings(clip, new AnimationClipSettings
            {
                loopTime = true,
            });

            var fileName = $"{clipName}.anim";
            AssetDatabase.CreateAsset(clip, Path.Combine(assetFolderPath, fileName));
            return clip;
        }

        private static AnimationCurve ToAnimationCurve(ToggleProperty property, bool isOn)
        {
            var rawValue = isOn ? property.valueOn : property.valueOff;
            float value = 0;

            if (rawValue is bool boolValue)
            {
                value = boolValue ? 1 : 0;
            }
            else if (rawValue is int intValue)
            {
                value = intValue;
            }
            else if (rawValue is float floatValue)
            {
                value = floatValue;
            }
            else
            {
                throw new ArgumentException($"Unhandled type: {value.GetType().Name}");
            }

            return AnimationCurve.Constant(0, 0, value);
        }

        private static ObjectReferenceKeyframe ToObjectReferenceKeyframe(ToggleProperty property, bool isOn)
        {
            var value = isOn ? property.valueOn : property.valueOff;
            return new ObjectReferenceKeyframe
            {
                time = 0,
                value = (Object) value
            };
        }
    }
}