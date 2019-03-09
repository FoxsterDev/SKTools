using System;
using UnityEngine;

namespace SKTools.Core.Invoker
{
    public class SystemInvokeException<T> : Exception where T : class , new()
    {
        private static T _ex;

        public static T Exception
        {
            get { return _ex ?? (_ex = new T()); }
        }

        public static void Print()
        {
            Debug.LogException((Exception)(object)Exception);
        }
    }
    
    public class CantResetBecauseArgumentCannotBeNullException : SystemInvokeException<CantResetBecauseArgumentCannotBeNullException>{ }
    public class UnitySynchronizationContextIsNullException : SystemInvokeException<UnitySynchronizationContextIsNullException>{}
    public class CantStartBecauseTimerCallbackIsNullException : SystemInvokeException<CantStartBecauseTimerCallbackIsNullException>{}
    public class CantStartBecauseTheCallbackAlreadyUsedException : SystemInvokeException<CantStartBecauseTheCallbackAlreadyUsedException>{}
    public class CantResetBecauseNotExistCallbackException : SystemInvokeException<CantResetBecauseNotExistCallbackException>{}
    public class CantStartBecauseNullContextException : SystemInvokeException<CantStartBecauseNullContextException>{}
    public class CantResetBecauseNotUnityContextException : SystemInvokeException<CantResetBecauseNotUnityContextException>{}
    public class CantStartBecauseNotUnityContextException : SystemInvokeException<CantStartBecauseNotUnityContextException>{}
    public class CantStartBecauseInValidKeyException : SystemInvokeException<CantStartBecauseInValidKeyException>{}
}