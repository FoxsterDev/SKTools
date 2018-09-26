using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SKTools.MenuItemsFinder
{
    internal class MenuItemLink 
    {
        public readonly MethodInfo Method;
        public readonly MenuItem MenuItem;
        public readonly string Label;
        public bool Starred;

        public MenuItemLink(MethodInfo method, MenuItem menuItem)
        {
            Method = method;
            MenuItem = menuItem;
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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MenuItemLink) obj);
        }

        public override int GetHashCode()
        {
            return (MenuItem != null ? MenuItem.GetHashCode() : 0);
        }
    }
}