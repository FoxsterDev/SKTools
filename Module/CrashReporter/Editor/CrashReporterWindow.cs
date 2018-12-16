﻿using SKTools.Base.Editor;
using UnityEngine;

namespace SKTools.Module.CrashReporter
{
    /// <summary>
    /// This window will show rate me window
    /// </summary>
    internal sealed class Window : CustomEditorWindow<Window>, IGUIContainer
    {
        protected override GUIContent GetTitleContent
        {
            get { return new GUIContent("Crash Reporter"); }
        }

        protected override Vector2? GetMinSize
        {
            get { return new Vector2(450, 350); }
        }

        protected override Vector2? GetMaxSize
        {
            get { return new Vector2(450, 350); }
        }

        public Rect Position
        {
            get { return position; }
        }
    }
}