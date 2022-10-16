using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

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

            GUILayout.BeginVertical(ButtonWizardStyles.MultipleFields);
            EditorGUI.BeginChangeCheck();
            ShowSaveLocation();
            folderPath = EditorGUILayout.TextField(" ", folderPath);
            if (EditorGUI.EndChangeCheck())
            {
                OnSaveLocationChanged();
            }
            GUILayout.EndVertical();

            ShowAvatarField();
            if (avatar == null)
            {
                EditorGUILayout.HelpBox(Localized.baseWindowMsgNoAvatar, MessageType.Info);
            }
            else
            {
                ShowAnimatorField();
                ShowTargetObjectField();

                GUILayout.BeginVertical(ButtonWizardStyles.MultipleFields);
                ShowVrcTargetMenuField();
                vrcTargetMenu = EditorGUILayout.ObjectField(" ", vrcTargetMenu,
                    typeof(VRCExpressionsMenu), false) as VRCExpressionsMenu;
                GUILayout.EndVertical();

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
    }
}