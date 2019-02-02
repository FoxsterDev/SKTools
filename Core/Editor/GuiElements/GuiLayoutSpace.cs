using UnityEngine;

namespace SKTools.Core.Editor.GuiElementsSystem
{
    public class GuiLayoutSpace : IGuiElement
    {
        private float _pixels;

        public string Id { get; private set; }

        public GuiLayoutSpace(float pixels)
        {
            _pixels = pixels;
        }

        public void Draw()
        {
            GUILayout.Space(_pixels);
        }

        public T GetChild<T>(string id) where T : IGuiElement
        {
            if (Id == id) return (T) (object) this;
            return default(T);
        }
    }
}