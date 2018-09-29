﻿using System.Collections.Generic;

namespace SKTools.MenuItemsFinder
{
    [System.Serializable]
    internal class MenuItemsFinderPreferences
    {
        [System.NonSerialized]
        public string PreviousSearchString = null;
        public string SearchString = string.Empty;
        public List<string> StarredMenuItems = new List<string>();
    }
}