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
    //to plan: check validate method
    //to provide and check context for menu item
    //to add star prefs
    //[Done] add json prefs
    internal class MenuItemsFinderWindow : EditorWindow
    {
        private GUIStyle _toolbarSearchFieldStyle, _toolbarSearchFieldCancelButtonStyle, _menuItemButton;
        private List<MenuItemLink> _menuItems;
        private List<MenuItemLink> _selection = new List<MenuItemLink>();
        //private string _previousSearchString = string.Empty, _searchString = "Please type menuitem name here..";
        private Vector2 _scrollPosition;
        private string _prefsFilePath;

        private MenuItemsFinderPreferences prefs = new MenuItemsFinderPreferences
        {
            SearchString = "Please type menuitem name here.."
        };

        [MenuItem("SKTools/MenuItems Finder %#f")]
        private static void Init()
        {
            var finderWindow = (MenuItemsFinderWindow) GetWindow(typeof(MenuItemsFinderWindow));
            finderWindow.Show();
        }
        
        private void Awake()
        {
            LoadPrefs();
            
            var instance = (EditorStyles) typeof(EditorStyles)
                .GetField("s_Current", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            _toolbarSearchFieldStyle = (GUIStyle) typeof(EditorStyles)
                .GetField("m_ToolbarSearchField", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance); //
            _toolbarSearchFieldStyle = new GUIStyle(_toolbarSearchFieldStyle);
            _toolbarSearchFieldStyle.stretchWidth = true;
            _toolbarSearchFieldStyle.stretchHeight = true;

            _toolbarSearchFieldCancelButtonStyle = (GUIStyle) typeof(EditorStyles)
                .GetField("m_ToolbarSearchFieldCancelButton", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(instance); //
            _toolbarSearchFieldCancelButtonStyle = new GUIStyle(_toolbarSearchFieldCancelButtonStyle);

            _menuItemButton = new GUIStyle(EditorStyles.miniButton);
            _menuItemButton.alignment = TextAnchor.MiddleLeft;
            
            var watch = new Stopwatch();
            watch.Start();
            _menuItems = FindAllMenuItems();
            watch.Stop();
            Debug.Log("Time to FindAllMenuItems takes="+ watch.ElapsedMilliseconds+"ms");//for mac book pro 2018 it takes about 170 ms, it is not critical affects every time to run it

            _menuItems.Sort((x, y) => y.Key[0] - x.Key[0]);
        }

        private void SavePrefs()
        {
            try
            {
                System.IO.File.WriteAllText(_prefsFilePath, EditorJsonUtility.ToJson(prefs));
            }
            catch{ }
        }

        private void LoadPrefs()
        {
            try
            {
                var stackTrace = new StackTrace(true);
                _prefsFilePath = stackTrace.GetFrames()[0].GetFileName()
                    .Replace(typeof(MenuItemsFinderWindow).Name + ".cs", "Prefs.json");

                if (File.Exists(_prefsFilePath))
                {
                    EditorJsonUtility.FromJsonOverwrite(File.ReadAllText(_prefsFilePath), prefs);
                }
            }catch{}
        }
        
        private void OnDestroy()
        {
           AssetDatabase.Refresh();    
        }
       
        private void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            prefs.SearchString = GUILayout.TextField(prefs.SearchString, _toolbarSearchFieldStyle, GUILayout.MinWidth(200));
            if (GUILayout.Button(string.Empty, _toolbarSearchFieldCancelButtonStyle))
            {
                prefs.SearchString = string.Empty;
                GUI.FocusControl(null);
            }

            GUILayout.EndHorizontal();

            if (!prefs.PreviousSearchString.Equals(prefs.SearchString))
            {
                prefs.PreviousSearchString = prefs.SearchString;
                _selection = _menuItems.FindAll(m => m.Key.Contains(prefs.PreviousSearchString.ToLower()));
                SavePrefs();
            }

            if (_selection.Count > 0)
            {
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);
                foreach (var item in _selection)
                {
                    GUI.color = item.MenuItem.validate ? Color.gray : Color.white;
                    if (GUILayout.Button(item.MenuItem.menuItem + " validate=" + item.MenuItem.validate,
                        _menuItemButton))
                    {
                        Debug.Log("Try execute menuItem="+ item);
                        try
                        {
                            item.Execute();
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("cant execute this menu item: " + item +"\n"+ ex);
                        }
                    }

                    GUI.color = Color.white;
                }

                GUILayout.EndScrollView();
            }
        }
        
        private static List<MenuItemLink> FindAllMenuItems()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var menuItems = new List<MenuItemLink>(200);
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var methods =
                        type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic |
                                        BindingFlags.Public); 

                    foreach (var method in methods)
                    {
                        var items = method.GetCustomAttributes(typeof(MenuItem), false).Cast<MenuItem>().ToArray();
                        if (items.Length != 1) continue;
                        menuItems.Add(new MenuItemLink(method, items[0]));
                    }
                }
            }

             return menuItems;
        }
    }
}