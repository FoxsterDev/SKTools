using System;
using SKTools.Core.Invoker;
using UnityEngine;
using UnityEngine.UI;

namespace SKTools.Core
{
    /// <summary>
    /// Generates an event after a set interval, with an option to generate recurring events
    /// Example of usage
    [RequireComponent(typeof(Text))]
    public class TimerExample : MonoBehaviour
    {
        [SerializeField] private uint _seconds = 60;
        private Timer _timer;
    
        void Start ()
        {
            var label = GetComponent<Text>();
            SystemInvoker.InvokeRepeating(value => { label.text = value.Passed.ToString(); }, 1000 );
           // UnityStopWatch.Start(Callback,1000);
        }

        private void Callback(Moment value)
        {
            throw new NotImplementedException();
        }
    }
}