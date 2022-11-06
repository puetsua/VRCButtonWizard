using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Puetsua.VRCButtonWizard.Editor
{
    public class ButtonWizardWindowBase : EditorWindow
    {
        public static bool debug = false;

        protected static readonly Rect WindowPos = new Rect(200, 200, 400, 600);
        protected static readonly Vector2 MinWindowSize = new Vector2(450, 200);
        protected static readonly Vector2 MaxWindowSize = new Vector2(1280, 720);
        protected static LocalizedTextDataset Localized => LocalizedTextDataset.primary;

        protected VRCAvatarDescriptor avatar;
        protected AnimatorController targetAnimatorController;
        protected List<ToggleProperty> targetProperties = new List<ToggleProperty>();
        protected VRCExpressionParameters vrcParameters;
        protected VRCExpressionsMenu vrcTargetMenu;
        protected string menuName;
        protected string parameterName;
        protected string folderPath;
        protected bool isParamSaved;
        protected bool defaultBool;

        protected VRCExpressionsMenu VrcRootMenu => avatar == null ? null : avatar.expressionsMenu;

        protected static void ToggleAlwaysAdvanced()
        {
            ButtonWizardPref.AlwaysAdvanced = !ButtonWizardPref.AlwaysAdvanced;
        }

        protected void ShowLanguageOption()
        {
            var selectedLanguage = ButtonWizardPref.Language;

            EditorGUI.BeginChangeCheck();
            selectedLanguage = (SupportedLanguage) EditorGUILayout.EnumPopup(
                Localized.buttonWizardSettingLabelLanguage, selectedLanguage);
            if (EditorGUI.EndChangeCheck())
            {
                LocalizedTextDataset.SetLanguage(selectedLanguage);
                ButtonWizardPref.Language = selectedLanguage;
            }
        }

        protected void LoadFolderPath()
        {
            folderPath = ButtonWizardPref.SavePath;
            if (debug) Debug.Log($"Save Path loaded: {folderPath}");
        }

        protected void SaveFolderPath()
        {
            ButtonWizardPref.SavePath = folderPath;
            if (debug) Debug.Log($"Save Path saved: {folderPath}");
        }

        protected void ShowAvatarField(Action onChange = null)
        {
            var label = new GUIContent
            {
                text = Localized.baseWindowLabelAvatar,
                tooltip = Localized.baseWindowTooltipAvatar
            };

            EditorGUI.BeginChangeCheck();
            avatar = EditorGUILayout.ObjectField(label, avatar,
                typeof(VRCAvatarDescriptor), true) as VRCAvatarDescriptor;
            if (EditorGUI.EndChangeCheck() && avatar != null)
            {
                SetupNewAvatarSetting(avatar);
                onChange?.Invoke();
            }
        }

        protected void ShowSaveLocation(Action onChange = null)
        {
            var label = new GUIContent
            {
                text = Localized.advancedButtonWizardWindowLabelSaveLocation,
                tooltip = Localized.advancedButtonWizardWindowLabelTooltipSaveLocation
            };

            EditorGUI.BeginChangeCheck();
            folderPath = PtEditorGUILayout.AssetPathPopup(label, folderPath);
            if (EditorGUI.EndChangeCheck())
            {
                onChange?.Invoke();
                Repaint();
            }
        }

        private void SetupNewAvatarSetting(VRCAvatarDescriptor avatarDesc)
        {
            if (targetAnimatorController == null)
            {
                targetAnimatorController = GetAnimatorController(avatarDesc);
            }

            targetProperties.Clear();
            vrcParameters = avatarDesc.expressionParameters;
            vrcTargetMenu = avatarDesc.expressionsMenu;
        }

        private static AnimatorController GetAnimatorController(VRCAvatarDescriptor avatar)
        {
            return avatar.baseAnimationLayers[4].animatorController as AnimatorController;
        }

        protected void ShowAnimatorField()
        {
            var label = new GUIContent
            {
                text = Localized.baseWindowLabelAnimator,
                tooltip = Localized.baseWindowTooltipAnimator
            };

            targetAnimatorController = EditorGUILayout.ObjectField(label, targetAnimatorController,
                typeof(AnimatorController), true) as AnimatorController;
        }

        protected void ShowVrcTargetMenuField()
        {
            var label = new GUIContent
            {
                text = Localized.baseWindowLabelVrcTargetMenu,
                tooltip = Localized.baseWindowTooltipVrcTargetMenu
            };

            vrcTargetMenu = PtEditorGUILayout.VrcMenuPopup(label, VrcRootMenu, vrcTargetMenu);
        }

        protected void ShowVrcParameterField()
        {
            var label = new GUIContent
            {
                text = Localized.baseWindowLabelVrcParameters,
                tooltip = Localized.baseWindowTooltipVrcParameters
            };

            vrcParameters = EditorGUILayout.ObjectField(label, vrcParameters,
                typeof(VRCExpressionParameters), false) as VRCExpressionParameters;
        }

        protected void ShowTargetObjectsField(Action onChange = null)
        {
            EditorGUI.BeginChangeCheck();

            var label = new GUIContent
            {
                text = Localized.baseWindowLabelTargetObject,
                tooltip = Localized.baseWindowTooltipTargetObject
            };

            EditorGUILayout.BeginVertical(ButtonWizardStyles.MultipleFields, GUILayout.MinHeight(100));
            EditorGUILayout.LabelField(label);

            if (targetProperties.Count == 0)
            {
                GUILayout.Label("[Drop Objects Here]", ButtonWizardStyles.LabelCenter, GUILayout.MinHeight(100));
            }
            else
            {
                ShowTargetProperties();
            }

            EditorGUILayout.EndVertical();

            var gobjs = PtEditorGUILayout.CheckDragAndDrop<GameObject>();
            if (gobjs.Count > 0)
            {
                foreach (var gobj in gobjs)
                {
                    TryToAddTargetProperty(gobj);
                }
            }

            if (EditorGUI.EndChangeCheck() && targetAnimatorController != null)
            {
                if (targetProperties.Count > 0)
                    SetupTargetObjectSetting(targetProperties[0]);
                onChange?.Invoke();
            }
        }

        private void TryToAddTargetProperty(GameObject gobj)
        {
            var property = new ToggleProperty(gobj, avatar.gameObject);
            targetProperties.Add(property);
            GUI.changed = true;
        }

        private void ShowTargetProperties()
        {
            for (int i = 0; i < targetProperties.Count; i++)
            {
                targetProperties[i] = ShowTargetProperty(targetProperties[i], out bool isMinusClicked);
                if (isMinusClicked)
                {
                    targetProperties[i] = null;
                    GUI.changed = true;
                }
            }

            targetProperties.RemoveAll(x => x == null);
        }

        private ToggleProperty ShowTargetProperty(ToggleProperty target, out bool isMinusClicked)
        {
            EditorGUILayout.BeginHorizontal();
            target = AnimatedPropertyField(target);
            isMinusClicked = GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"), GUILayout.Width(30));
            EditorGUILayout.EndHorizontal();
            return target;
        }

        protected virtual ToggleProperty AnimatedPropertyField(ToggleProperty target)
        {
            EditorGUILayout.ObjectField(target.gameObject, typeof(GameObject), true);
            return target;
        }

        private void SetupTargetObjectSetting(ToggleProperty target)
        {
            if (target == null)
                return;

            if (string.IsNullOrWhiteSpace(menuName))
            {
                menuName = target.binding.path;
            }

            if (string.IsNullOrWhiteSpace(parameterName))
            {
                parameterName = target.binding.path;
            }
        }

        protected void ShowMenuNameField()
        {
            var label = new GUIContent
            {
                text = Localized.baseWindowLabelMenuName,
                tooltip = Localized.baseWindowTooltipMenuName
            };

            menuName = EditorGUILayout.TextField(label, menuName);
        }

        protected void ShowParameterNameField()
        {
            var label = new GUIContent
            {
                text = Localized.baseWindowLabelParameterName,
                tooltip = Localized.baseWindowTooltipParameterName
            };

            parameterName = EditorGUILayout.TextField(label, parameterName);
        }

        protected void ShowParameterSaveField()
        {
            var label = new GUIContent
            {
                text = Localized.baseWindowLabelSaveParameter,
                tooltip = Localized.baseWindowTooltipSaveParameter
            };

            isParamSaved = EditorGUILayout.Toggle(label, isParamSaved);
        }

        protected void ShowParameterDefaultField()
        {
            var label = new GUIContent
            {
                text = Localized.baseWindowLabelDefaultValue,
                tooltip = Localized.baseWindowTooltipDefaultValue
            };

            defaultBool = EditorGUILayout.Toggle(label, defaultBool);
        }

        protected void CreateToggleClipsOnly(string toggleMenuName, string toggleParameterName)
        {
            CreateFolderIfNotExist(folderPath);

            var properties = targetProperties.ToArray();

            AnimationClipUtil.ToggleCreate(folderPath, properties, toggleParameterName, true);
            AnimationClipUtil.ToggleCreate(folderPath, properties, toggleParameterName, false);
        }

        protected void CreateToggle(string toggleMenuName, string toggleParameterName)
        {
            if (targetAnimatorController == null)
            {
                Debug.LogAssertion(new NotImplementedException());
                return;
            }

            CreateFolderIfNotExist(folderPath);

            var properties = targetProperties.ToArray();

            var assetPath = AssetDatabase.GetAssetPath(targetAnimatorController);
            AnimationClip clipOn = AnimationClipUtil.ToggleCreate(folderPath, properties, toggleParameterName, true);
            AnimationClip clipOff = AnimationClipUtil.ToggleCreate(folderPath, properties, toggleParameterName, false);
            var stateOn = AnimatorStateUtil.ToggleCreate(assetPath, clipOn, true);
            var stateOff = AnimatorStateUtil.ToggleCreate(assetPath, clipOff, false);
            var stateMachine = AnimatorStateMachineUtil.ToggleCreate(assetPath, stateOn, stateOff, toggleParameterName);
            AnimatorStateUtil.ToggleLink(assetPath, stateOn, stateOff, parameterName);
            var toggleLayer = CreateToggleLayer(stateMachine, toggleMenuName);

            targetAnimatorController.AddLayer(toggleLayer);
            targetAnimatorController.TryAddParameter(CreateToggleParameters(toggleParameterName));

            EditorUtility.SetDirty(targetAnimatorController);
        }

        private AnimatorControllerLayer CreateToggleLayer(AnimatorStateMachine stateMachine, string toggleMenuName)
        {
            return new AnimatorControllerLayer
            {
                name = toggleMenuName,
                stateMachine = stateMachine,
                blendingMode = AnimatorLayerBlendingMode.Override,
                defaultWeight = 1,
            };
        }

        private AnimatorControllerParameter CreateToggleParameters(string toggleParameterName)
        {
            return new AnimatorControllerParameter
            {
                name = toggleParameterName,
                type = AnimatorControllerParameterType.Bool,
                defaultBool = false
            };
        }

        protected void CreateVrcToggle(string toggleMenuName, string toggleParameterName, bool isSaved,
            bool defaultValue)
        {
            if (vrcParameters == null)
            {
                Debug.LogAssertion(new NotImplementedException());
            }
            else
            {
                vrcParameters.AddToggle(toggleParameterName, isSaved, defaultValue);
                EditorUtility.SetDirty(vrcParameters);
            }

            if (vrcTargetMenu == null)
            {
                Debug.LogAssertion(new NotImplementedException());
            }
            else
            {
                vrcTargetMenu.AddToggle(toggleMenuName, toggleParameterName);
                EditorUtility.SetDirty(vrcTargetMenu);
            }
        }

        protected static void CreateFolderIfNotExist(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            var parentFolderPath = Path.GetDirectoryName(path);
            if (parentFolderPath == "Asset")
                return;

            if (!string.IsNullOrEmpty(parentFolderPath))
            {
                CreateFolderIfNotExist(parentFolderPath);
            }

            var folderName = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parentFolderPath, folderName);
        }

        protected static void ShowFooter()
        {
            EditorGUILayout.BeginHorizontal();
            PtEditorGUILayout.CreateLink("GitHub",
                "https://github.com/puetsua/VRCButtonWizard",
                "Favorite");
            GUILayout.Label(ButtonWizardConst.Version, ButtonWizardStyles.LabelRight);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            PtEditorGUILayout.CreateLink("Twitter",
                "https://twitter.com/puetsua",
                "Favorite");
            GUILayout.Label("Pue-Tsuâ Workshop", ButtonWizardStyles.LabelRight);
            EditorGUILayout.EndHorizontal();
        }
    }
}