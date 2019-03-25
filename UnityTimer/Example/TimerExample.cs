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
            SystemInvoker.InvokeRepeating(null,  0,1000);
        }
    }
}
