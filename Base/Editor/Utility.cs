using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SKTools.Base.Editor
{
    public class Utility
    {
        /// <summary>
        /// In development mode I want to measure time of some methods to avoid abuse compilation other systems
        /// </summary>
        /// <param name="method">Method what should be runned</param>
        public static void DiagnosticRun(Action method)
        {
            
#if !FOXSTER_DEV_MODE
            method();
#else
            var watch = new Stopwatch();
            watch.Start();

            method();

            watch.Stop();
            Debug.LogFormat("DiagnosticRun {0} takes={1}", method.Method.Name, watch.ElapsedMilliseconds + "ms");
#endif
        }

        public static void OpenFile(string filePath)
        {
#if !UNITY_EDITOR_WIN
            filePath = "file://" + filePath.Replace(@"\", "/");
#else
            filePath = @"file:\\" + filePath.Replace("/", @"\");;
#endif
            Application.OpenURL(filePath);
        }
        
        public static string GetPathRelativeToExecutableCurrentFile(params string[] subName)
        {
            var path = GetDirectoryRelativeToExecutableCurrentFile(2);
            foreach (var name in subName)
            {
                path = Path.Combine(path, name);
            }
            return path;
        }
        
        /// <summary>
        /// Get directory of place where was called this method, simple way to detect places scripts. To avoid hardcoded pathes
        /// and search in the full project
        /// </summary>
        /// <returns></returns>
        public static string GetDirectoryRelativeToExecutableCurrentFile(int frameIndex = 1)
        {
            var stackTrace = new StackTrace(true);
            return new FileInfo(stackTrace.GetFrames()[frameIndex].GetFileName()).DirectoryName;
        }
    }
}