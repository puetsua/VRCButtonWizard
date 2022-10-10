using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Puetsua.VRCButtonWizard.Editor
{
    public class AssetPathPopupWindow : PopupWindowContent
    {
        public static string lastSelectedPath;

        private readonly float _windowWidth;
        private readonly Action<string> _onPathSelected;

        private bool _assetFoldout;
        private AssetPathTreeView _treeView;
        private TreeViewState _treeViewState;
        private SearchField _searchField;

        public AssetPathPopupWindow(float windowWidth)
        {
            _windowWidth = windowWidth;
        }

        public override void OnOpen()
        {
            if (_treeViewState == null)
                _treeViewState = new TreeViewState();

            _treeView = new AssetPathTreeView(_treeViewState, OnItemDoubleClicked);
            _searchField = new SearchField();
            _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;
        }

        private void OnItemDoubleClicked(string path)
        {
            lastSelectedPath = path;
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