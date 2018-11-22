using System;
using System.Threading;
using UnityEngine;

namespace SKTools.Base
{
    public class UnitySystemClock : IDisposable
    {
        private static SynchronizationContext _unitySynchronizationContext;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void UnitySynchronizationContextCatch()
        {
            _unitySynchronizationContext = SynchronizationContext.Current;
        }

        public event Action<object, DateTime> OnIntervalElapsed;

        private uint _interval;
        private bool _enabled;
        private SynchronizationContext _synchronizationContext;
        private bool _autoReset;
        private bool _disposed;
        private Timer _timer;
        private readonly TimerCallback _callback;
        private object _cookie;

        /// <summary>
        /// Gets or sets the interval, expressed in milliseconds, at which to raise the Elapsed event.
        /// </summary>
        public uint Interval
        {
            get { return _interval; }
            set
            {
                _interval = Math.Max(value, 15);
                if (_timer == null)
                    return;

                Update();
            }
        }

        /// <summary>
        /// Gets or sets a Boolean indicating whether the Timer should raise the Elapsed event only once (false) or repeatedly (true).
        /// </summary>
        private bool AutoReset
        {
            get { return _autoReset; }
            set
            {
                if (_autoReset == value)
                    return;

                _autoReset = value;
                if (_timer == null)
                    return;

                Update();
            }
        }

        public SynchronizationContext SynchronizationContext
        {
            get { return _synchronizationContext; }
            set { _synchronizationContext = value; }
        }

        /// <summary>
        /// Create unity friendly system timer
        /// </summary>
        /// <param name="interval">Sets the interval, expressed in milliseconds, at which to raise the Elapsed event.</param>
        /// <param name="autoReset"><Sets a Boolean indicating whether the Timer should raise the Elapsed event only once (false) or repeatedly (true)./param>
        /// <param name="synchronizationContext">Sync context for callback OnIntervalElapsed. Default is unity context</param>
        public UnitySystemClock(uint interval, bool autoReset = true, SynchronizationContext synchronizationContext = null)
        {
            _interval = Math.Max(interval, 15); //the minimum system clock interval
            _enabled = false;
            _autoReset = autoReset;
            _callback = TimerCallback;
            SynchronizationContext = synchronizationContext;
        }

        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            if (_enabled)
                return;

            _enabled = true;

            if (_timer == null)
            {
                _cookie = new object();
                _timer = new Timer(
                    _callback, _cookie, _interval, _autoReset
                                                       ? _interval
                                                       : 0);
            }
            else
            {
                Update();
            }
        }

        public void Stop()
        {
            _enabled = false;

            if (_timer != null)
            {
                _cookie = null;
                _timer.Dispose();
                _timer = null;
            }
        }

        private void Update()
        {
            _timer.Change(
                _interval, _autoReset
                               ? _interval
                               : 0);
        }


        private void TimerCallback(object state)
        {
            if (state != _cookie)
                return;

            if (!_autoReset)
            {
                _enabled = false;
            }

            try
            {
                var onIntervalElapsed = OnIntervalElapsed;
                if (onIntervalElapsed == null)
                    return;

                var context = SynchronizationContext ?? _unitySynchronizationContext;
                if (context == SynchronizationContext.Current)
                {
                    onIntervalElapsed(this, DateTime.Now);
                }
                else
                {
                    context.Post(_ => onIntervalElapsed(this, DateTime.Now), state);
                }
            }
            catch
            {
                // ignored
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitySystemClock()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            Stop();
            _disposed = true;
        }

        #region Timers

        public event Action<object, uint, uint> OnTimerUpdated; //uint timer, total seconds, left seconds

        private DateTime _timeTarget;
        private uint _totalSeconds;

        public void StartTimerWithSeconds(uint totalSeconds)
        {
            //var timeSpan = TimeSpan.FromSeconds(totalSeconds);
            _totalSeconds = totalSeconds;
            _timeTarget = DateTime.Now.AddSeconds(totalSeconds);

            Start();
            if (OnTimerUpdated != null)
            {
                OnTimerUpdated(this, totalSeconds, totalSeconds);
            }

            //StartInternal(TimeSpan.FromSeconds(totalSeconds));
        }

        #endregion
    }
}
