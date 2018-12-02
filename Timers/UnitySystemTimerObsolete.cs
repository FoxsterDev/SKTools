using System;

namespace SKTools.Base
{
    public class UnitySystemTimerObsolete : UnitySystemClockObsolete
    {
        /// <summary>
        /// This call is synchronized with unity main thread
        /// </summary>
        public override void OnTimerElapsed()
        {
            if (_timeSpan >= _updateInterval)
            {
                _timeSpan = _timeSpan.Subtract(_updateInterval);
                 FireUpdate();
            }

            if (_timeSpan < _updateInterval)
            {
                Stop();
            }
        }

        public void StartWithSeconds(int totalSeconds)
        {
            StartInternal(TimeSpan.FromSeconds(totalSeconds));
        }
    }
}
