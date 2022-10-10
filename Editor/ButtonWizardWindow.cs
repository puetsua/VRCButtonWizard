using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Puetsua.VRCButtonWizard.Editor
{
    public class ButtonWizardWindow : ButtonWizardWindowBase
    {
        [MenuItem("Tools/VRChat Button Wizard")]
        private static void OpenWindow()
        {
            EditorWindow wnd = GetWindow<ButtonWizardWindow>();
            wnd.titleContent = new GUIContent("Button Wizard");
            wnd.position = WindowPos;
            wnd.minSize = MinWindowSize;
            wnd.maxSize = MaxWindowSize;
        }

        private void OnGUI()
        {
            GUILayout.Label("VRChat Button Wizard", EditorStyles.boldLabel);
            ShowAvatarField();
            if (avatar != null)
            {
                ShowAvatarOptions();
            }
        }
    }
}