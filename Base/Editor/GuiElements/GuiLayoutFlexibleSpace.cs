using UnityEngine;

namespace SKTools.Base.Editor.GuiElementsSystem
{
    public class GuiLayoutFlexibleSpace : IGuiElement
    {
        public string Id { get; private set; }

        public void Draw()
        {
            GUILayout.FlexibleSpace();
        }

        public T GetChild<T>(string id) where T : IGuiElement
        {
            if (Id == id) return (T) (object) this;
            return default(T);
        }
    }
}