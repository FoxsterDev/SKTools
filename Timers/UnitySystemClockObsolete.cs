using System;
using System.Threading;
using System.Timers;
using UnityEngine;
using UnityEngine.Assertions;
using Timer = System.Timers.Timer ;
namespace SKTools.Base
{
  public abstract class UnitySystemClockObsolete : IDisposable
    {
        private const ushort DEFAULT_INTERVAL = 1000;
        private Timer _timer;

        protected TimeSpan _updateInterval;
        protected TimeSpan _timeSpan;

        /// <summary>
        /// This event will be fires on unity main thread
        /// After started, immediately will be fire update , and then every interval
        /// </summary>
        public event Action<TimeSpan> OnUpdated;
        public event Action<TimeSpan> OnStopped;

        internal UnitySystemClockObsolete()
        {
            _timer = new Timer();
            _timer.AutoReset = true;
            _timer.Enabled = false;
            SetInterval(DEFAULT_INTERVAL);
        }

        public bool IsStarted { get; private set; }

        public TimeSpan Value
        {
            get { return _timeSpan; }
        }

        public abstract void OnTimerElapsed();

        public void SetInterval(ushort milliseconds)
        {
            _timer.Interval = milliseconds;
            _updateInterval = TimeSpan.FromMilliseconds(milliseconds);
        }

        public void Stop()
        {
            Assert.IsNotNull(_timer, "Cannot use this timer after realising");
            Assert.IsTrue(IsStarted, "This timer is not started");
            Assert.IsTrue(InUnityContex, "The timer is not thread safe, please call only on unity main thread");

            IsStarted = false;
            _timer.Elapsed -= OnSystemTimerElapsed;
            _timer.Enabled = false;
            SafeInvoke(OnStopped, _timeSpan);
        }

        void IDisposable.Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }

        private void OnSystemTimerElapsed(object sender, ElapsedEventArgs e)
        {
            CheckUnityContext(OnTimerElapsed);
        }

        protected static void SafeInvoke(Action<TimeSpan> action, TimeSpan time)
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

        protected void StartInternal(TimeSpan timeSpan)
        {
            Assert.IsNotNull(_timer, "Cannot use this timer after realising");
            Assert.IsFalse(IsStarted, "This timer is already started.");
            Assert.IsTrue(InUnityContex, "The timer is not thread safe, please call only on unity main thread");

            IsStarted = true;

            _timeSpan = timeSpan;
            _timer.Elapsed += OnSystemTimerElapsed;
            _timer.Enabled = true;
            FireUpdate();
        }

        protected void FireUpdate()
        {
            SafeInvoke(OnUpdated, _timeSpan);
        }

        private void Release()
        {
            if (_timer != null)
            {
                _timer.Elapsed -= OnSystemTimerElapsed;
                _timer.Enabled = false;
                _timer.Dispose();
                _timer = null;
            }
        }

        ~UnitySystemClockObsolete()
        {
            Release();
        }

        #region SynchronizationContext

        private static SynchronizationContext _unitySynchronizationContext;

        protected static bool InUnityContex
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

        private static void CheckUnityContext(Action action)
        {
            if (action == null)
            {
                return;
            }

            try
            {
                if (InUnityContex)
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
