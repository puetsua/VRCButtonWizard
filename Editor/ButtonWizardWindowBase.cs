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
        protected List<GameObject> targetObjects = new List<GameObject>();
        protected VRCExpressionParameters vrcParameters;
        protected VRCExpressionsMenu vrcTargetMenu;
        protected string menuName;
        protected string parameterName;
        protected string folderPath;
        protected bool isParamSaved;
        protected bool defaultBool;

        protected VRCExpressionsMenu VrcRootMenu => avatar == null ? null : avatar.expressionsMenu;

        protected void ShowLanguageOption()
        {
            var selectedLanguage = (SupportedLanguage) EditorPrefs.GetInt(
                EditorPrefConst.Language, (int) SupportedLanguage.English);

            EditorGUI.BeginChangeCheck();
            selectedLanguage = (SupportedLanguage) EditorGUILayout.EnumPopup(
                Localized.buttonWizardSettingLabelLanguage, selectedLanguage);
            if (EditorGUI.EndChangeCheck())
            {
                LocalizedTextDataset.SetLanguage(selectedLanguage);
                EditorPrefs.SetInt(EditorPrefConst.Language, (int) selectedLanguage);
            }
        }

        protected void LoadFolderPath()
        {
            folderPath = EditorPrefs.GetString(EditorPrefConst.SavePath);
            if (debug) Debug.Log($"Save Path loaded: {folderPath}");
        }

        protected void SaveFolderPath()
        {
            EditorPrefs.SetString(EditorPrefConst.SavePath, folderPath);
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

            targetObjects.Clear();
            vrcParameters = avatarDesc.expressionParameters;
            vrcTargetMenu = avatarDesc.expressionsMenu;
        }

        private static AnimatorController GetAnimatorController(VRCAvatarDescriptor avatar)
        {
            return avatar.baseAnimationLayers[4].animatorController as AnimatorController;
        }

        protected void ShowCreateToggleButton()
        {
            bool areTargetObjectValid = true;
            if (targetObjects.Count > 0)
            {
                if (!AreTargetObjectsHierarchyValid())
                {
                    EditorGUILayout.HelpBox(Localized.baseWindowMsgObjectNotUnderAvatar, MessageType.Error);
                    areTargetObjectValid = false;
                }
            }
            else
            {
                areTargetObjectValid = false;
            }

            GUI.enabled = !string.IsNullOrWhiteSpace(menuName) &&
                          !string.IsNullOrWhiteSpace(parameterName) &&
                          areTargetObjectValid;
            if (GUILayout.Button("Create Toggle"))
            {
                CreateVrcToggle(menuName, parameterName, isParamSaved, defaultBool);
                CreateToggle(menuName, parameterName);
                AssetDatabase.SaveAssets();
            }

            GUI.enabled = true;
        }

        private bool AreTargetObjectsHierarchyValid()
        {
            foreach (var targetObject in targetObjects)
            {
                if (targetObject == null)
                    continue;
                if (!targetObject.transform.IsChildOf(avatar.transform))
                {
                    return false;
                }
            }

            return true;
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

            EditorGUILayout.BeginHorizontal(ButtonWizardStyles.MultipleFields);

            var label = new GUIContent
            {
                text = Localized.baseWindowLabelTargetObject,
                tooltip = Localized.baseWindowTooltipTargetObject
            };

            EditorGUILayout.PrefixLabel(label);
            EditorGUILayout.BeginVertical(GUILayout.MinHeight(100));

            if (targetObjects.Count == 0)
            {
                EditorGUILayout.LabelField("[Drop Objects Here]",GUILayout.MinHeight(100));
            }
            else
            {
                ShowTargetObjects();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            var gobjs = PtEditorGUILayout.CheckDragAndDrop<GameObject>();
            if (gobjs.Count > 0)
            {
                foreach (var gobj in gobjs)
                {
                    if (!targetObjects.Contains(gobj))
                    {
                        targetObjects.Add(gobj);
                        GUI.changed = true;
                    }
                }
            }

            if (EditorGUI.EndChangeCheck() && targetAnimatorController != null)
            {
                if (targetObjects.Count > 0)
                    SetupTargetObjectSetting(targetObjects[0]);
                onChange?.Invoke();
            }
        }

        private void ShowTargetObjects()
        {
            for (int i = 0; i < targetObjects.Count; i++)
            {
                targetObjects[i] = ShowTargetObject(targetObjects[i], out bool isMinusClicked);
                if (isMinusClicked)
                {
                    targetObjects[i] = null;
                    GUI.changed = true;
                }
            }

            targetObjects.RemoveAll(x => x == null);
        }

        private GameObject ShowTargetObject(GameObject target, out bool isMinusClicked)
        {
            EditorGUILayout.BeginHorizontal();
            target = EditorGUILayout.ObjectField(target, typeof(GameObject), true) as GameObject;
            isMinusClicked = GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"), GUILayout.Width(30));
            EditorGUILayout.EndHorizontal();
            return target;
        }

        private void SetupTargetObjectSetting(GameObject target)
        {
            if (target == null)
                return;

            if (string.IsNullOrWhiteSpace(menuName))
            {
                menuName = target.name;
            }

            if (string.IsNullOrWhiteSpace(parameterName))
            {
                parameterName = target.name;
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

        private void CreateToggle(string toggleMenuName, string toggleParameterName)
        {
            if (targetAnimatorController == null)
            {
                Debug.LogAssertion(new NotImplementedException());
                return;
            }

            CreateFolderIfNotExist(folderPath);

            var paths = new string[targetObjects.Count];
            for (int i = 0; i < targetObjects.Count; i++)
            {
                paths[i] = targetObjects[i].transform.GetHierarchyPath(avatar.transform);
            }

            var assetPath = AssetDatabase.GetAssetPath(targetAnimatorController);
            AnimationClip clipOn = AnimationClipUtil.ToggleCreate(folderPath, paths, toggleParameterName, true);
            AnimationClip clipOff = AnimationClipUtil.ToggleCreate(folderPath, paths, toggleParameterName, false);
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

        private void CreateVrcToggle(string toggleMenuName, string toggleParameterName, bool isSaved, bool defaultValue)
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