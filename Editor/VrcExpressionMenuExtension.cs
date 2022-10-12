using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Puetsua.VRCButtonWizard.Editor
{
    internal static class VrcExpressionMenuExtension
    {
        internal static void AddToggle(this VRCExpressionsMenu menu, string menuName, string parameterName)
        {
            if (HasToggleWithParameter(menu, parameterName))
            {
                Debug.LogWarning($"There is already a toggle uses parameter '{parameterName}' in menu '{menu.name}'");
                return;
            }

            menu.controls.Add(new VRCExpressionsMenu.Control
            {
                name = menuName,
                icon = null,
                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                parameter = new VRCExpressionsMenu.Control.Parameter
                {
                    name = parameterName
                },
            });
        }

        internal static bool HasToggleWithParameter(this VRCExpressionsMenu menu, string parameterName)
        {
            foreach (var control in menu.controls)
            {
                if (control.type == VRCExpressionsMenu.Control.ControlType.Toggle &&
                    control.parameter.name == parameterName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}