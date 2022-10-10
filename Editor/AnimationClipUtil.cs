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