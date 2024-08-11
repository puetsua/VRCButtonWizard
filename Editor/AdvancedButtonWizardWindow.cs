using System.Linq;
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
                false, OpenSimpleWindow);
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
            ShowAnimatorField();
            ShowVrcTargetMenuField();
            ShowMenuNameField();
            ShowVrcParameterField();
            ShowParameterNameField();
            ShowParameterSaveField();
            ShowParameterDefaultField();
            ShowInverseToggleField();
            ShowAnimatorControllerField();
            ShowTargetObjectsField();
            ShowCreateToggleButton();

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

        private void OpenSimpleWindow()
        {
            ButtonWizardWindow.OpenWindow();
            Close();
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

        private void ShowCreateToggleButton()
        {
            bool areTargetObjectValid = targetProperties.Count > 0;
            bool hasVrcTargetMenu = vrcTargetMenu != null;
            bool hasTargetAnimator = targetAnimator != null;

            GuiEnableIf(
                !string.IsNullOrWhiteSpace(menuName) &&
                !string.IsNullOrWhiteSpace(parameterName) &&
                hasVrcTargetMenu, () =>
                {
                    if (GUILayout.Button(Localized.advancedButtonWizardWindowButtonCreateVrcToggle))
                    {
                        CreateVrcToggle(menuName, parameterName, isParamSaved, defaultBool);
                        AssetDatabase.SaveAssets();
                    }
                });

            GuiEnableIf(!string.IsNullOrWhiteSpace(menuName) &&
                        !string.IsNullOrWhiteSpace(parameterName), () =>
            {
                if (GUILayout.Button(Localized.advancedButtonWizardWindowButtonCreateAnimatorControllerLayer))
                {
                    CreateToggleAnimatorLayerOnly(menuName, parameterName);
                    AssetDatabase.SaveAssets();
                }
            });

            GuiEnableIf(!string.IsNullOrWhiteSpace(menuName) &&
                        !string.IsNullOrWhiteSpace(parameterName) &&
                        hasTargetAnimator &&
                        areTargetObjectValid, () =>
            {
                if (GUILayout.Button(Localized.advancedButtonWizardWindowButtonCreateAnimationClips))
                {
                    CreateToggleClipsOnly(menuName, parameterName);
                    AssetDatabase.SaveAssets();
                }
            });

            GuiEnableIf(!string.IsNullOrWhiteSpace(menuName) &&
                        !string.IsNullOrWhiteSpace(parameterName) &&
                        hasVrcTargetMenu &&
                        hasTargetAnimator &&
                        areTargetObjectValid, () =>
            {
                if (GUILayout.Button(Localized.baseWindowButtonCreateToggle))
                {
                    CreateVrcToggle(menuName, parameterName, isParamSaved, defaultBool);
                    CreateToggle(menuName, parameterName);
                    AssetDatabase.SaveAssets();
                }
            });

            GUI.enabled = true;
        }

        private void GuiEnableIf(bool condition, System.Action guiAction)
        {
            bool guiEnabled = GUI.enabled;
            GUI.enabled = condition;
            guiAction();
            GUI.enabled = guiEnabled;
        }
    }
}