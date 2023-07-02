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
        /// Load from relative folder the asset
        /// </summary>
        public void Load(string relativeDirectory)
        {
            try
            {
                var filePath = Path.Combine(relativeDirectory, FileName);
#if FOXSTER_DEV_MODE
                Debug.Log(filePath);
#endif
                if (File.Exists(filePath))
                {
                    EditorJsonUtility.FromJsonOverwrite(File.ReadAllText(filePath), this);
                }
                else
                {
#if FOXSTER_DEV_MODE
                    Debug.LogError("!" + filePath);
#endif
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
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

                var filePath = Path.Combine(relativeDirectory, FileName);
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