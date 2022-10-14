using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Puetsua.VRCButtonWizard.Editor
{
    public class VrcMenuTreeView : TreeView
    {
        private const int MaxDepth = 10;

        public static bool debug = false;

        private static int _maxDepthReached;

        private readonly Action<VRCExpressionsMenu> _onItemSelect;
        private int _itemId;
        private VRCExpressionsMenu _rootMenu;
        private List<VRCExpressionsMenu> _menus = new List<VRCExpressionsMenu>();

        public VrcMenuTreeView(TreeViewState treeViewState, VRCExpressionsMenu rootMenu,
            Action<VRCExpressionsMenu> onItemSelect)
            : base(treeViewState)
        {
            _rootMenu = rootMenu;
            _onItemSelect = onItemSelect;
            Reload();
        }

        public void FocusOnMenu(VRCExpressionsMenu menu)
        {
            var id = _menus.IndexOf(menu);

            SetSelection(new List<int> {id});

            foreach (var parentId in GetAncestors(id))
            {
                SetExpanded(parentId, true);
            }
        }

        protected override TreeViewItem BuildRoot()
        {
            if (debug) _maxDepthReached = 0;
            _itemId = 0;

            var root = new TreeViewItem(-1, -1);
            var items = GetItems(_rootMenu, _rootMenu.name, 0).ToList();
            if (debug) Debug.Log($"Traversed maximum {_maxDepthReached} folder depth.");

            SetupParentsAndChildrenFromDepths(root, items);

            return root;
        }

        protected override void DoubleClickedItem(int id)
        {
            _onItemSelect(_menus[id]);
        }

        protected override void KeyEvent()
        {
            var e = Event.current;
            if (e.type == EventType.KeyDown &&
                e.keyCode == KeyCode.Return)
            {
                _onItemSelect(_menus[state.lastClickedID]);
            }
        }

        private IEnumerable<TreeViewItem> GetItems(VRCExpressionsMenu menu, string menuName, int depth)
        {
            var subMenus = new List<(VRCExpressionsMenu, string)>();
            foreach (var control in menu.controls)
            {
                if (control.type == VRCExpressionsMenu.Control.ControlType.SubMenu &&
                    control.subMenu != null)
                {
                    subMenus.Add((control.subMenu, control.name));
                }
            }

            var items = new List<TreeViewItem>
            {
                new TreeViewItem
                {
                    id = _itemId++,
                    depth = depth,
                    displayName = menu.name == menuName
                        ? menu.name
                        : $"{menu.name} ({menuName})",
                }
            };
            _menus.Add(menu);

            if (depth > MaxDepth)
                return items;

            foreach (var subMenu in subMenus)
            {
                items.AddRange(GetItems(subMenu.Item1, subMenu.Item2, depth + 1));
            }

            if (debug && depth > _maxDepthReached)
            {
                _maxDepthReached = depth;
            }

            return items;
        }
    }
}