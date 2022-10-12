﻿using System.IO;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Puetsua.VRCButtonWizard.Editor
{
    internal static class PtEditorGUILayout
    {
        internal static string AssetPathPopup(string label, string path)
        {
            if (string.IsNullOrEmpty(path) || !path.StartsWith("Assets"))
                path = "Assets";

            var folderName = Path.GetFileName(path);
            var style = new GUIStyle("MiniPullDown");
            if (folderName == "Assets")
            {
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

        internal static VRCExpressionsMenu VrcMenuPopup(string label, VRCExpressionsMenu rootMenu,
            VRCExpressionsMenu menu)
        {
            var style = new GUIStyle("MiniPullDown");
            if (menu == null)
            {
                menu = rootMenu;
            }

            var content = EditorGUIUtility.IconContent("Folder Icon");
            content.text = menu.name;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            var rect = EditorGUILayout.GetControlRect(true, 18f, style);
            if (EditorGUI.DropdownButton(rect, content, FocusType.Keyboard, style))
            {
                VrcMenuPopupWindow.lastSelectedMenu = menu;
                PopupWindow.Show(rect, new VrcMenuPopupWindow(rect.width, rootMenu));
            }

            EditorGUILayout.EndHorizontal();

            if (VrcMenuPopupWindow.lastSelectedMenu != null &&
                VrcMenuPopupWindow.lastSelectedMenu != menu)
            {
                menu = VrcMenuPopupWindow.lastSelectedMenu;
                GUI.changed = true;
            }

            return menu;
        }
    }
}