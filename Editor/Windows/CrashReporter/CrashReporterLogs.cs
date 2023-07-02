using System;
using System.Collections.Generic;
using SKTools.Editor;
using UnityEngine;

namespace SKTools.Editor.Windows.CrashReporter
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
            public LogType Type;
            public ushort Count;

            public override string ToString()
            {
                return string.Format(Type + "[Count={0}] Condition={1} \nStacktrace:\n{2}\n\n", Count, Condition, Stacktrace);
            }

            public bool Equals(Line other)
            {
                return other != null && (String.Compare(Condition, other.Condition, StringComparison.Ordinal) == 0 && 
                                         String.Compare(Stacktrace, other.Stacktrace, StringComparison.Ordinal) == 0 && 
                                         Type == other.Type);
            }
        }
    }
}