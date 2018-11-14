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
        private string _assetsDirectory;

        protected AssetsContainer(string assetsDirectory)
        {
            AssetsDict = new Dictionary<string, Object>();
            _assetsDirectory = assetsDirectory;
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

        public void Load()
        {
            try
            {
                var files = Directory.GetFiles(_assetsDirectory, "*.*", SearchOption.AllDirectories);
                var startIndex = Application.dataPath.Length - "Assets".Length;

#if FOXSTER_DEV_MODE
           Debug.Log("[AssetsContainer] load from "+ _assetsDirectory);
#endif
                foreach (var filePath in files)
                {
                    if (Path.GetExtension(filePath) == ".meta")
                        continue;
                    var assetPath = filePath.Substring(startIndex);
                    var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                    if (asset == null)
                        continue;
                    AssetsDict[asset.name] = asset;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }
}