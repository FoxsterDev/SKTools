using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

namespace SKTools.MenuItemsFinder
{
    //windows doesn work starred + hotkeys ctrl + shift..
    //[Done] replace special hotkeys to readable symbols
    //[Done] to separate searchbox
    //to plan: check validate method
    //to provide and check context for menu item
    //to add star prefs
    //[Done] add json prefs
    internal class MenuItemsFinderWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private GUIStyle _menuItemButtonStyle, _unstarredMenuItemButtonStyle, _starredMenuItemButtonStyle;
        private MenuItemsFinder _finder;

        [MenuItem("SKTools/MenuItems Finder %#f")]
        private static void Init()
        {
            var finderWindow = (MenuItemsFinderWindow) GetWindow(typeof(MenuItemsFinderWindow));
            finderWindow.Show();
        }

        private void CreateStyles()
        {
            _menuItemButtonStyle = new GUIStyle(EditorStyles.miniButton);
            _menuItemButtonStyle.fixedHeight = 20;
            //_menuItemButtonStyle.fixedWidth = 200;
            _menuItemButtonStyle.alignment = TextAnchor.MiddleLeft;
            _menuItemButtonStyle.richText = true;

            _unstarredMenuItemButtonStyle = new GUIStyle(EditorStyles.miniButton);
            _unstarredMenuItemButtonStyle.fixedHeight = 28;
            _unstarredMenuItemButtonStyle.fixedWidth = 28;
            _unstarredMenuItemButtonStyle.stretchHeight = false;
            _unstarredMenuItemButtonStyle.stretchWidth = false;
            _unstarredMenuItemButtonStyle.imagePosition = ImagePosition.ImageOnly;
            _unstarredMenuItemButtonStyle.overflow = new RectOffset(0, 0, 6, -6);
            _unstarredMenuItemButtonStyle.active.background =
                _unstarredMenuItemButtonStyle.focused.background =
                    _unstarredMenuItemButtonStyle.hover.background =
                        _unstarredMenuItemButtonStyle.normal.background = _finder.UnstarredImage;

            _starredMenuItemButtonStyle = new GUIStyle(_unstarredMenuItemButtonStyle);

            _starredMenuItemButtonStyle.active.background =
                _starredMenuItemButtonStyle.focused.background =
                    _starredMenuItemButtonStyle.hover.background =
                        _starredMenuItemButtonStyle.normal.background = _finder.StarredImage;
        }

        private void Awake()
        {
            _finder = new MenuItemsFinder();
            _finder.Load();
            CreateStyles();
        }

        private void OnGUI()
        {
            _finder.Prefs.SearchString = GUILayoutCollection.SearchTextField(_finder.Prefs.SearchString, GUILayout.MinWidth(200));

            if (!_finder.Prefs.SearchString.Equals(_finder.Prefs.PreviousSearchString))
            {
                _finder.Prefs.PreviousSearchString = _finder.Prefs.SearchString;
                _finder.SelectedItems =
                    _finder.MenuItems.FindAll(m => m.Label.Contains(_finder.Prefs.PreviousSearchString.ToLower()));
            }
            
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);

            if (_finder.Prefs.StarredMenuItems.Count > 0)
            {
                foreach (var item in _finder.MenuItems)
                {
                    if (!item.Starred)
                        continue;
                    Draw(item);
                }
            }

            if (_finder.SelectedItems.Count > 0)
            {
                foreach (var item in _finder.SelectedItems)
                {
                    if (item.Starred)
                        continue;
                    Draw(item);
                }
            }
            
            GUILayout.EndScrollView();
        }
        
        private void OnDestroy()
        {
            _finder.SavePrefs();
            AssetDatabase.Refresh();
        }

        private void Draw(MenuItemLink item)
        {
            GUILayout.BeginHorizontal();

            GUI.color = item.MenuItem.validate ? Color.gray : Color.white;
            if (GUILayout.Button(item.Label, _menuItemButtonStyle, GUILayout.MaxWidth(300)))
            {
                Debug.Log("Try execute menuItem=" + item);
                try
                {
                    item.Execute();
                }
                catch (Exception ex)
                {
                    Debug.LogError("cant execute this menu item: " + item + "\n" + ex);
                }
            }

            GUI.color = Color.white;
            if (GUILayout.Button("", item.Starred ? _starredMenuItemButtonStyle : _unstarredMenuItemButtonStyle))
            {
                item.Starred = !item.Starred;
                if (item.Starred && !_finder.Prefs.StarredMenuItems.Contains(item.MenuItem.menuItem))
                {
                    _finder.Prefs.StarredMenuItems.Add(item.MenuItem.menuItem);
                }
                else if (_finder.Prefs.StarredMenuItems.Contains(item.MenuItem.menuItem))
                {
                    _finder.Prefs.StarredMenuItems.Remove(item.MenuItem.menuItem);
                }
            }

            GUILayout.EndHorizontal();
        }
    }
}