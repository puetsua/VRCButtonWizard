using System.IO;
using UnityEditor;
using UnityEngine;

namespace Puetsua.VRCButtonWizard.Editor
{
    internal static class AnimationClipUtil
    {
        internal static AnimationClip ToggleCreate(string assetFolderPath, string hierarchyPath, string animName,
            bool isOn)
        {
            var clipName = isOn
                ? $"{animName}On"
                : $"{animName}Off";

            var guids = AssetDatabase.FindAssets($"t:AnimationClip {clipName}", new[] {assetFolderPath});
            if (guids.Length == 1)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var asset = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                Debug.Log($"Using existing AnimationClip '{clipName}' for toggle.");
                return asset;
            }

            if (guids.Length > 1)
            {
                Debug.LogError("Multiple AnimationClips with same name?!");
                return null;
            }

            var clip = new AnimationClip
            {
                name = clipName,
                frameRate = 60,
            };

            var curve = AnimationCurve.Constant(0, 0, isOn ? 1 : 0);
            clip.SetCurve(hierarchyPath, typeof(GameObject), PropertyNameConst.IsActive, curve);
            AnimationUtility.SetAnimationClipSettings(clip, new AnimationClipSettings
            {
                loopTime = true,
            });

            var fileName = $"{clipName}.anim";
            AssetDatabase.CreateAsset(clip, Path.Combine(assetFolderPath, fileName));
            return clip;
        }
    }
}