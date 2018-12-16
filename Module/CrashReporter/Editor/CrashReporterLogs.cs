using System;
using System.Collections.Generic;
using SKTools.Base.Editor;
using UnityEngine;

namespace SKTools.Module.CrashReporter
{
    [System.Serializable]
    public class CrashReporterLogs : EditorJsonAsset
    {
        public CrashReporterConfig Config;
        public List<Line> Lines = new List<Line>(5);
        private string _fileName;

        protected override string FileName
        {
            get { return _fileName; }
        }

        public CrashReporterLogs(string fileName) : base()
        {
            _fileName = fileName + ".json";
            Debug.Log(_fileName);
        }
        
        [System.Serializable]
        public class Line : IEquatable<Line>
        {
            public string Condition;
            public string Stacktrace;
            public LogType type;
            public ushort Count;

            public bool Equals(Line other)
            {
                return string.Equals(Condition, other.Condition) && string.Equals(Stacktrace, other.Stacktrace) && type == other.type;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Line && Equals((Line) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (Condition != null ? Condition.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Stacktrace != null ? Stacktrace.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (int) type;
                    hashCode = (hashCode * 397) ^ Count.GetHashCode();
                    return hashCode;
                }
            }
        }

    }
}