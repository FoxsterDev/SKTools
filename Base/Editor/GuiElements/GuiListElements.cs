using System.Collections.Generic;

namespace SKTools.Base.Editor.GuiElementsSystem
{
    public class GuiListElements : IGuiElement
    {
        public List<IGuiElement> List;
        private IGuiElement _space;

        public string Id { get; private set; }

        /// <summary>
        ///  Define container elements
        /// </summary>
        /// <param name="id">Uniq id of the container</param>
        /// <param name="space">Layout space between elements</param>
        /// <param name="list"></param>
        public GuiListElements(string id, float space, params IGuiElement[] list)
        {
            Id = id;
            List = new List<IGuiElement>(list);
            _space = new GuiLayoutSpace(space);
        }

        public GuiListElements(params IGuiElement[] list)
        {
            List = new List<IGuiElement>(list);
        }

        public GuiListElements Add(IGuiElement element)
        {
            List.Add(element);
            return this;
        }

        public T GetChild<T>(string id) where T : IGuiElement
        {
            if (id == Id) return (T) (object) this;
            
            foreach (var el in List)
            {
                var child = el.GetChild<T>(id);
                if (!ReferenceEquals(child, default(T)))
                    return child;
            }

            return default(T);
        }

        public void Draw()
        {
            foreach (var el in List)
            {
                el.Draw();
                if (_space != null)
                {
                    _space.Draw();
                }
            }
        }
    }
}