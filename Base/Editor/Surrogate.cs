namespace SKTools.Base.Editor
{
    public class Surrogate<T, K>  
        where T : IGUIContainer 
        where K : IAssetsContainer
    {
        public readonly T Container;
        public readonly K Assets;
        
        public Surrogate(T container, K assetsProvider)
        {
            Container = container;
            Assets = assetsProvider;
        }
    }
}