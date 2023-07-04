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

            if (!SessionState.GetBool("FirstInitDone", false) || true)
            {
                SessionState.SetBool("FirstInitDone", true);
                
                Debug.Log("First Init.");
         
                //schedule the current package itself
                //might be configured externally
                //SKTools.Editor.EntryPoint .w .Windows.RateMe
                //Idea
                //You are using {the package} already for few month.
                //How much are you happy?
                //Later , Never , Give a star on my Github
                
                //when Later to reschedule
                //Never - to remove item from prefs and leave only the coice

                //resolve config 
                //verify scheduling options
                //EditorPrefs.GetString("SKToolsConfig", "");
                //var config = EditorJsonAsset.LoadFromEditorPrefs<Config>();

                //Debug.Log("config="+config.Version);
               
                //EditorJsonAsset.SaveToEditorPrefs(config);
                
                //var rateMeConfig = new RateMeConfig();
                //RateMe.Show(rateMeConfig);

                var state = Preferences.Load<RateMeState>();

                if (string.IsNullOrEmpty(state.Source))
                {
                    Debug.Log("save first time");
                    state = new RateMeState
                    {
                        Source = "SKTools", 
                        SchedulingTimeUtc = DateTime.UtcNow.AddSeconds(20).ToBinary(),
                        UserChoiceNeverShowAgain = false,
                        Cleared = false
                    };
                    Preferences.Save(state);
                }

                if (DateTime.UtcNow > DateTime.FromBinary(state.SchedulingTimeUtc))
                {
                    Debug.Log("triggered rate me");
                    //state.Cleared = true;
                    Preferences.Delete<RateMeState>();
                    RateMe.Show(null);
                }
            }
        }
    }
    
}