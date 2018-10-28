using UnityEngine;

namespace SKTools.Base.Editor.GuiElementsSystem
{
    public class GuiLayoutHorizontal : IGuiElement
    {
        public IGuiElement Element;

        public string Id { get; private set; }


        public GuiLayoutHorizontal(IGuiElement element)
        {
            Element = element;
        }

        public void Draw()
        {
            GUILayout.BeginHorizontal();
            Element.Draw();
            GUILayout.EndHorizontal();
        }

        public T GetChild<T>(string id) where T : IGuiElement
        {
            if (Id == id) return (T) (object) this;
            return Element.GetChild<T>(id);
        }
    }
}