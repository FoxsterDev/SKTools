using System;

namespace SKTools.Core.Invoker
{
    public struct Moment
    {
        /// <summary>
        /// All time passed since start 
        /// </summary>
        public TimeSpan Passed;
        
        /// <summary>
        /// HAll time passed since the last update 
        /// </summary>
        public TimeSpan Delta;

        public override string ToString()
        {
            return string.Concat("Passed: ", Passed.TotalMilliseconds, "ms ", "Delta: ", Delta.TotalMilliseconds );
        }
    }
}