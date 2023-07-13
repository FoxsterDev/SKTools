using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[assembly: InternalsVisibleTo("SKTools.Editor")]
namespace SKTools.Editor.Windows.RateMe
{
    public partial class RateMe
    {
        private Surrogate<IGUIContainer, Assets> _targetGui;
        private RateMeConfig _config;

        private static RateMe _instance;

        private static RateMe GetRateMe()
        {
            return _instance ?? (_instance = new RateMe());
        }

        internal static void Initialize()
        {
            var states = Preferences.Load<ListOfRateMeState>();
          
            //1 config for 1 display or 1 source for 1 display
            //only 1 display per editor launch
            //skip batch mode
            
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

            
            var state = states.List.Find(s => !s.Cleared);

            if (state != null && DateTime.UtcNow > DateTime.FromBinary(state.SchedulingTimeUtc))
            {
                state.Cleared = true;
                Preferences.Save(states);
                RateMe.Show(state.Config);
            }
            
            //RateMe.Schedule(null);
        }

        public static void Schedule(RateMeConfig config)
        {
            FailIfConfigNotValid(config);
            //check if it is already cleared
            var states = Preferences.Load<ListOfRateMeState>();
           
            var exist = states.List.Find(r => r.Source == config.Source);

            if (exist != null)
            {
                if (exist.Cleared)
                {
                    return;
                }
                else// scheduled
                {
                    //update some parameters ?
                }
            }
            //to add new one
            Debug.Log("save first time");
            var state = new RateMeState
            {
                Source = config.Source, 
                SchedulingTimeUtc = DateTime.UtcNow.AddSeconds(config.DisplayInSeconds).ToBinary(),
                UserSelectedNeverShowAgain =  false,
                Cleared = false,
                Config = config
            };
            states.List.Add(state);
            Preferences.Save(states);
        }


        private static void FailIfConfigNotValid(RateMeConfig config)
        {
            Assert.IsNotNull(config);
            Assert.IsNotNull(config.Source);
        }
        public static void MarkAsUsed()
        {
            
        }

        public static void Show(RateMeConfig config)
        {
            Debug.Log("Show rate me");
            var instance = GetRateMe();
            instance._config = config;
            instance.SetUpWindow(true);
        }

        private RateMeConfig Config
        {
            get { return _config ?? (_config = new RateMeConfig()); }
        }

        [InitializeOnLoadMethod]
        private static void RateMeWindow_CheckReload()
        {
            GetRateMe().SetUpWindow(false);
        }

        private void SetUpWindow(bool createIfNotExist)
        {
            var container = CustomEditorWindow<Configuration>.GetWindow(createIfNotExist);
            if (container == null) return;

            _feedbackMessage = Config.FeedbackMessage;
            _starRects = new Rect[Config.MaxStar];

            var assetsDirectory = Utility.GetPathRelativeToCurrentDirectory("Editor Resources");
            var assets = new Assets(assetsDirectory);

            Utility.DiagnosticRun(assets.Load);

            _targetGui = new Surrogate<IGUIContainer, Assets>(container, assets);
            _targetGui.Container.DrawGuiCallback = DrawRateGui;

            if (createIfNotExist)
            {
                _targetGui.Container.Show();
            }


            SetCountStar(1);
        }

#if FOXSTER_DEV_MODE
        [MenuItem("SKTools/DevMode/Rate Me  Clean All Scheduling")]
        private static void CleanInternalScheduling()
        {
            var deleted = Preferences.Delete<ListOfRateMeState>();
            Debug.Log("ListOfRateMeState Cleaned "+ deleted);
        }
        
        [MenuItem("SKTools/DevMode/Rate Me Test")]
        private static void ShowWindowMenuItem()
        {
            Show(null);
        }

        [MenuItem("SKTools/DevMode/Rate Me Save Default Config")]
        private static void SaveConfigMenuItem()
        {
            //new RateMeConfig().Save();
        }
        
        [MenuItem("SKTools/DevMode/Rate Me TestException")]
        private static void TestException()
        {
           throw new Exception("nsnansmnnmn");
        }

#endif
    }
}