using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

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

        [Conditional(Const.FOXSTER_DEV_MODE)]
        private static void PrintDebugInformation()
        {
            Log.Debug(Preferences.FullRawJson());
        }

        [Conditional(Const.FOXSTER_DEV_MODE)]
        internal static void InitializeDebug()
        {
            PrintDebugInformation();
            Preferences.DeleteRoot();
            Log.Debug("ListOfRateMeState entry is deleted ");
            Initialize();
        }

        internal static void Initialize()
        {
            var states = TriggerRateMeWindowByTime();

            //to avoid loading config if it is already scheduled
            const string defaultItselfSource = "SKTools";

            if (states.List.Find(state => state.Source == defaultItselfSource) == null)
            {
                //self registration  
                var assetsDirectory = Utility.GetPathRelativeToCurrentDirectory("Editor Resources");

                var config = new RateMeConfig();
                var isLoaded = config.Load(assetsDirectory);
                config.Source = defaultItselfSource;

                Log.Debug($"Config is loaded {isLoaded} with source {config.Source}");

                RateMe.Schedule(config);
            }
        }

        private static ListOfRateMeStates TriggerRateMeWindowByTime()
        {
            var states = Preferences.Load<ListOfRateMeStates>();

            foreach (var state in states.List)
            {
                if (state?.Config == null || state.Cleared)
                {
                    continue;
                }

                var seconds = (DateTime.FromBinary(state.SchedulingTimeUtc) - DateTime.UtcNow).TotalSeconds;
                //present only one for one editor session
                if (seconds < 0)
                {
                    state.Cleared = true;
                    Preferences.Save(states);
                    RateMe.Show(state.Config);
                    break;
                }

                Log.Debug($"RateMe {state.Source} will be presented in {seconds} s");
            }

            return states;
        }

        public static void Schedule(RateMeConfig config)
        {
            FailIfConfigNotValid(config);
            //check if it is already cleared
            var states = Preferences.Load<ListOfRateMeStates>();
           
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
            
            var state = new RateMeState
            {
                Source = config.Source, 
                SchedulingTimeUtc = DateTime.UtcNow.AddSeconds(config.DisplayInSeconds).ToBinary(),
                UserSelectedNeverShowAgain =  false,
                Cleared = false,
                Config = config
            };
            states.List.Add(state);
            var json = Preferences.Save(states);
            
            Log.Debug(json);
        }

        private static void FailIfConfigNotValid(RateMeConfig config)
        {
            Assert.IsNotNull(config);
            //Assert.IsNotNull(config.Source);
        }
        
        public static void MarkAsUsed()
        {
            
        }

        public static void Show(RateMeConfig config)
        {
            Log.Debug("Show rate me");
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
        
        [MenuItem("SKTools/DevMode/Rate Me Trigger By Time")]
        private static void RateMeTriggerByTimeMenuItem()
        {
            TriggerRateMeWindowByTime();
        }
        
        [MenuItem("SKTools/DevMode/Rate Me  Clean All Scheduling")]
        private static void CleanInternalScheduling()
        {
            var deleted = Preferences.Delete<ListOfRateMeStates>();
            Log.Debug("ListOfRateMeState Cleaned "+ deleted);
        }
        
        [MenuItem("SKTools/DevMode/Rate Me Test")]
        private static void ShowWindowMenuItem()
        {
            Show(null);
        }

        [MenuItem("SKTools/DevMode/Rate Me Save Default Config")]
        private static void SaveConfigMenuItem()
        {
            var assetsDirectory = Utility.GetPathRelativeToCurrentDirectory("Editor Resources");

            var config = new RateMeConfig();
            var isLoaded = config.Load(assetsDirectory);
            config.Source = "SKTools";
            config.DisplayInSeconds = 30;
            config.MaxDisplayCount = 3;
            config.SchedulingEnabled = true;
            config.ShowOnEditorStartUp = true;
            config.TryAgainInSeconds = 180;
            
            config.Save(assetsDirectory);
        }
        
        [MenuItem("SKTools/DevMode/Rate Me TestException")]
        private static void TestException()
        {
           throw new Exception("nsnansmnnmn");
        }

#endif
    }
}