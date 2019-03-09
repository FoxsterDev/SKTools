using System;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEditor;
using Timer = SKTools.Core.Timer;
namespace SKTools.Core.Tests
{
    public class UnitySystemClockTest
    {
        [UnityTest]
        [Timeout(10 * 1000)]
        public IEnumerator TestTimerWithInterval1000ms()
        {
            var timer = CreateTimer(5, 1000);
            yield return new WaitWhile(() => timer.IsStarted);
        }

        [UnityTest]
        [Timeout(10 * 1000)]
        public IEnumerator TestTimerWithInterval100ms()
        {
            var timer = CreateTimer(1, 100);
            yield return new WaitWhile(() => timer.IsStarted);
        }

        [UnityTest]
        [Timeout(10 * 1000)]
        public IEnumerator TestTimerWithInterval15ms()
        {
            var time = EditorApplication.timeSinceStartup;
            var time2 = DateTime.Now;
            var timer = CreateTimer(3, 15);
            yield return new WaitWhile(() => timer.IsStarted);
            UnityEngine.Debug.Log(EditorApplication.timeSinceStartup - time);
            Assert.IsTrue((int)(DateTime.Now - time2).TotalSeconds == 3);
        }
        
        [UnityTest]
        [Timeout(10 * 1000)]
        public IEnumerator TestTimerWithInterval5ms()
        {
            var time2 = DateTime.Now;
            var timer = CreateTimer(1, 5);
            yield return new WaitWhile(() => timer.IsStarted);
            Assert.IsTrue((int)(DateTime.Now - time2).TotalSeconds == 1);
        }

        [UnityTest]
        [Timeout(10 * 1000)]
        public IEnumerator TestTimerWithThreadSleep1000ms()
        {
            var time2 = DateTime.Now;
            var timer = CreateTimer(3, 25);
            Thread.Sleep(1000);
            yield return new WaitWhile(() => timer.IsStarted);
            UnityEngine.Debug.Log((DateTime.Now - time2).TotalSeconds);
            Assert.IsTrue((int)(DateTime.Now - time2).TotalSeconds == 3);
        }

        private Timer CreateTimer(int sec, ushort inter)
        {
            var countUpdated = 0;
            var seconds = sec;
            ushort interval = inter;
            var timer = new Timer(interval);
            var watch = new Stopwatch();
            var id = Thread.CurrentThread.ManagedThreadId;
            var timeSpanFinish = TimeSpan.Zero;

            var list = new System.Collections.Generic.List<double>(seconds*1000/interval);
            timer.OnIntervalElapsed += delegate(TimeSpan timeSpan)
            {
                //UnityEngine.Debug.Log(timeSpan.TotalMilliseconds);
                countUpdated++;
                list.Add(timeSpan.TotalMilliseconds);
                timeSpanFinish += timeSpan;
                Assert.IsTrue(Thread.CurrentThread.ManagedThreadId == id);
            };

            timer.OnTimesUp += delegate
            {
                watch.Stop();
                UnityEngine.Debug.Log(
                    "OnFinished: TotalTime " + timer.StartTime.TotalMilliseconds + " Elapsed watching seconds: " + (int) watch.Elapsed.TotalSeconds +
                    " countUpdate=" + countUpdated + " watch.Elapsed. ms" + watch.Elapsed.TotalMilliseconds +" timeSpanFinish.TotalSeconds:: "+ timeSpanFinish.TotalSeconds );

                UnityEngine.Debug.Log("Min timespan:="+list.Min() +" Max="+list.Max()+" Average="+ list.Average());
                Assert.IsTrue((int) watch.Elapsed.TotalSeconds == seconds);
                Assert.IsTrue((int) timeSpanFinish.TotalSeconds == seconds);
                Assert.IsTrue(Thread.CurrentThread.ManagedThreadId == id);
                Assert.IsTrue(timer.RemainingTime == TimeSpan.Zero);
                Assert.IsFalse(timer.IsStarted);
                Assert.IsFalse(timer.IsPaused);
            };

            watch.Start();
            timer.Start(TimeSpan.FromSeconds(seconds));
            return timer;
        }
    }
}
