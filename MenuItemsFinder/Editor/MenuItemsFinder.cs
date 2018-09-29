using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SKTools.MenuItemsFinder
{
    internal class MenuItemsFinder
    {
        private string _prefsFilePath;

        public List<MenuItemLink> MenuItems, SelectedItems = new List<MenuItemLink>();
        public Texture2D StarredImage, UnstarredImage;
        public MenuItemsFinderPreferences Prefs = new MenuItemsFinderPreferences
        {
            SearchString = "Please type menuitem name here.."
        };

        public void SavePrefs()
        {
            try
            {
                File.WriteAllText(_prefsFilePath, EditorJsonUtility.ToJson(Prefs));
            }
            catch
            {
            }
        }

        public void Load()
        {
            try
            {
                var stackTrace = new StackTrace(true);
                var editorDirectory = stackTrace.GetFrames()[0].GetFileName()
                    .Replace(typeof(MenuItemsFinder).Name + ".cs", string.Empty);
                _prefsFilePath = editorDirectory + "Prefs.json";
                var starFilePath = editorDirectory.Replace("Editor/", "Editor Resources/")
                    .Substring(Application.dataPath.Length - "Assets".Length);
                UnstarredImage = AssetDatabase.LoadAssetAtPath<Texture2D>(starFilePath + "unstarred.png");
                StarredImage = AssetDatabase.LoadAssetAtPath<Texture2D>(starFilePath + "starred.png");

                if (File.Exists(_prefsFilePath))
                {
                    EditorJsonUtility.FromJsonOverwrite(File.ReadAllText(_prefsFilePath), Prefs);
                }

                MenuItems = FindAllMenuItems();
                MenuItems.Sort((x, y) => y.Label[0] - x.Label[0]);
                if (Prefs.StarredMenuItems.Count > 0)
                {
                    MenuItems.ForEach(i =>
                    {
                        if (!i.MenuItem.validate && Prefs.StarredMenuItems.Contains(i.MenuItem.menuItem))
                        {
                            i.Starred = true;
                        }
                    });
                }
            }
            catch
            {
            }
        }

        private List<MenuItemLink> FindAllMenuItems()
        {
            var watch = new Stopwatch();
            watch.Start();

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

            watch.Stop();
            Debug.Log("Time to FindAllMenuItems takes=" + watch.ElapsedMilliseconds +
                      "ms Count="+menuItems.Count); //for mac book pro 2018 it takes about 170 ms, it is not critical affects every time to run it

            return menuItems;
        }
    }
}