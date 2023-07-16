using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SKTools.Editor
{
    [System.Serializable]
    public abstract class EditorJsonAsset
    {
        protected EditorJsonAsset()
        {
        }

        protected EditorJsonAsset(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                EditorJsonUtility.FromJsonOverwrite(json, this);
            }
        }

        protected virtual string RelativeFolder
        {
            get { return "Editor Resources"; }
        }

        protected abstract string FileName { get; }

        /// <summary>
        ///  Load from relative folder the asset
        /// </summary>
        /// <param name="relativeDirectory">file path to local relative folder Editor Resources</param>
        /// <param name="fileNameJson"> without file extension, will be loaded as file+".json"</param>
        public bool Load(string relativeDirectory, string fileNameJson = default)
        {
            try
            {
                var filePath = Path.Combine(relativeDirectory, fileNameJson != null ? fileNameJson+".json" : FileName+".json");
                Log.Debug(filePath);

                if (File.Exists(filePath))
                {
                    var text = File.ReadAllText(filePath);
                    EditorJsonUtility.FromJsonOverwrite(text, this);
                    return true;
                }
                
                Log.Warning("!" + filePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return false;
        }

        /// <summary>
        /// Save from relative folder the asset
        /// </summary>
        public void Save(string relativeDirectory)
        {
            try
            {
                if (!Directory.Exists(relativeDirectory))
                {
                    Directory.CreateDirectory(relativeDirectory);
                }

                var filePath = Path.Combine(relativeDirectory, FileName+".json");
                
#if FOXSTER_DEV_MODE
                Debug.Log(filePath);
#endif
                File.WriteAllText(filePath, EditorJsonUtility.ToJson(this, true));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}