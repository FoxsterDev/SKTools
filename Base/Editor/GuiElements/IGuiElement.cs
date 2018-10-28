namespace SKTools.Base.Editor.GuiElementsSystem
{
    public interface IGuiElement
    {
        string Id { get; }
        void Draw();
        T GetChild<T>(string id) where T : IGuiElement;
    }
}