using UnityEngine;

namespace SKTools.Base.Editor
{
    public interface IAssetsContainer
    {
        T Get<T>(string name) where T : Object;
    }
}