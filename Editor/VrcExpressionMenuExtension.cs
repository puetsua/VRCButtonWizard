using VRC.SDK3.Avatars.ScriptableObjects;

namespace Puetsua.VRCButtonWizard.Editor
{
    internal static class VrcExpressionMenuExtension
    {
        internal static void AddToggle(this VRCExpressionsMenu menu, string menuName, string parameterName)
        {
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
    }
}