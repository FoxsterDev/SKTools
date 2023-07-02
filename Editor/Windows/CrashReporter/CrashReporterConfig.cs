using SKTools.Editor;
using UnityEngine;

namespace SKTools.Editor.Windows.CrashReporter
{
    [System.Serializable]
    public sealed class CrashReporterConfig : EditorJsonAsset
    {
        public LogType Type = LogType.Exception;
        public ushort MaxCount = 10;
        public string FileNameLogs = string.Empty;
        public string EmailToSend = string.Empty;
        public string KeysInStackTrace = string.Empty;
        public string AdditionalInfoToSend = string.Empty;
        public bool DontShowAgain = false;
        public ushort Version;
        
        public CrashReporterConfig() : base()
        {
        }

        public CrashReporterConfig(string json) : base(json)
        {
        }

        protected override string FileName
        {
            get { return GetType().Name + ".json"; }
        }
    }
}