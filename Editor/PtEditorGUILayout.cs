using System.Collections.Generic;
using System.IO;
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
            return AssetPathPopup(new GUIContent(label), path);
        }

        internal static string AssetPathPopup(GUIContent label, string path)
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
                AssetPathPopupWindow.lastSelectedPath = null;
                PopupWindow.Show(rect, new AssetPathPopupWindow(rect.width, path));
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

        internal static string VrcMenuPopup(string label, string path)
        {
            return AssetPathPopup(new GUIContent(label), path);
        }

        internal static VRCExpressionsMenu VrcMenuPopup(GUIContent label, VRCExpressionsMenu rootMenu,
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
                VrcMenuPopupWindow.lastSelectedMenu = null;
                PopupWindow.Show(rect, new VrcMenuPopupWindow(rect.width, rootMenu, menu));
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

        internal static void CreateLink(string text, string url, string iconName)
        {
            var labelTwitter = EditorGUIUtility.IconContent(iconName);
            labelTwitter.text = text;
            if (GUILayout.Button(labelTwitter, EditorStyles.linkLabel))
            {
                Application.OpenURL(url);
            }
        }

        internal static List<T> CheckDragAndDrop<T>()
            where T : Object
        {
            return CheckDragAndDrop<T>(GUILayoutUtility.GetLastRect());
        }
        
        internal static List<T> CheckDragAndDrop<T>(Rect area)
            where T : Object
        {
            Event evt = Event.current;
            var list = new List<T>();

            if (evt.type == EventType.DragUpdated && 
                area.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
            else if (evt.type == EventType.DragPerform && 
                     area.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                DragAndDrop.AcceptDrag();
                foreach (var obj in DragAndDrop.objectReferences)
                {
                    if (obj is T tObj) list.Add(tObj);
                }
            }

            return list;
        }
    }
}