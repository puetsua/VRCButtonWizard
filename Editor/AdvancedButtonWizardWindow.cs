using UnityEditor;
using UnityEngine;

namespace Puetsua.VRCButtonWizard.Editor
{
    public class AdvancedButtonWizardWindow : ButtonWizardWindowBase
    {
        private bool settingFoldout;

        private void CreateGUI()
        {
            LoadFolderPath();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical(ButtonWizardStyles.Box);
            GUILayout.Label(Localized.advancedButtonWizardWindowTitle, ButtonWizardStyles.Title);

            ShowSaveLocation(OnSaveLocationChanged);
            ShowAvatarField();
            if (avatar == null)
            {
                EditorGUILayout.HelpBox(Localized.buttonWizardWindowMsgNoAvatar, MessageType.Info);
            }
            else
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

            GUILayout.EndVertical();

            settingFoldout = EditorGUILayout.Foldout(settingFoldout, Localized.settingWindowLabelTitle);
            if (settingFoldout)
            {
                ShowLanguageOption();
            }
            
            GUILayout.FlexibleSpace();
            ShowFooter();
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