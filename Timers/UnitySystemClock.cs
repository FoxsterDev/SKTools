using System;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace SKTools.Base
{
    #region Timer

    /// <summary>
    /// Generates an event after a set interval, with an option to generate recurring events
    /// Example of usage
    /*[RequireComponent(typeof(Text))]
    public class TimerExample : MonoBehaviour
    {
        [SerializeField] private uint _seconds = 60;
        private Timer _timer;
    
        void Start ()
        {
            var label = GetComponent<Text>();
            _timer = new Timer(1000);
            _timer.OnIntervalElapsed += delegate(TimeSpan span)
            {
                label.text = (Mathf.CeilToInt((float)_timer.RemainingTime.TotalSeconds)).ToString();
            };
            _timer.StartWithSeconds(_seconds);
        }
        
        //dont forget to dispose it
        void OnDestroy()
        {
            ((IDisposable)_timer).Dispose();
        }
    }*/
    /// </summary>
    public sealed class Timer : UnitySystemClock
    {
        public TimeSpan RemainingTime;
        public TimeSpan StartTime;

        public event Action OnTimesUp;

        public Timer(uint intervalMilliseconds)
            : base(intervalMilliseconds)
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
            SetEnableState();
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
            SetDisableState();
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
                SetDisableState();

                var onTimesUp = OnTimesUp;
                if (onTimesUp == null)
                {
                    return;
                }

                try
                {
                    onTimesUp();
                }
                catch
                {
                    //ignore
                }
            }
        }
    }

    #endregion

    #region StopWatch

    public sealed class StopWatch : UnitySystemClock
    {
        public TimeSpan ElapsedTime;
        
        public bool IsPaused { get; private set; }
        public bool IsStarted { get; private set; }

        
        public void Start()
        {
            Assert.IsFalse(IsStarted, "Timer has already started!");

            IsPaused = false;
            IsStarted = true;
            ElapsedTime = TimeSpan.Zero;
            SetEnableState();
        }
        
        public StopWatch(uint intervalMilliseconds)
            : base(intervalMilliseconds)
        {
        }
        
        public void Pause()
        {
            Assert.IsTrue(IsStarted, "Cannot pause not started timer!");

            IsPaused = true;
        }
        
        public void Resume()
        {
            Assert.IsTrue(IsPaused, "Cannot resume not paused timer!");

            IsPaused = false;
        }

        public void Reset()
        {
            Assert.IsTrue(IsStarted, "Cannot reset not started timer!");

            IsStarted = false;
            IsPaused = false;
            SetDisableState();
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
            ElapsedTime += intervalElapsed;
        }
    }

    #endregion
    
    #region Clock
    
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
        /// It will be fired on UnitySynchronizationContext
        /// </summary>
        public event Action<TimeSpan> OnIntervalElapsed;

        /// <summary>
        /// Create unity friendly system timer
        /// </summary>
        /// <param name="intervalMilliseconds">Sets the interval, expressed in milliseconds, at which to raise the Elapsed event.</param>
        protected UnitySystemClock(uint intervalMilliseconds)
        {
            _interval = Math.Max(intervalMilliseconds, MinIntervalMs); //the minimum system clock interval
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
        protected void SetEnableState()
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
        protected void SetDisableState()
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

            try
            {
                onIntervalElapsed(timeSpan);
            }
            catch
            {
                //ignore
            }
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
            SetDisableState();
        }

        private static SynchronizationContext _unitySynchronizationContext;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void UnitySynchronizationContextCatch()
        {
            _unitySynchronizationContext = SynchronizationContext.Current;
        }
    }
    
    #endregion
}
