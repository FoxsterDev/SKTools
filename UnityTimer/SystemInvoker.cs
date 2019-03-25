using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/*
 *  unity invoking 
 *
 *CancelInvoke	Cancels all Invoke calls on this MonoBehaviour.
Invoke	Invokes the method methodName in time seconds.
InvokeRepeating	Invokes the method methodName in time seconds, then repeatedly every repeatRate seconds.
IsInvoking
 *
 * 
 */
namespace SKTools.Core.Invoker
{
    public delegate void TimeDelegate(Moment value);

    public static partial class SystemInvoker
    {
        private class Bucket
        {
            public TimeDelegate Callback;
            public uint RepeatMs;

            public DateTime StartTime;

            //public DateTime NowTime;
            public DateTime CallTime;
            public int HashCode;
            public bool Repeating;
        }

        private const uint MinPeriodMs = 15;
        private const uint DueTimeMs = 15;
        private const int Capacity = 15;

        private static object _systemThreadingTimerState;
        private static System.Threading.Timer _systemThreadingTimer;
        private static List<Bucket> _buckets;
        private static DateTime _bucketsUpdateTime;
        private static SynchronizationContext _synchronizationContext;

        /// <summary>
        /// Is any invoke on callback pending?
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static bool IsInvoking(TimeDelegate callback)
        {
            return FindIndexBucket(callback) > -1;
        }

        /// <summary>
        /// nvokes the method methodName in time milliseconds.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="time"></param>
        public static void Invoke(TimeDelegate callback, uint time)
        {
            InvokeRepeating(callback, time, 0);
        }

        /// <summary>
        /// Invokes the method in time ms, then repeatedly every repeatRate ms.
        /// </summary>
        /// <param name="callback">the method</param>
        /// <param name="repeatRate"></param>
        /// <param name="time"></param>
        public static void InvokeRepeating(TimeDelegate callback, uint time, uint repeatRate)
        {
            var hashCode = -1;
            if (!CanStart(callback, ref hashCode))
            {
                return;
            }

            var bucket = CreateBucket(callback, hashCode, time, repeatRate);

            if (_buckets == null)
            {
                _buckets = new List<Bucket>(Capacity);
            }

            _buckets.Add(bucket);

            if (_systemThreadingTimer == null)
            {
                _systemThreadingTimerState = new object();
                _systemThreadingTimer = new System.Threading.Timer(SystemThreadingTimerCallback, _systemThreadingTimerState, DueTimeMs, MinPeriodMs);
            }
        }

        private static Bucket CreateBucket(TimeDelegate callback, int hashCode, uint time, uint repeatRate)
        {
            var repeatMs = Math.Max(repeatRate, MinPeriodMs);
            var now = DateTime.UtcNow;

            var bucket = new Bucket
            {
                Callback = callback,
                RepeatMs = repeatMs,
                StartTime = now,
                CallTime = now.AddMilliseconds(time),
                HashCode = hashCode,
                Repeating = repeatRate > 0
            };

            return bucket;
        }

        /// <summary>
        /// Can be called from different threads
        /// </summary>
        /// <param name="state"></param>
        private static void SystemThreadingTimerCallback(object state)
        {
            if (state != _systemThreadingTimerState)
            {
                return;
            }

            var now = DateTime.UtcNow;

            try
            {
                var context = _synchronizationContext;

                if (context == null)
                    return;

                if (context == SynchronizationContext.Current)
                {
                    UpdateBuckets(now);
                }
                else
                {
                    context.Post(_ => UpdateBuckets(now), state); //async 
                }
            }
            catch
            {
                //ignore
            }
        }

        private static void UpdateBuckets(DateTime now)
        {
            if (now > _bucketsUpdateTime)
            {
                _bucketsUpdateTime = now;

                for (var k = _buckets.Count - 1; k >= 0; k--)
                {
                    var timer = _buckets[k];

                    if (now >= timer.CallTime)
                    {
                        var moment = new Moment
                        {
                            Delta = now - timer.CallTime,
                            Passed = now - timer.StartTime
                        };

                        try
                        {
                            timer.Callback(moment);
                        }
                        catch
                        {
                            //ignore
                        }

                        if (timer.Repeating)
                        {
                            timer.CallTime = now.AddMilliseconds(timer.RepeatMs);
                        }
                        else
                        {
                            _buckets.RemoveAt(k);
                        }
                    }
                }
            }
        }

        private static int FindIndexBucket(TimeDelegate callback)
        {
            var hashCode = callback.GetHashCode();

            return _buckets != null
                       ? _buckets.FindIndex(t => t.HashCode == hashCode)
                       : -1;
        }

        public static void CancelInvoke(TimeDelegate callback)
        {
            var index = -1;

            if (CanCancel(callback, ref index))
            {
                _buckets.RemoveAt(index);

                if (_buckets.Count < 1)
                {
                    Clear();
                }
            }
        }

        private static void Clear()
        {
            _systemThreadingTimerState = null;

            if (_systemThreadingTimer != null)
            {
                _systemThreadingTimer.Dispose();
                _systemThreadingTimer = null;
            }

            if (_buckets != null)
            {
                _buckets.Clear();
            }
        }

        private static bool CanStart(TimeDelegate callback, ref int hashCode)
        {
            if (callback == null)
            {
                CantStartBecauseTimerCallbackIsNullException.ToLog();
                return false;
            }

            if (_synchronizationContext == null)
            {
                CantStartBecauseNullContextException.ToLog();
                return false;
            }

            if (_synchronizationContext != SynchronizationContext.Current)
            {
                CantStartBecauseNotStartedContextException.ToLog();
                return false;
            }

            var hash = callback.GetHashCode();
            if (_buckets != null && _buckets.Find(t => t.HashCode == hash) != null)
            {
                CantStartBecauseCallbackAlreadyAddedException.ToLog();
                return false;
            }

            hashCode = hash;

            return true;
        }

        private static bool CanCancel(TimeDelegate callback, ref int index)
        {
            if (callback == null)
            {
                CantResetBecauseArgumentCannotBeNullException.ToLog();
                return false;
            }

            if (_synchronizationContext == null || _synchronizationContext != SynchronizationContext.Current)
            {
                CantResetBecauseNotStartedContextException.ToLog();
                return false;
            }

            index = FindIndexBucket(callback);

            if (index < 0)
            {
                CantCancelBecauseNotExistCallbackException.ToLog();
                return false;
            }

            return true;
        }
    }
}
