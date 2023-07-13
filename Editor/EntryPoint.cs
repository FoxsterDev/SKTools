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
            Debug.Log("EntryPoint.");

            if (!SessionState.GetBool("FirstInitDone", false))
            {
                SessionState.SetBool("FirstInitDone", true);
                
                Debug.Log("First Init.");
         
                RateMe.Initialize();
            }
        }
    }
    
}