using System.Collections.Generic;

namespace SKTools.Editor
{
    [System.Serializable]
    internal class Config : EditorJsonAsset
    {
        public int Version = 1;
        protected override string FileName => typeof(Config).FullName;

        public List<string> Keys = new List<string>(1);
        public List<string> Values = new List<string>(1);
    }
}