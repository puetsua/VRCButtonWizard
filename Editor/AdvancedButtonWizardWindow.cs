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
                ShowInverseToggleField();

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

        private void ShowTargetObjectsField2()
        {
            if (targetProperties.Count >= 1 && GUILayout.Button("test"))
            {
                var list = AnimationUtility.GetAnimatableBindings(targetProperties[0].gameObject, avatar.gameObject);
                Debug.Log(string.Join("\n", list.Select(
                    b =>
                    {
                        string typeName = null;
                        var animObj = AnimationUtility.GetAnimatedObject(avatar.gameObject, b);
                        if (animObj != null)
                        {
                            var serializedObject = new SerializedObject(animObj);
                            var property = serializedObject.FindProperty(b.propertyName);
                            typeName = property?.type;
                        }

                        if (string.IsNullOrWhiteSpace(typeName))
                        {
                            return "";
                        }

                        return $"{b.propertyName} // " +
                               $"{b.path} // " +
                               $"{typeName}";
                    })));
            }
        }

        private void ShowCreateToggleButton()
        {
            bool areTargetObjectValid = targetProperties.Count > 0;

            GUI.enabled = !string.IsNullOrWhiteSpace(menuName) &&
                          !string.IsNullOrWhiteSpace(parameterName);

            if (GUILayout.Button(Localized.advancedButtonWizardWindowButtonCreateVrcToggle))
            {
                CreateVrcToggle(menuName, parameterName, isParamSaved, defaultBool);
                AssetDatabase.SaveAssets();
            }

            if (GUILayout.Button(Localized.advancedButtonWizardWindowButtonCreateAnimatorControllerLayer))
            {
                CreateToggleAnimatorLayerOnly(menuName, parameterName);
                AssetDatabase.SaveAssets();
            }

            GUI.enabled = true;
            GUI.enabled = !string.IsNullOrWhiteSpace(menuName) &&
                          !string.IsNullOrWhiteSpace(parameterName) &&
                          areTargetObjectValid;

            if (GUILayout.Button(Localized.advancedButtonWizardWindowButtonCreateAnimationClips))
            {
                CreateToggleClipsOnly(menuName, parameterName);
                AssetDatabase.SaveAssets();
            }

            if (GUILayout.Button(Localized.baseWindowButtonCreateToggle))
            {
                CreateVrcToggle(menuName, parameterName, isParamSaved, defaultBool);
                CreateToggle(menuName, parameterName);
                AssetDatabase.SaveAssets();
            }

            GUI.enabled = true;
        }
    }
}