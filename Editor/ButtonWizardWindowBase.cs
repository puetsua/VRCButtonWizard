using System;
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

        protected bool targetObjectSettingReady;

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

        protected void ShowAvatarOptions()
        {
            targetAnimatorController = EditorGUILayout.ObjectField("Animator", targetAnimatorController,
                typeof(AnimatorController), true) as AnimatorController;
            targetObject = EditorGUILayout.ObjectField("Toggle Object", targetObject,
                typeof(GameObject), true) as GameObject;
            menuName = EditorGUILayout.TextField("Menu Name", menuName);
            parameterName = EditorGUILayout.TextField("Parameter Name", parameterName);
            isParamSaved = EditorGUILayout.Toggle("Save Parameter", isParamSaved);
            defaultBool = EditorGUILayout.Toggle("Default Value", defaultBool);

            if (targetObject != null)
            {
                if (!targetObject.transform.IsChildOf(avatar.transform))
                {
                    EditorGUILayout.HelpBox("Target Object is not under avatar.", MessageType.Error);
                    return;
                }

                if (!targetObjectSettingReady)
                {
                    SetupTargetObjectSetting(targetObject);
                    targetObjectSettingReady = true;
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
            
        }
        
        protected void ShowTargetObjectField()
        {
            
        }
        
        protected void ShowMenuNameField()
                {
                    
                }
        
        protected void ShowParameterNameField()
        {
                    
        }
        
        protected void ShowParameterSaveField()
        {
                    
        }
        
        protected void ShowParameterDefaultField()
        {
                    
        }
        
        private void SetupTargetObjectSetting(GameObject targetObject)
        {
            if (string.IsNullOrWhiteSpace(menuName))
            {
                menuName = targetObject.name;
            }

            if (string.IsNullOrWhiteSpace(parameterName))
            {
                parameterName = targetObject.name;
            }
        }

        private void CreateToggle(string menuToggleName, string parameterName)
        {
            string path = targetObject.transform.GetHierarchyPath(avatar.transform);
            AnimationClip clipOn = AnimationClipUtil.ToggleCreate(folderPath, path, parameterName, true);
            AnimationClip clipOff = AnimationClipUtil.ToggleCreate(folderPath, path, parameterName, false);
            var toggleLayer = new AnimatorControllerLayer
            {
                name = menuToggleName,
                stateMachine = AnimatorStateMachineUtil.ToggleCreate(clipOn, clipOff,
                    parameterName, parameterName),
                blendingMode = AnimatorLayerBlendingMode.Override,
                defaultWeight = 1,
            };

            targetAnimatorController.AddLayer(toggleLayer);
            targetAnimatorController.TryAddParameter(new AnimatorControllerParameter
            {
                name = parameterName,
                type = AnimatorControllerParameterType.Bool,
                defaultBool = false
            });
        }

        private void CreateVrcToggle(string menuName, string parameterName, bool isSaved, bool defaultBool)
        {
            vrcParameters.AddToggle(parameterName, isSaved, defaultBool);
            vrcTargetMenu.AddToggle(menuName, parameterName);
        }
    }
}