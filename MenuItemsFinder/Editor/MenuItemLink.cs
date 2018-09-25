using System.Linq;
using System.Reflection;
using UnityEditor;

namespace SKTools.MenuItemsFinder
{
    internal class MenuItemLink
    {
        public readonly MethodInfo Method;
        public readonly MenuItem MenuItem;
        public readonly string Key;
        public readonly string Label;

        public bool Starred;

        public MenuItemLink(MethodInfo method, MenuItem menuItem)
        {
            Method = method;
            MenuItem = menuItem;
            Key = menuItem.menuItem.ToLower();
            //% (ctrl on Windows, cmd on macOS), # (shift), & (alt).
            Label = menuItem.menuItem;
            var index = Label.LastIndexOf(' ');
            if (index > -1)
            {
                var s = Label.Substring(index);
                var k = s;
                if (s.Contains('%') || s.Contains('#') || s.Contains('&'))
                {
                        s =
    #if UNITY_EDITOR_OSX
                    s.Replace("%", "cmd+").
    #else
                    s.Replace("%", "ctrl+").
    #endif
                    Replace("#", "shift+").Replace("&", "alt+");
                    s = string.Concat("<color=cyan>", s, "</color>");
                    Label = Label.Replace(k, s);
                }
            }
        }

        public void Execute()
        {
            Method.Invoke(null, null);
        }

        public override string ToString()
        {
            return string.Format("MenuItem={0}; Method={1}", MenuItem.menuItem, Method.Name);
        }
    }
}