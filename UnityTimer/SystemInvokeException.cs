using System;
using UnityEngine;

namespace SKTools.Core.Invoker
{
    public class SystemInvokeException<T> : Exception where T : class, new()
    {
        private static T _ex;

        public static T Exception
        {
            get { return _ex ?? (_ex = new T()); }
        }

        public static void ToLog()
        {
            Debug.LogException((Exception) (object) Exception);
        }
    }

    public class CantResetBecauseArgumentCannotBeNullException : SystemInvokeException<CantResetBecauseArgumentCannotBeNullException>
    {
    }

    public class UnitySynchronizationContextIsNullException : SystemInvokeException<UnitySynchronizationContextIsNullException>
    {
    }

    public class CantStartBecauseTimerCallbackIsNullException : SystemInvokeException<CantStartBecauseTimerCallbackIsNullException>
    {
    }

    public class CantStartBecauseCallbackAlreadyAddedException : SystemInvokeException<CantStartBecauseCallbackAlreadyAddedException>
    {
    }

    public class CantCancelBecauseNotExistCallbackException : SystemInvokeException<CantCancelBecauseNotExistCallbackException>
    {
    }

    public class CantStartBecauseNullContextException : SystemInvokeException<CantStartBecauseNullContextException>
    {
    }

    public class CantResetBecauseNotStartedContextException : SystemInvokeException<CantResetBecauseNotStartedContextException>
    {
    }

    public class CantStartBecauseNotStartedContextException : SystemInvokeException<CantStartBecauseNotStartedContextException>
    {
    }

    public class CantStartBecauseInValidKeyException : SystemInvokeException<CantStartBecauseInValidKeyException>
    {
    }
}
