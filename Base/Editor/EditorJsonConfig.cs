using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SKTools.Base.Editor
{
    [System.Serializable]
    public abstract class EditorJsonConfig
    {
        protected EditorJsonConfig()
        {
           Load();
        }
        
        protected EditorJsonConfig(string json)
        {
            EditorJsonUtility.FromJsonOverwrite(json, this);
        }

        protected virtual string RelativeFolder
        {
            get { return "Editor Resources"; }
        }
        
        protected abstract string FileName { get; }
       
        private void Load()
        {
            try
            {
                var filePath = Utility.GetPathRelativeToExecutableCurrentFile(RelativeFolder, FileName);
                if (File.Exists(filePath))
                {
                    EditorJsonUtility.FromJsonOverwrite(File.ReadAllText(filePath), this);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
       
        public virtual void Save(string relativeFolder = "Editor Resources")
        {
            try
            {
                var filePath = Utility.GetPathRelativeToExecutableCurrentFile(relativeFolder, FileName);
                File.WriteAllText(filePath, EditorJsonUtility.ToJson(this, true));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}