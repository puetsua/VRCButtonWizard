using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Puetsua.VRCButtonWizard.Editor
{
    public class VrcMenuPopupWindow : PopupWindowContent
    {
        public static VRCExpressionsMenu lastSelectedMenu;

        private readonly float _windowWidth;

        private bool _assetFoldout;
        private VrcMenuTreeView _treeView;
        private TreeViewState _treeViewState;
        private SearchField _searchField;
        private VRCExpressionsMenu _rootMenu;

        public VrcMenuPopupWindow(float windowWidth, VRCExpressionsMenu rootMenu)
        {
            _rootMenu = rootMenu;
            _windowWidth = windowWidth;
        }

        public override void OnOpen()
        {
            if (_treeViewState == null)
                _treeViewState = new TreeViewState();

            _treeView = new VrcMenuTreeView(_treeViewState, _rootMenu, OnItemDoubleClicked);
            _searchField = new SearchField();
            _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;
        }

        private void OnItemDoubleClicked(VRCExpressionsMenu menu)
        {
            lastSelectedMenu = menu;
            editorWindow.Close();
        }

        public override void OnClose()
        {
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(_windowWidth, 150);
        }

        public override void OnGUI(Rect rect)
        {
            DrawSearchToolbar();
            DrawTreeView();
        }

        private void DrawSearchToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Space(100);
            GUILayout.FlexibleSpace();
            _treeView.searchString = _searchField.OnToolbarGUI(_treeView.searchString);
            GUILayout.EndHorizontal();
        }

        private void DrawTreeView()
        {
            Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
            _treeView.OnGUI(rect);
        }
    }
}