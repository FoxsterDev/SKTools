using System;
using SKTools.Editor.Windows.RateMe;
using UnityEditor;
using UnityEngine;

namespace SKTools.Editor
{
    [InitializeOnLoad]
    internal class EntryPoint
    {
        static EntryPoint()
        {
            Editor.Log.Info("EntryPoint.");

            if (!SessionState.GetBool("FirstInitDone", false))
            {
                SessionState.SetBool("FirstInitDone", true);
                
                RateMe.Initialize();
            }
            else
            {
                RateMe.InitializeDebug();
            }
        }
    }
}