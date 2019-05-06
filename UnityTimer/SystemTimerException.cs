using System;
using UnityEngine;

namespace SKTools.Core.Invoker
{
    public class SystemTimerException<T> : Exception where T : class, new()
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

    public class CantResetBecauseCallbackCannotBeNullException : SystemTimerException<CantResetBecauseCallbackCannotBeNullException>
    {
    }

    public class UnitySynchronizationContextIsNullException : SystemTimerException<UnitySynchronizationContextIsNullException>
    {
    }

    public class CantStartBecauseCallbackCannotBeNullException : SystemTimerException<CantStartBecauseCallbackCannotBeNullException>
    {
    }

    public class CantStartBecauseCallbackExistedException : SystemTimerException<CantStartBecauseCallbackExistedException>
    {
    }

    public class CantCancelBecauseNotExistCallbackException : SystemTimerException<CantCancelBecauseNotExistCallbackException>
    {
    }

    public class CantStartBecauseNullContextException : SystemTimerException<CantStartBecauseNullContextException>
    {
    }

    public class CantResetBecauseNotStartedContextException : SystemTimerException<CantResetBecauseNotStartedContextException>
    {
    }

    public class CantStartBecauseNotStartedContextException : SystemTimerException<CantStartBecauseNotStartedContextException>
    {
    }

    public class CantStartBecauseInValidKeyException : SystemTimerException<CantStartBecauseInValidKeyException>
    {
    }
}
