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

            ShowSaveLocation(OnSaveLocationChanged);
            ShowAvatarField();
            if (avatar != null)
            {
                ShowAnimatorField();
                ShowTargetObjectField();
                ShowVrcTargetMenuField();
                ShowMenuNameField();
                ShowVrcParameterField();
                ShowParameterNameField();
                ShowParameterSaveField();
                ShowParameterDefaultField();
                ShowCreateToggleButton();
            }

            // TestZ();
        }

        private void OnSaveLocationChanged()
        {
            SaveFolderPath();
        }

        private void TestZ()
        {
            var test = PtEditorGUILayout.VrcMenuPopup("Toggle Object", VrcRootMenu, vrcTargetMenu);
            if (test != vrcTargetMenu)
            {
                Debug.Log($"{test.name} {vrcTargetMenu.name}");
            }
        }
    }
}