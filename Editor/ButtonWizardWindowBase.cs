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
        protected Animator targetAnimator;
        protected AnimatorController targetAnimatorController;
        protected List<ToggleProperty> targetProperties = new List<ToggleProperty>();
        protected VRCExpressionParameters vrcParameters;
        protected VRCExpressionsMenu vrcTargetMenu;
        protected string menuName;
        protected string parameterName;
        protected string folderPath;
        protected bool isParamSaved;
        protected bool defaultBool;
        protected bool invertToggle;

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

        protected void ShowAnimatorField(Action onChange = null)
        {
            var label = new GUIContent
            {
                text = Localized.baseWindowLabelAnimator,
                tooltip = Localized.baseWindowTooltipAnimator
            };

            EditorGUI.BeginChangeCheck();
            targetAnimator = EditorGUILayout.ObjectField(label, targetAnimator,
                typeof(Animator), true) as Animator;
            if (EditorGUI.EndChangeCheck())
            {
                if (targetAnimator != null && targetAnimatorController == null)
                {
                    targetAnimatorController = targetAnimator.runtimeAnimatorController as AnimatorController;
                }
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
            if (targetAnimator == null)
            {
                targetAnimator = avatarDesc.GetComponent<Animator>();
            }

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

        protected void ShowAnimatorControllerField()
        {
            var label = new GUIContent
            {
                text = Localized.baseWindowLabelAnimatorController,
                tooltip = Localized.baseWindowTooltipAnimatorController
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

            if (VrcRootMenu == null)
            {
                vrcTargetMenu = EditorGUILayout.ObjectField(label, vrcTargetMenu,
                    typeof(VRCExpressionsMenu), false) as VRCExpressionsMenu;
            }
            else
            {
                GUILayout.BeginVertical(ButtonWizardStyles.MultipleFields);

                vrcTargetMenu = PtEditorGUILayout.VrcMenuPopup(label, VrcRootMenu, vrcTargetMenu);
                vrcTargetMenu = EditorGUILayout.ObjectField(" ", vrcTargetMenu,
                    typeof(VRCExpressionsMenu), false) as VRCExpressionsMenu;

                GUILayout.EndVertical();
            }
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
            var label = new GUIContent
            {
                text = Localized.baseWindowLabelTargetObject,
                tooltip = Localized.baseWindowTooltipTargetObject
            };

            EditorGUILayout.BeginVertical(ButtonWizardStyles.MultipleFields, GUILayout.MinHeight(100));
            EditorGUILayout.LabelField(label);

            EditorGUI.BeginChangeCheck();

            if (targetAnimator == null)
            {
                GUILayout.Label(Localized.baseWindowTipAnimatorRequired, ButtonWizardStyles.LabelCenter,
                    GUILayout.MinHeight(100));
            }
            else
            {
                int previousPropertyCount = targetProperties.Count;
                if (targetProperties.Count == 0)
                {
                    GUILayout.Label(Localized.baseWindowTipDropObjectsHere, ButtonWizardStyles.LabelCenter,
                        GUILayout.MinHeight(100));
                }
                else
                {
                    ShowTargetProperties();
                }

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
                    // Automatically fill menu name and parameter name if first object is set
                    bool isFirstPropertyAdded = previousPropertyCount == 0 && targetProperties.Count > 0;
                    if (isFirstPropertyAdded)
                        SetupTargetObjectSetting(targetProperties[0]);
                    onChange?.Invoke();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void TryToAddTargetProperty(GameObject gobj)
        {
            var property = new ToggleProperty(gobj, targetAnimator.gameObject);
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
                menuName = target.gameObject.name;
            }

            if (string.IsNullOrWhiteSpace(parameterName))
            {
                parameterName = target.gameObject.name;
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

        protected void ShowInverseToggleField()
        {
            var label = new GUIContent
            {
                text = Localized.baseWindowLabelInvertToggle,
                tooltip = Localized.baseWindowTooltipInverseToggle
            };

            invertToggle = EditorGUILayout.Toggle(label, invertToggle);
        }

        protected void CreateToggleClipsOnly(string toggleMenuName, string toggleParameterName)
        {
            CreateFolderIfNotExist(folderPath);

            var properties = targetProperties.ToArray();

            AnimationClipUtil.ToggleCreate(folderPath, properties, toggleParameterName, true);
            AnimationClipUtil.ToggleCreate(folderPath, properties, toggleParameterName, false);
            
            Debug.Log("Toggle clips created successfully.");
        }

        protected void CreateToggleAnimatorLayerOnly(string toggleMenuName, string toggleParameterName)
        {
            if (targetAnimatorController == null)
            {
                Debug.LogAssertion(new NotImplementedException("Must assign an animator controller."));
                return;
            }

            CreateFolderIfNotExist(folderPath);

            var assetPath = AssetDatabase.GetAssetPath(targetAnimatorController);
            var isOff = invertToggle;
            var isOn = !invertToggle;
            var stateOff = AnimatorStateUtil.ToggleCreate(assetPath, null, isOff);
            var stateOn = AnimatorStateUtil.ToggleCreate(assetPath, null, isOn);
            var stateMachine = AnimatorStateMachineUtil.ToggleCreate(assetPath, stateOff, stateOn, toggleParameterName);
            AnimatorStateUtil.ToggleLink(assetPath, stateOff, stateOn, parameterName);
            var toggleLayer = CreateToggleLayer(stateMachine, toggleMenuName);

            targetAnimatorController.AddLayer(toggleLayer);
            targetAnimatorController.TryAddParameter(CreateToggleParameters(toggleParameterName));

            EditorUtility.SetDirty(targetAnimatorController);
            
            Debug.Log("Toggle Animator Layer created successfully.");
        }

        protected void CreateToggle(string toggleMenuName, string toggleParameterName)
        {
            if (targetAnimatorController == null)
            {
                Debug.LogAssertion(new NotImplementedException("Must assign an animator controller."));
                return;
            }

            CreateFolderIfNotExist(folderPath);

            var properties = targetProperties.ToArray();

            var assetPath = AssetDatabase.GetAssetPath(targetAnimatorController);
            var isOff = invertToggle;
            var isOn = !invertToggle;
            AnimationClip clipOff = AnimationClipUtil.ToggleCreate(folderPath, properties, toggleParameterName, isOff);
            AnimationClip clipOn = AnimationClipUtil.ToggleCreate(folderPath, properties, toggleParameterName, isOn);
            var stateOff = AnimatorStateUtil.ToggleCreate(assetPath, clipOff, isOff);
            var stateOn = AnimatorStateUtil.ToggleCreate(assetPath, clipOn, isOn);
            var stateMachine = AnimatorStateMachineUtil.ToggleCreate(assetPath, stateOff, stateOn, toggleParameterName);
            AnimatorStateUtil.ToggleLink(assetPath, stateOff, stateOn, parameterName);
            var toggleLayer = CreateToggleLayer(stateMachine, toggleMenuName);

            targetAnimatorController.AddLayer(toggleLayer);
            targetAnimatorController.TryAddParameter(CreateToggleParameters(toggleParameterName));

            EditorUtility.SetDirty(targetAnimatorController);
            
            Debug.Log("Toggle animator layer and clips created successfully.");
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
            
            Debug.Log("VRC Toggle created successfully.");
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