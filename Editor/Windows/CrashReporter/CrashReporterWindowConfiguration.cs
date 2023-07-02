using SKTools.Editor;
using UnityEngine;

namespace SKTools.Editor.Windows.CrashReporter
{
    /// <summary>
    /// This window will show rate me window
    /// </summary>
    internal sealed class Configuration : CustomEditorWindow<Configuration>, IGUIContainer
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