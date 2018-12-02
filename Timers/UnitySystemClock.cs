﻿using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace SKTools.Base
{
    #region Timer

    public class Timer : UnitySystemClock
    {
        public TimeSpan RemainingTime;
        public TimeSpan StartTime;

        public event Action OnTimesUp;

        public Timer(uint interval)
            : base(interval)
        {
        }

        public bool IsPaused { get; private set; }
        public bool IsStarted { get; private set; }

        public void StartWithSeconds(uint seconds)
        {
            Start(TimeSpan.FromSeconds(seconds));
        }

        public void StartWithMilliseconds(uint ms)
        {
            Start(TimeSpan.FromMilliseconds(ms));
        }

        public void Start(TimeSpan timeSpan)
        {
            Assert.IsFalse(IsStarted, "Timer has already started!");

            RemainingTime = StartTime = timeSpan;
            IsPaused = false;
            IsStarted = true;
            StartInternal();
        }

        public void Resume()
        {
            Assert.IsTrue(IsPaused, "Cannot resume not paused timer!");

            IsPaused = false;
        }

        public void Pause()
        {
            Assert.IsTrue(IsStarted, "Cannot pause not started timer!");

            IsPaused = true;
        }

        public void Cancel()
        {
            Assert.IsTrue(IsStarted, "Cannot cancel not started timer!");

            IsStarted = false;
            IsPaused = false;
            StopInternal();
        }

        public void Restart()
        {
            Start(StartTime);
        }

        protected override void FireIntervalElapsed(TimeSpan intervalElapsed)
        {
            if (!IsStarted)
            {
                return;
            }

            if (IsPaused)
            {
                return;
            }

            base.FireIntervalElapsed(intervalElapsed);

            RemainingTime -= intervalElapsed;
            if (RemainingTime <= TimeSpan.Zero)
            {
                RemainingTime = TimeSpan.Zero;
                IsStarted = false;
                StopInternal();

                if (OnTimesUp != null)
                {
                    OnTimesUp.Invoke();
                }
            }
        }
    }

    #endregion

    #region StopWatch

    public class StopWatch : UnitySystemClock
    {
        public StopWatch(uint interval)
            : base(interval)
        {
        }

        public void StartStopWatch()
        {
            throw new NotImplementedException();
        }

        public void ResumeStopWatch()
        {
            throw new NotImplementedException();
        }

        public void ResetStopWatch()
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    public abstract class UnitySystemClock : IDisposable
    {
        private const uint MinIntervalMs = 1;
        private readonly TimerCallback _callback;

        private uint _interval;
        private bool _enabled;
        private bool _disposed;
        private object _cookie;
        private System.Threading.Timer _timer;
        private DateTime _previousDateTime;

        /// <summary>
        /// Will be fired on UnitySynchronizationContext
        /// </summary>
        public event Action<TimeSpan> OnIntervalElapsed;

        /// <summary>
        /// Create unity friendly system timer
        /// </summary>
        /// <param name="interval">Sets the interval, expressed in milliseconds, at which to raise the Elapsed event.</param>
        protected UnitySystemClock(uint interval)
        {
            _interval = Math.Max(interval, MinIntervalMs); //the minimum system clock interval
            _enabled = false;
            _callback = TimerCallback;
        }

        /// <summary>
        /// Gets or sets the interval, expressed in milliseconds, at which to raise the Elapsed event.
        /// </summary>
        public uint Interval
        {
            get { return _interval; }
            set
            {
                _interval = Math.Max(value, MinIntervalMs);
                if (_timer == null)
                {
                    return;
                }

                _timer.Change(_interval, _interval);
            }
        }

        /// <summary>
        /// Start threading timer
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        protected void StartInternal()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (_enabled)
            {
                return;
            }

            _enabled = true;

            if (_timer == null)
            {
                _cookie = new object();
                _previousDateTime = DateTime.Now;
                _timer = new System.Threading.Timer(_callback, _cookie, _interval, _interval);
            }
            else
            {
                _timer.Change( _interval, _interval);
            }
        }

        /// <summary>
        /// Stop the current timer
        /// </summary>
        protected void StopInternal()
        {
            _enabled = false;

            if (_timer != null)
            {
                _cookie = null;
                _timer.Dispose();
                _timer = null;
            }
        }

        private void TimerCallback(object state)
        {
            if (state != _cookie)
            {
                return;
            }

            try
            {
                var context = _unitySynchronizationContext;
                if (context == null || context == SynchronizationContext.Current)
                {
                    FireIntervalElapsed();
                }
                else
                {
                    context.Post(_ => FireIntervalElapsed(), state); //async 
                }
            }
            catch
            {
                // ignored
            }
        }

        private void FireIntervalElapsed()
        {
            var now = DateTime.Now;

            if (now > _previousDateTime)
            {
                var interval = now - _previousDateTime;
                _previousDateTime = now;
                FireIntervalElapsed(interval);
            }
        }

        protected virtual void FireIntervalElapsed(TimeSpan timeSpan)
        {
            var onIntervalElapsed = OnIntervalElapsed;
            if (onIntervalElapsed == null)
            {
                return;
            }

            onIntervalElapsed(timeSpan);
        }

        ~UnitySystemClock()
        {
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
            StopInternal();
        }

        private static SynchronizationContext _unitySynchronizationContext;

        public static SynchronizationContext UnitySynchronizationContext
        {
            get { return _unitySynchronizationContext; }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void UnitySynchronizationContextCatch()
        {
            _unitySynchronizationContext = SynchronizationContext.Current;
        }
    }
}
