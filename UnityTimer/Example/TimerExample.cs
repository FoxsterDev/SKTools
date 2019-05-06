using SKTools.Core.Invoker;
using UnityEngine;
using UnityEngine.UI;

namespace SKTools.Core
{
    [RequireComponent(typeof(Text))]
    public class TimerExample : MonoBehaviour
    {
        [SerializeField]
        private uint _seconds = 60;

        private void Start()
        {
            var label = GetComponent<Text>();
            //SystemInvoker.InvokeRepeating((moment) => { label.text = moment.Passed.TotalSeconds.ToString();},  0,1000);
            Debug.Log(Time.realtimeSinceStartup);
            
            SystemTimer.Invoke(
                (moment) =>
                {
                    label.text = moment.Passed.TotalSeconds.ToString();
                    Debug.Log("!!!" + Time.realtimeSinceStartup);
                }, 5000);
        }
    }
}
