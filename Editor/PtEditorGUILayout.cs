using System.IO;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Puetsua.VRCButtonWizard.Editor
{
    internal static class PtEditorGUILayout
    {
        internal static string AssetPathPopup(string label, string path)
        {
            var folderName = Path.GetFileName(path);
            var style = new GUIStyle("MiniPullDown");
            if (string.IsNullOrEmpty(path))
            {
                folderName = "[Assets]";
                style.fontStyle = FontStyle.Italic;
            }

            var folderContent = EditorGUIUtility.IconContent("Folder Icon");
            folderContent.text = folderName;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            var rect = EditorGUILayout.GetControlRect(true, 18f, style);
            if (EditorGUI.DropdownButton(rect, folderContent, FocusType.Keyboard, style))
            {
                AssetPathPopupWindow.lastSelectedPath = path;
                PopupWindow.Show(rect, new AssetPathPopupWindow(rect.width));
            }

            EditorGUILayout.EndHorizontal();

            if (AssetPathPopupWindow.lastSelectedPath != null &&
                AssetPathPopupWindow.lastSelectedPath != path)
            {
                path = AssetPathPopupWindow.lastSelectedPath;
                GUI.changed = true;
            }

            return path;
        }
    }
}