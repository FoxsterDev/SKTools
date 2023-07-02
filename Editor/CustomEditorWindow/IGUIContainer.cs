using UnityEngine;

namespace SKTools.Core.Editor
{
    public delegate void GUIDelegate<T>(T obj);

    public interface IGUIContainer
    {
        Rect Position { get; }
        GUIDelegate<IGUIContainer> DrawGuiCallback { set; }
        GUIDelegate<IGUIContainer> FocusCallback { set; }
        GUIDelegate<IGUIContainer> LostFocusCallback { set; }
        GUIDelegate<IGUIContainer> CloseCallback { set; }
        void Init();
        void Show();
        void Focus();
        void Repaint();
        void Close();
    }
}