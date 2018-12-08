using SKTools.Base.Editor;

namespace SKTools.Module.CrashReporter
{
    [System.Serializable]
    public sealed class CrashReporterConfig : EditorJsonConfig
    {
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