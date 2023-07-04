namespace SKTools.Editor
{
    public interface IAssetsContainer
    {
        T Get<T>(string name) where T : UnityEngine.Object;
    }
}