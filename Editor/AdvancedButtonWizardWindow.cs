using UnityEditor;
using UnityEngine;

namespace Puetsua.VRCButtonWizard.Editor
{
    public class AdvancedButtonWizardWindow : ButtonWizardWindowBase
    {
        [MenuItem("Tools/Advanced VRChat Button Wizard")]
        private static void OpenWindow()
        {
            EditorWindow wnd = GetWindow<AdvancedButtonWizardWindow>();
            wnd.titleContent = new GUIContent("Advanced Button Wizard");
            wnd.position = WindowPos;
            wnd.minSize = MinWindowSize;
            wnd.maxSize = MaxWindowSize;
        }

        private void CreateGUI()
        {
            LoadFolderPath();
        }

        private void OnGUI()
        {
            GUILayout.Label("Advanced VRChat Button Wizard", EditorStyles.boldLabel);

            ShowSaveLocation(SaveFolderPath);
            ShowAvatarField();
            if (avatar != null)
            {
                ShowAvatarOptions();
            }
        }
    }
}