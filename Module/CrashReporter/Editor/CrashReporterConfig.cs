using System;
using System.IO;
using SKTools.Base.Editor;
using UnityEditor;
using UnityEngine;

namespace SKTools.Module.CrashReporter
{
    [System.Serializable]
    public class CrashReporterConfig
    {
        public CrashReporterConfig Load()
        {
            return this;
        }
    }
}