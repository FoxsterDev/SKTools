using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace SKTools.Base.Editor
{
    public class AssetsContainer : IAssetsContainer
    {
        private Dictionary<string, Object> AssetsDict { get; set; }

        protected AssetsContainer()
        {
            AssetsDict = new Dictionary<string, Object>();
        }
        
        public Object this[string nameAsset]
        {
            get { return Get<Object>(nameAsset); }
        }
        
        public T Get<T>(string name) where T : Object
        {
            if (!AssetsDict.ContainsKey(name))
            {
                Debug.LogAssertion("Cant get an asset with name=" + name);
                return default(T);
            }

            return (T) AssetsDict[name];
        }

        public void Load(string absoluteDirectoryPath)
        {
            
            try
            {
                var files = Directory.GetFiles(absoluteDirectoryPath, "*.*", SearchOption.AllDirectories);
                
                foreach (var filePath in files)
                {
                    if (Path.GetExtension(filePath) == ".meta")
                        continue;
                    var assetPath = filePath.Substring(Application.dataPath.Length - "Assets".Length);
                    var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                    AssetsDict[asset.name] = asset;
                }
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
            }

        }
    }
}