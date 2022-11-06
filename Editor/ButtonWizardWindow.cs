using System.IO;
using UnityEditor;
using UnityEngine;

namespace Puetsua.VRCButtonWizard.Editor
{
    public class ButtonWizardWindow : ButtonWizardWindowBase, IHasCustomMenu
    {
        [MenuItem("Tools/VRChat Button Wizard", false, ButtonWizardConst.MenuItemPriority)]
        public static void OpenPreferredWindow()
        {
            if (ButtonWizardPref.AlwaysAdvanced)
            {
                AdvancedButtonWizardWindow.OpenWindow();
            }
            else
            {
                OpenWindow();
            }
        }

        public static void OpenWindow()
        {
            var wnd = GetWindow<ButtonWizardWindow>(Localized.buttonWizardWindowName);
            wnd.position = WindowPos;
            wnd.minSize = MinWindowSize;
            wnd.maxSize = MaxWindowSize;
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent(Localized.baseWindowMenuToggleAlwaysAdvanced),
                ButtonWizardPref.AlwaysAdvanced, ToggleAlwaysAdvanced);
            menu.AddItem(new GUIContent(Localized.buttonWizardWindowMenuAdvanced),
                false, AdvancedButtonWizardWindow.OpenWindow);
        }

        private void CreateGUI()
        {
            SetFolderPath();
        }

        private void OnGUI()
        {
            ShowLanguageOption();

            GUILayout.BeginVertical(ButtonWizardStyles.Box);
            GUILayout.Label(Localized.buttonWizardWindowTitle, ButtonWizardStyles.Title);

            ShowAvatarField(OnAvatarChanged);
            if (avatar == null)
            {
                EditorGUILayout.HelpBox(Localized.baseWindowMsgNoAvatar, MessageType.Info);
            }
            else
            {
                ShowMenuNameField();
                ShowParameterSaveField();
                ShowParameterDefaultField();
                ShowTargetObjectsField();
                ShowCreateToggleButton();
            }

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            ShowFooter();
        }

        private void OnAvatarChanged()
        {
            SetFolderPath();
        }

        private void SetFolderPath()
        {
            if (avatar == null)
                return;

            var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(avatar);
            if (string.IsNullOrEmpty(path))
            {
                path = avatar.gameObject.scene.path;
            }

            path = Path.GetDirectoryName(path) ?? "";
            path = Path.Combine(path, "Animations");

            folderPath = path;
            Debug.Log($"Planned to save generated AnimationClip to '{path}'");
        }
    }
}