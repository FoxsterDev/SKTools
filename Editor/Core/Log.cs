using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace SKTools.Editor
{
    public class Log
    {
#if FOXSTER_DEV_MODE
        private static readonly bool debug = true;
#else
        private static readonly bool debug = false;
#endif

        
        [Conditional(Const.FOXSTER_DEV_MODE)]
        public static void Debug(object message)
        {
            UnityEngine.Debug.Log(message);
        }
        
        [Conditional(Const.FOXSTER_DEV_MODE)]
        public static void Info(object message)
        {
            UnityEngine.Debug.Log(message);
        }
        
        [Conditional(Const.FOXSTER_DEV_MODE)]
        public static void Warning(object message)
        {
            UnityEngine.Debug.Log(message);
        }
        
        //[Conditional(Const.FOXSTER_DEV_MODE)]
        public static void Error(object message)
        {
            UnityEngine.Debug.Log(message);
        }
    }
}