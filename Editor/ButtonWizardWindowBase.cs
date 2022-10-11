using System;
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
        public static bool debug = true;
        protected static readonly Rect WindowPos = new Rect(400, 400, 600, 400);
        protected static readonly Vector2 MinWindowSize = new Vector2(450, 200);
        protected static readonly Vector2 MaxWindowSize = new Vector2(1280, 720);

        protected VRCAvatarDescriptor avatar;
        protected AnimatorController targetAnimatorController;
        protected GameObject targetObject;
        protected VRCExpressionParameters vrcParameters;
        protected VRCExpressionsMenu vrcTargetMenu;
        protected string menuName;
        protected string parameterName;
        protected string folderPath;
        protected bool isParamSaved;
        protected bool defaultBool;

        protected VRCExpressionsMenu VrcRootMenu => avatar == null ? null : avatar.expressionsMenu;
        protected string AssetPath => string.IsNullOrEmpty(folderPath) ? "Assets" : $"Assets/{folderPath}";

        protected void LoadFolderPath()
        {
            folderPath = EditorPrefs.GetString(EditorPrefConst.SavePath);
            if (debug) Debug.Log("Save Path loaded.");
        }

        protected void SaveFolderPath()
        {
            EditorPrefs.SetString(EditorPrefConst.SavePath, folderPath);
            if (debug) Debug.Log("Save Path saved.");
        }

        protected void ShowAvatarField(Action onChange = null)
        {
            EditorGUI.BeginChangeCheck();
            avatar = EditorGUILayout.ObjectField("Avatar", avatar,
                typeof(VRCAvatarDescriptor), true) as VRCAvatarDescriptor;
            if (EditorGUI.EndChangeCheck() && avatar != null)
            {
                SetupNewAvatarSetting(avatar);
                onChange?.Invoke();
            }
        }

        protected void ShowSaveLocation(Action onChange = null)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            folderPath = PtEditorGUILayout.AssetPathPopup("Save Location", folderPath);
            EditorGUILayout.EndHorizontal();
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

            vrcParameters = avatarDesc.expressionParameters;
            vrcTargetMenu = avatarDesc.expressionsMenu;

            // TODO: when _vrcParameters null
            // TODO: when _vrcTargetMenu null
        }

        private static AnimatorController GetAnimatorController(VRCAvatarDescriptor avatar)
        {
            return avatar.baseAnimationLayers[4].animatorController as AnimatorController;
        }

        protected void ShowCreateToggleButton()
        {
            if (targetObject != null)
            {
                if (!targetObject.transform.IsChildOf(avatar.transform))
                {
                    EditorGUILayout.HelpBox("Target Object is not under avatar.", MessageType.Error);
                    return;
                }
            }

            GUI.enabled = !string.IsNullOrWhiteSpace(menuName) &&
                          !string.IsNullOrWhiteSpace(parameterName) &&
                          targetObject != null;
            if (GUILayout.Button("Create Toggle"))
            {
                CreateVrcToggle(menuName, parameterName, isParamSaved, defaultBool);
                CreateToggle(menuName, parameterName);
            }

            GUI.enabled = true;
        }

        protected void ShowAnimatorField()
        {
            targetAnimatorController = EditorGUILayout.ObjectField("Animator", targetAnimatorController,
                typeof(AnimatorController), true) as AnimatorController;
        }

        protected void ShowVrcTargetMenuField()
        {
            vrcTargetMenu = EditorGUILayout.ObjectField("Target Menu", vrcTargetMenu,
                typeof(VRCExpressionsMenu), false) as VRCExpressionsMenu;
        }

        protected void ShowVrcParameterField()
        {
            vrcParameters = EditorGUILayout.ObjectField("Parameters", vrcParameters,
                typeof(VRCExpressionParameters), false) as VRCExpressionParameters;
        }

        protected void ShowTargetObjectField(Action onChange = null)
        {
            EditorGUI.BeginChangeCheck();
            targetObject = EditorGUILayout.ObjectField("Toggle Object", targetObject,
                typeof(GameObject), true) as GameObject;
            if (EditorGUI.EndChangeCheck() && targetAnimatorController != null)
            {
                SetupTargetObjectSetting(targetObject);
                onChange?.Invoke();
            }
        }

        private void SetupTargetObjectSetting(GameObject target)
        {
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
            menuName = EditorGUILayout.TextField("Menu Name", menuName);
        }

        protected void ShowParameterNameField()
        {
            parameterName = EditorGUILayout.TextField("Parameter Name", parameterName);
        }

        protected void ShowParameterSaveField()
        {
            isParamSaved = EditorGUILayout.Toggle("Save Parameter", isParamSaved);
        }

        protected void ShowParameterDefaultField()
        {
            defaultBool = EditorGUILayout.Toggle("Default Value", defaultBool);
        }

        private void CreateToggle(string toggleMenuName, string toggleParameterName)
        {
            CreateFolderIfNotExist(AssetPath);

            string path = targetObject.transform.GetHierarchyPath(avatar.transform);
            AnimationClip clipOn = AnimationClipUtil.ToggleCreate(AssetPath, path, toggleParameterName, true);
            AnimationClip clipOff = AnimationClipUtil.ToggleCreate(AssetPath, path, toggleParameterName, false);
            var toggleLayer = new AnimatorControllerLayer
            {
                name = toggleMenuName,
                stateMachine = AnimatorStateMachineUtil.ToggleCreate(clipOn, clipOff,
                    toggleParameterName, toggleParameterName),
                blendingMode = AnimatorLayerBlendingMode.Override,
                defaultWeight = 1,
            };
            
            targetAnimatorController.AddLayer(toggleLayer);
            targetAnimatorController.TryAddParameter(new AnimatorControllerParameter
            {
                name = toggleParameterName,
                type = AnimatorControllerParameterType.Bool,
                defaultBool = false
            });
        }

        private void CreateVrcToggle(string toggleMenuName, string toggleParameterName, bool isSaved, bool defaultValue)
        {
            vrcParameters.AddToggle(toggleParameterName, isSaved, defaultValue);
            vrcTargetMenu.AddToggle(toggleMenuName, toggleParameterName);
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
    }
}