using UnityEngine;

namespace SKTools.Base.Editor
{
    public interface IGUIContainer
    {
        Rect Position { get; }
        GUIDelegate<IGUIContainer> DrawGuiCallback { set; }
        GUIDelegate<IGUIContainer> LostFocusCallback{ set; }
        GUIDelegate<IGUIContainer> CloseCallback{ set; }
        void Init();
        void Show();
        void Repaint();
    }
}