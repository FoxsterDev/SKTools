using System;
using UnityEngine;

namespace SKTools.Editor.GuiElementsSystem
{
    public class GuiLayoutToggle : IGuiElement
    {
        public string Id { get; private set; }
        public string Label { get; private set; }
        private Action<bool> _setValue;
        private Func<bool> _getValue;
        private GUILayoutOption[] _options;

        public GuiLayoutToggle(string label, Action<bool> setValue, Func<bool> getValue,
            params GUILayoutOption[] options)
        {
            Id = label;
            Label = label;
            _setValue = setValue;
            _getValue = getValue;
            _options = options;
        }

        public void Draw()
        {
            _setValue(GUILayout.Toggle(_getValue.Invoke(), Id, _options));
        }

        public T GetChild<T>(string id) where T : IGuiElement
        {
            if (Id == id) return (T) (object) this;
            return default(T);
        }
    }
}