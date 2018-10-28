using System;
using UnityEngine;

namespace SKTools.Base.Editor.GuiElementsSystem
{
    public class GuiLayoutButton : IGuiElement
    {
        private Action _act;
        private string _name;
        private GUIStyle _style;

        public string Id
        {
            get { return _name; }
        }

        public GuiLayoutButton(string name, GUIStyle style, Action act = null)
        {
            _act = act;
            _name = name;
            _style = style;
        }

        public void Draw()
        {
            if (GUILayout.Button(_name, _style))
            {
                if (_act != null)
                {
                    _act();
                }
            }
        }

        public T GetChild<T>(string id) where T : IGuiElement
        {
            if (Id == id) return (T) (object) this;
            return default(T);
        }
    }
}