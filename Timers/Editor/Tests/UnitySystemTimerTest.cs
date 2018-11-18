using System;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Diagnostics;

namespace SKTools.Base.Tests
{
    public class UnityTimerTest
    {
        [UnityTest]
        [Timeout(10 * 1000)]
        public IEnumerator TestCoolDownWhenInterval1000ms()
        {
            var countUpdated = 0;
            var seconds = 5;
            ushort interval = 1000;
            var timer = new UnitySystemTimer();
            timer.SetInterval(interval);
            var watch = new Stopwatch();

            timer.OnUpdated += delegate(TimeSpan time)
            {
                UnityEngine.Debug.Log(time);
                countUpdated++;
            };

            timer.OnFinished += delegate(TimeSpan time)
            {
                watch.Stop();
                Assert.IsTrue((int) watch.Elapsed.TotalSeconds == seconds);
                Assert.IsTrue(seconds * 1000 / interval + 1 == countUpdated);
                UnityEngine.Debug.Log(
                    "OnFinished: " + timer.Value + " Elapsed watching: " + (int) watch.Elapsed.TotalSeconds +
                    " countUpdate=" + countUpdated + " watch.Elapsed." + watch.Elapsed.TotalMilliseconds);
            };

            watch.Start();
            timer.StartCooldownWithSeconds(seconds);
            yield return new WaitWhile(() => timer.IsStarted);
        }

        [UnityTest]
        [Timeout(10 * 1000)]
        public IEnumerator TestCoolDownWhenInterval100ms()
        {
            var countUpdated = 0;
            var seconds = 2;
            ushort interval = 100;
            var timer = new UnitySystemTimer();
            timer.SetInterval(interval);
            var watch = new Stopwatch();

            timer.OnUpdated += delegate(TimeSpan time)
            {
                UnityEngine.Debug.Log(time);
                countUpdated++;
            };

            timer.OnFinished += delegate(TimeSpan time)
            {
                watch.Stop();
                Assert.IsTrue((int) watch.Elapsed.TotalSeconds == seconds);
                Assert.IsTrue(seconds * 1000 / interval + 1 == countUpdated);
                UnityEngine.Debug.Log(
                    "OnFinished: " + timer.Value + " Elapsed watching: " + (int) watch.Elapsed.TotalSeconds +
                    " countUpdate=" + countUpdated + " watch.Elapsed." + watch.Elapsed.TotalMilliseconds);
            };

            watch.Start();
            timer.StartCooldownWithSeconds(seconds);
            yield return new WaitWhile(() => timer.IsStarted);
        }
        
        [UnityTest]
        [Timeout(10 * 1000)]
        public IEnumerator TestWhenInterval1000ms()
        {
            var countUpdated = 0;
            var seconds = 5;
            ushort interval = 1000;
            var timer = new UnitySystemTimer();
            timer.SetInterval(interval);
            var watch = new Stopwatch();

            timer.OnUpdated += delegate(TimeSpan time)
            {
                UnityEngine.Debug.Log(time);
                countUpdated++;
                if (countUpdated == seconds + 1)
                {
                    timer.Stop();
                    Assert.IsTrue((int) timer.Value.TotalSeconds == seconds);
                }
            };

            watch.Start();
            timer.Start();
            yield return new WaitWhile(() => timer.IsStarted);
        }
    }
}
