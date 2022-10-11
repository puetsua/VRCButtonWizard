using System.IO;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Puetsua.VRCButtonWizard.Editor
{
    public class ButtonWizardWindow : ButtonWizardWindowBase
    {
        [MenuItem("Tools/VRChat Button Wizard")]
        private static void OpenWindow()
        {
            EditorWindow wnd = GetWindow<ButtonWizardWindow>();
            wnd.titleContent = new GUIContent("Button Wizard");
            wnd.position = WindowPos;
            wnd.minSize = MinWindowSize;
            wnd.maxSize = MaxWindowSize;
        }

        private void OnGUI()
        {
            GUILayout.Label("VRChat Button Wizard", EditorStyles.boldLabel);
            ShowAvatarField(OnAvatarChanged);
            if (avatar != null)
            {
                ShowTargetObjectField();
                ShowMenuNameField();
                ShowParameterSaveField();
                ShowParameterDefaultField();
                ShowCreateToggleButton();
            }
        }

        private void OnAvatarChanged()
        {
            SetFolderPath();
        }

        private void SetFolderPath()
        {
            var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(avatar);
            if (string.IsNullOrEmpty(path))
            {
                path = avatar.gameObject.scene.path;
            }

            path = Path.GetDirectoryName(path) ?? "";
            path = Path.Combine(path, "Animations");

            Debug.Log($"{path} {avatar.gameObject.scene.path}");
        }
    }
}