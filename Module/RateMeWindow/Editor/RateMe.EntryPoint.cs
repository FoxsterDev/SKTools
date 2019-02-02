using System;
using SKTools.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace SKTools.Module.RateMeWindow
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

        public static void Show(RateMeConfig config)
        {
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
        [MenuItem("SKTools/Rate Me Test")]
        private static void ShowWindowMenuItem()
        {
            Show(null);
        }

        [MenuItem("SKTools/Rate Me Save Default Config")]
        private static void SaveConfigMenuItem()
        {
            //new RateMeConfig().Save();
        }
        
        [MenuItem("SKTools/Rate Me TestException")]
        private static void TestException()
        {
           throw new Exception("nsnansmnnmn");
        }

#endif
    }
}