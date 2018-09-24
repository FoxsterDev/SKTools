using System.Reflection;
using UnityEditor;

namespace SKTools.MenuItemsFinder
{
    internal class MenuItemLink
    {
        public readonly MethodInfo Method;
        public readonly MenuItem MenuItem;
        public readonly string Key;

        public MenuItemLink(MethodInfo method, MenuItem menuItem)
        {
            Method = method;
            MenuItem = menuItem;
            Key = menuItem.menuItem.ToLower();
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