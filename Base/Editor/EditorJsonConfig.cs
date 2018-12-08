using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SKTools.Base.Editor
{
    [System.Serializable]
    public abstract class EditorConfig
    {
        protected EditorConfig()
        {
            
        }
        
        protected EditorConfig(string json)
        {
            EditorJsonUtility.FromJsonOverwrite(json, this);
        }

        protected virtual string FileName
        {
            get { return GetType().Name + ".json"; }
        }
        
        public virtual T Load<T>(string relativeFolder = "Editor Resources") where T : EditorConfig, new()
        {
            try
            {
                var filePath = Utility.GetPathRelativeToExecutableCurrentFile(relativeFolder, FileName);
                if (File.Exists(filePath))
                {
                    EditorJsonUtility.FromJsonOverwrite(File.ReadAllText(filePath), this);
                    return (T)this;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            return new T();
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