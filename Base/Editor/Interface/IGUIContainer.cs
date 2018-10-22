using UnityEngine;

namespace SKTools.Base.Editor
{
    public interface IGUIContainer
    {
        GUIDelegate<Rect> DrawGuiCallback { get; set; }
        GUIDelegate<Rect> LostFocusCallback{ get; set; }
        GUIDelegate<Rect> CloseCallback{ get; set; }
        void Configurate();
        void Show();
    }
}