using System;
using System.Threading;
using System.Timers;
using UnityEngine;
using UnityEngine.Assertions;
using Timer = System.Timers.Timer;

namespace SKTools.Base
{
    public class UnitySystemTimer : IDisposable
    {
        private const ushort DEFAULT_INTERVAL = 1000;
        private TimeSpan _updateInterval;
        private Timer _systemTimer;
        private TimeSpan _timeSpan;
        private bool _isCooldownMode;

        public event Action<TimeSpan> OnUpdated;
        public event Action<TimeSpan> OnFinished;

        /// <summary> 
        /// Wrapper of System.Timers.Timer, DEFAULT_INTERVAL = 1000ms
        /// </summary>
        public UnitySystemTimer()
        {
            _updateInterval = TimeSpan.FromMilliseconds(DEFAULT_INTERVAL);
            _systemTimer = new Timer { Interval = DEFAULT_INTERVAL };
            _systemTimer.AutoReset = true;
            _systemTimer.Enabled = false;
        }

        public bool IsStarted { get; private set; }

        public TimeSpan Value
        {
            get { return _timeSpan; }
        }

        public void SetInterval(ushort intervalMs)
        {
            _systemTimer.Interval = intervalMs;
            _updateInterval = TimeSpan.FromMilliseconds(DEFAULT_INTERVAL);
        }

        public void Start()
        {
            StartInternal(TimeSpan.Zero, false);
        }

        public void StartCooldownWithSeconds(int seconds)
        {
            StartInternal(TimeSpan.FromSeconds(seconds), true);
        }

        private void StartInternal(TimeSpan timeSpan, bool isCoolDown)
        {
            Assert.IsNotNull(_systemTimer, "Cannot use timer after realising");
            Assert.IsFalse(IsStarted, "This timer is already started.");
            Assert.IsTrue(IsUnityContex, "The unitysystemtimer is not thread safe, please call only on unity main thread");

            IsStarted = true;
            _isCooldownMode = isCoolDown;
            _timeSpan = timeSpan;
            _systemTimer.Elapsed += OnSystemTimerElapsed;
            _systemTimer.Enabled = true;
            SafeInvoke(OnUpdated, _timeSpan);
        }

        public void Stop()
        {
            Assert.IsNotNull(_systemTimer, "Cannot use timer after realising");
            Assert.IsTrue(IsStarted, "This timer is not started");
            Assert.IsTrue(IsUnityContex, "The unitysystemtimer is not thread safe, please call only on unity main thread");

            IsStarted = false;
            _systemTimer.Elapsed -= OnSystemTimerElapsed;
            _systemTimer.Enabled = false;
            SafeInvoke(OnFinished, _timeSpan);
        }

        public void Release()
        {
            if (_systemTimer != null)
            {
                _systemTimer.Elapsed -= OnSystemTimerElapsed;
                _systemTimer.Enabled = false;
                _systemTimer.Dispose();
                _systemTimer = null;
            }
        }

        void IDisposable.Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }

        private void OnSystemTimerElapsed(object sender, ElapsedEventArgs e)
        {
            CheckInvokationUnityContext(TimeUpdateOnUnityMainThread);
        }

        /// <summary>
        /// This call is synchronized with unity main thread
        /// </summary>
        private void TimeUpdateOnUnityMainThread()
        {
            if (_isCooldownMode)
            {
                if (_timeSpan >= _updateInterval)
                {
                    _timeSpan = _timeSpan.Subtract(_updateInterval);
                    SafeInvoke(OnUpdated, _timeSpan);
                }

                if (_timeSpan < _updateInterval)
                {
                    Stop();
                }
            }
            else
            {
                _timeSpan = _timeSpan.Add(_updateInterval);
                SafeInvoke(OnUpdated, _timeSpan);
            }
        }

        private static void SafeInvoke(Action<TimeSpan> action, TimeSpan time)
        {
            if (action == null)
            {
                return;
            }

            try
            {
                action(time);
            }
            catch
            {
                // ignored
            }
        }

        ~UnitySystemTimer()
        {
            Release();
        }

        #region SynchronizationContext

        private static SynchronizationContext _unitySynchronizationContext;

        private static bool IsUnityContex
        {
            get { return _unitySynchronizationContext == SynchronizationContext.Current; }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void UnitySynchronizationContextCatch()
        {
            if (_unitySynchronizationContext == null)
            {
                _unitySynchronizationContext = SynchronizationContext.Current;
            }
        }

        private static void CheckInvokationUnityContext(Action action)
        {
            if (action == null)
            {
                return;
            }

            try
            {
                if (IsUnityContex)
                {
                    action();
                }
                else
                {
                    _unitySynchronizationContext.Post(_ => action(), null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        #endregion
    }
}
