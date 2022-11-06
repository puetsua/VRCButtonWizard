using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Puetsua.VRCButtonWizard.Editor
{
    public class AdvancedButtonWizardWindow : ButtonWizardWindowBase, IHasCustomMenu
    {
        private bool _settingFoldout;

        public static void OpenWindow()
        {
            var wnd = GetWindow<AdvancedButtonWizardWindow>(Localized.advancedButtonWizardWindowName);
            wnd.position = WindowPos;
            wnd.minSize = MinWindowSize;
            wnd.maxSize = MaxWindowSize;
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent(Localized.baseWindowMenuToggleAlwaysAdvanced),
                ButtonWizardPref.AlwaysAdvanced, ToggleAlwaysAdvanced);
            menu.AddItem(new GUIContent(Localized.advancedButtonWizardWindowMenuSimple),
                false, ButtonWizardWindow.OpenWindow);
        }

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

                ShowAnimatorField();
                ShowTargetObjectsField();

                ShowCreateToggleButton();
            }

            GUILayout.EndVertical();

            _settingFoldout = EditorGUILayout.Foldout(_settingFoldout, Localized.settingWindowLabelTitle);
            if (_settingFoldout)
            {
                ShowLanguageOption();
                ShowAlwaysAdvancedOption();
            }

            GUILayout.FlexibleSpace();
            ShowFooter();
        }

        private void OnSaveLocationChanged()
        {
            SaveFolderPath();
        }

        private void ShowAlwaysAdvancedOption()
        {
            ButtonWizardPref.AlwaysAdvanced = EditorGUILayout.Toggle(Localized.baseWindowMenuToggleAlwaysAdvanced,
                ButtonWizardPref.AlwaysAdvanced);
        }
    }
}