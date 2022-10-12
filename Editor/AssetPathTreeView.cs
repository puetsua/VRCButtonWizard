﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Puetsua.VRCButtonWizard.Editor
{
    internal class AssetPathTreeView : TreeView
    {
        private const int MaxDepth = 10;
        
        public static bool debug = false;
        
        private static int _maxDepthReached;

        private readonly Action<string> _onItemDoubleClicked;
        private int _itemId;
        private List<string> _paths = new List<string>();

        public AssetPathTreeView(TreeViewState treeViewState, Action<string> onItemDoubleClicked)
            : base(treeViewState)
        {
            _onItemDoubleClicked = onItemDoubleClicked;
            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            if (debug) _maxDepthReached = 0;
            _itemId = 0;
            var items = GetItems("Assets", -1).ToList();
            if (debug) Debug.Log($"Traversed maximum {_maxDepthReached} folder depth.");

            TreeViewItem root = items[0];
            items.RemoveAt(0);

            SetupParentsAndChildrenFromDepths(root, items);

            return root;
        }

        protected override void DoubleClickedItem(int id)
        {
            var path = _paths[id];
            _onItemDoubleClicked(path);
        }

        private IEnumerable<TreeViewItem> GetItems(string folderPath, int depth)
        {
            string[] subFolderPaths = AssetDatabase.GetSubFolders(folderPath);
            var items = new List<TreeViewItem>
            {
                new TreeViewItem
                {
                    id = _itemId++, 
                    depth = depth, 
                    displayName = Path.GetFileName(folderPath),
                }
            };
            _paths.Add(folderPath);

            if (depth > MaxDepth)
                return items;

            foreach (string subFolderPath in subFolderPaths)
            {
                items.AddRange(GetItems(subFolderPath, depth + 1));
            }

            if (debug && depth > _maxDepthReached)
            {
                _maxDepthReached = depth;
            }

            return items;
        }
    }
}