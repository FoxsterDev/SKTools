using System;
using System.Collections.Generic;
using System.Threading;

namespace SKTools.Core.Invoker
{
    public delegate void TimeDelegate(Moment value);

    public static class SystemInvoker
    {
        private class Bucket
        {
            public TimeDelegate Callback;
            public uint DeltaMs;
            public DateTime StartTime;
            public DateTime NowTime;
            public int HashCode;
        }

        private const uint MinPeriodMs = 15;
        private const uint DueTimeMs = 15;

        private static SynchronizationContext _unitySynchronizationContext;
        private static object _cookie;
        private static System.Threading.Timer _systemThreadingTimer;
        private static List<Bucket> _buckets;

        /// <summary>
        /// Invokes the method in time ms, then repeatedly every repeatRate ms.
        /// </summary>
        /// <param name="callback">the method</param>
        /// <param name="repeatRate"></param>
        public static void InvokeRepeating(TimeDelegate callback, uint repeatRate)
        {
            var hashCode = callback.GetHashCode();
            
            if (!CanStart(hashCode, callback))
            {
                return;
            }

            if (_buckets == null)
            {
                _buckets = new List<Bucket>(10);
            }

            var now = DateTime.UtcNow;

            _buckets.Add(new Bucket
            {
                Callback = callback,
                DeltaMs = Math.Max(repeatRate, MinPeriodMs),
                StartTime = now,
                NowTime = now, 
                HashCode = hashCode
            });

            if (_systemThreadingTimer == null)
            {
                _cookie = new object();
                _systemThreadingTimer = new System.Threading.Timer(SystemThreadingTimerCallback, _cookie, DueTimeMs, MinPeriodMs);
            }
        }
        
        private static bool CanStart(int hashCode, TimeDelegate callback)
        {
            if (callback == null)
            {
                CantStartBecauseTimerCallbackIsNullException.Print();
                return false;
            }

            if (_unitySynchronizationContext == null)
            {
                CantStartBecauseNullContextException.Print();
                return false;
            }

            if (_unitySynchronizationContext != SynchronizationContext.Current)
            {
                CantStartBecauseNotUnityContextException.Print();
                return false;
            }

            if (_buckets != null && _buckets.Find(t => t.HashCode == hashCode) != null)
            {
                CantStartBecauseTheCallbackAlreadyUsedException.Print();
                return false;
            }

            return true;
        }
        
        private static bool CanReset(int hashCode, TimeDelegate callback, ref int index)
        {
            if (callback == null)
            {
                CantResetBecauseArgumentCannotBeNullException.Print();
                return false;
            }

            if (_unitySynchronizationContext == null || _unitySynchronizationContext != SynchronizationContext.Current)
            {
                CantResetBecauseNotUnityContextException.Print();
                return false;
            }
            
            index = _buckets != null ? _buckets.FindIndex(t => t.HashCode == hashCode) : -1;
            if (index < 0)
            {
                CantResetBecauseNotExistCallbackException.Print();
                return false;
            }

            return true;
        }
        
        public static void CancelInvoke(TimeDelegate callback)
        {
            var hashCode = callback.GetHashCode();
            var index = -1;
            
            if (CanReset(hashCode, callback, ref index))
            {
                _buckets.RemoveAt(index);
                if (_buckets.Count < 1)
                {
                    Clear();
                }
            }
        }
        
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialization()
        {
            _unitySynchronizationContext = SynchronizationContext.Current;

            if (_unitySynchronizationContext == null)
            {
                UnitySynchronizationContextIsNullException.Print();
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

        private static void Clear()
        {
            _cookie = null;
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
        
        private static void SystemThreadingTimerCallback(object state)
        {
            if (state != _cookie)
            {
                return;
            }

            var now = DateTime.UtcNow;

            for (var k = _buckets.Count - 1; k >= 0; k--)
            {
                var timer = _buckets[k];
                var delta = now - timer.NowTime;

                if (delta.TotalMilliseconds >= timer.DeltaMs)
                {
                    timer.NowTime = now;
                    
                    var moment = new Moment
                    {
                        Delta = delta,
                        Passed = now - timer.StartTime
                    };
                    
                    try
                    {
                        var context = _unitySynchronizationContext;
                        
                        if (context == SynchronizationContext.Current)
                        {
                            timer.Callback(moment);
                        }
                        else
                        {
                            context.Post(_ => timer.Callback(moment), state); //async 
                        }
                    }
                    catch
                    {
                        //ignore
                    }
                }
            }
        }
    }
}