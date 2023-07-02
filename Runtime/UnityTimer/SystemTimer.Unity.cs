using System.Threading;

namespace SKTools.Core.Invoker
{
    public static partial class SystemTimer
    {
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialization()
        {
            _synchronizationContext = SynchronizationContext.Current;

            if (_synchronizationContext == null)
            {
                UnitySynchronizationContextIsNullException.ToLog();
            }

#if UNITY_EDITOR

            UnityEditor.EditorApplication.playModeStateChanged += playModeStateChange =>
            {
                if (playModeStateChange == UnityEditor.PlayModeStateChange.ExitingPlayMode)
                {
                    Clear();
                }
            };
#endif
        }
    }
}