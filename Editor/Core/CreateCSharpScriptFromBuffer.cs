using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SKTools.Editor
{
    public static class CreateCSharpScriptFromBuffer
    {
        private const string MenuAssetPath = "Assets/Create/";
        private const int Priority = 80;

        [MenuItem(MenuAssetPath + "C# Script From Buffer %v", false, Priority + 1)]
        private static void ScriptFromBuffer()
        {
            var fileName = GetFileNameFromBuffer();
            if (string.IsNullOrEmpty(fileName))
                return;
            var guids = Selection.assetGUIDs;
            var path = string.Empty;
            if (guids.Length == 1)
            {
                path = AssetDatabase.GUIDToAssetPath(guids[0]);
                if (File.Exists(path)) path = Path.GetDirectoryName(path);

                path = path.Replace("Assets/", string.Empty);
            }

            var assetPath = Path.Combine(Application.dataPath, Path.Combine(path, string.Concat(fileName, ".cs")));
            File.WriteAllText(assetPath, EditorGUIUtility.systemCopyBuffer, Encoding.UTF8);
            AssetDatabase.Refresh();
        }

        [MenuItem(MenuAssetPath + "C# Script From Buffer %v", true, Priority + 1)]
        private static bool ScriptFromBufferValidation()
        {
            return !string.IsNullOrEmpty(GetFileNameFromBuffer());
        }

        private static string GetFileNameFromBuffer()
        {
            var fileName = string.Empty;
            var content = EditorGUIUtility.systemCopyBuffer;
            if (!string.IsNullOrEmpty(content))
                try
                {
                    var index1 = content.IndexOf("class", StringComparison.Ordinal);
                    if (index1 > -1)
                    {
                        index1 += "class".Length;
                        
                        var index2 = content.IndexOf('{', 0);
                        if (index2 > -1)
                        {
                            var index3 = content.IndexOf(':', index1);
                            if (index3 > -1 && index3 < index2) index2 = index3;
                            fileName = content.Substring(index1, index2 - index1);
                            
                            var invalidChars = Path.GetInvalidFileNameChars().Concat(new[] {' ', '\n'}).ToArray();
                            fileName = string.Join(string.Empty, fileName.Split(invalidChars));
                        }
                    }
                    
                    //clean 
                    fileName = fileName.Replace("<T>", "");
                }
                catch
                {
                    fileName = string.Empty;
                }

            return fileName;
        }
    }
}