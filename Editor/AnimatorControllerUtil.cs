using UnityEditor.Animations;
using UnityEngine;

namespace Puetsua.VRCButtonWizard.Editor
{
    internal static class AnimatorControllerUtil
    {
        internal static void TryAddParameter(this AnimatorController controller,
            AnimatorControllerParameter parameter)
        {
            if (controller.HasParameterName(parameter.name))
            {
                return;
            }

            controller.AddParameter(parameter);
        }

        internal static bool HasParameterName(this AnimatorController controller, string parameterName)
        {
            foreach (var parameter in controller.parameters)
            {
                if (parameter.name == parameterName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}