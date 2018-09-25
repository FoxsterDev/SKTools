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
    //replace special hotkeys to readable symbols
    //[Done] to separate searchbox
    //to plan: check validate method
    //to provide and check context for menu item
    //to add star prefs
    //[Done] add json prefs
    internal class MenuItemsFinderWindow : EditorWindow
    {
        private List<MenuItemLink> _menuItems;
        private List<MenuItemLink> _selection = new List<MenuItemLink>();
        private Vector2 _scrollPosition;
        private string _prefsFilePath;
        private  GUIStyle _menuItemButton, _unstarredMenuItemButton, _starredMenuItemButton;
        
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

        private void CreateStyles()
        {
            _menuItemButton = new GUIStyle(EditorStyles.miniButton);
            _menuItemButton.fixedHeight = 20;
            _menuItemButton.alignment = TextAnchor.MiddleLeft;
            _menuItemButton.richText = true;
            
            _unstarredMenuItemButton = new GUIStyle(EditorStyles.miniButton);
            _unstarredMenuItemButton.fixedHeight = 28;
            _unstarredMenuItemButton.fixedWidth = 28;
            _unstarredMenuItemButton.stretchHeight = false;
            _unstarredMenuItemButton.stretchWidth = false;
            _unstarredMenuItemButton.imagePosition = ImagePosition.ImageOnly;
            _unstarredMenuItemButton.overflow = new RectOffset(0, 0, 6, -6);
            _starredMenuItemButton = new GUIStyle(_unstarredMenuItemButton);
        }
        
        private void Awake()
        {
            CreateStyles();
            Load();
            var watch = new Stopwatch();
            watch.Start();
            _menuItems = FindAllMenuItems();
            watch.Stop();
            Debug.Log("Time to FindAllMenuItems takes="+ watch.ElapsedMilliseconds+"ms");//for mac book pro 2018 it takes about 170 ms, it is not critical affects every time to run it

            _menuItems.Sort((x, y) => y.Key[0] - x.Key[0]);
        }

        private void Save()
        {
            try
            {
                File.WriteAllText(_prefsFilePath, EditorJsonUtility.ToJson(prefs));
            }
            catch{ }
        }

        private void Load()
        {
            try
            {
                var stackTrace = new StackTrace(true);
                var editorDirectory = stackTrace.GetFrames()[0].GetFileName()
                    .Replace(typeof(MenuItemsFinderWindow).Name + ".cs", string.Empty);
                _prefsFilePath = editorDirectory + "Prefs.json";
                var starFilePath = editorDirectory.Replace("Editor/", "Editor Resources/").Substring(Application.dataPath.Length-"Assets".Length);
                _unstarredMenuItemButton.active.background = 
                _unstarredMenuItemButton.focused.background =
                _unstarredMenuItemButton.hover.background = 
                _unstarredMenuItemButton.normal.background  = AssetDatabase.LoadAssetAtPath<Texture2D>(starFilePath+"unstarred.png");
                
                _starredMenuItemButton.active.background = 
                    _starredMenuItemButton.focused.background =
                        _starredMenuItemButton.hover.background = 
                            _starredMenuItemButton.normal.background  = AssetDatabase.LoadAssetAtPath<Texture2D>(starFilePath+"starred.png");

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
            prefs.SearchString = GUILayoutCollection.SearchTextField(prefs.SearchString, GUILayout.MinWidth(200));
           
            if (!prefs.PreviousSearchString.Equals(prefs.SearchString))
            {
                prefs.PreviousSearchString = prefs.SearchString;
                _selection = _menuItems.FindAll(m => m.Key.Contains(prefs.PreviousSearchString.ToLower()));
                Save();
            }

            if (_selection.Count > 0)
            {
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);
                var y = 40f;
                foreach (var item in _selection)
                {
                    GUILayout.BeginHorizontal();
                    
                    GUI.color = item.MenuItem.validate ? Color.gray : Color.white;
                    if (GUILayout.Button(item.Label, _menuItemButton))
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
                    if (GUILayout.Button("", item.Starred ? _starredMenuItemButton : _unstarredMenuItemButton))//, GUILayout.MaxWidth(32), GUILayout.MaxHeight(32)))
                    {
                        item.Starred = !item.Starred;
                    }
                    
                    GUILayout.EndHorizontal();

                    y += (_menuItemButton.fixedHeight + 2);
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