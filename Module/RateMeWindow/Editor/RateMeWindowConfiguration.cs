﻿using SKTools.Core.Editor;
using UnityEngine;

namespace SKTools.Module.RateMeWindow
{
    /// <summary>
    /// This window will show rate me window
    /// </summary>
    internal class Configuration : CustomEditorWindow<Configuration>, IGUIContainer
    {
        protected override GUIContent GetTitleContent
        {
            get { return new GUIContent("Rate Me"); }
        }

        protected override Vector2? GetMinSize
        {
            get { return new Vector2(400, 400); }
        }

        protected override Vector2? GetMaxSize
        {
            get { return new Vector2(400, 400); }
        }

        public Rect Position
        {
            get { return position; }
        }
    }
}