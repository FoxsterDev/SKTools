using System;
using SKTools.Base.Editor;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SKTools.Module.CrashReporter
{
    public partial class CrashReporter
    {
        [InitializeOnLoadMethod]
        private static void CrashReporter_CheckReload()
        {
            Application.logMessageReceived += GetCrashReporter().Application_LogMessageReceived;
            GetCrashReporter().SetUpWindow(false);
        }

        private void SetUpWindow(bool createIfNotExist)
        {
            var container = CustomEditorWindow<Window>.GetWindow(createIfNotExist);
            if (container == null) return;

            var assetsDirectory = Utility.GetPathRelativeToExecutableCurrentFile("Editor Resources");
            var assets = new Assets(assetsDirectory);

            Utility.DiagnosticRun(assets.Load);

            _targetGui = new Surrogate<IGUIContainer, Assets>(container, assets);
            _targetGui.Container.DrawGuiCallback = DrawCrashReporterGui;

            if (createIfNotExist)
            {
                _targetGui.Container.Show();
            }
        }

        private static void Show(CrashReporterConfig config)
        {
            var instance = GetCrashReporter();
            instance._config = config;
            instance.SetUpWindow(true);
        }

#if FOXSTER_DEV_MODE
        
        [MenuItem("SKTools/CrashReporter Throw Exception")]
        private static void ShowWindowMenuItem()
        {
            throw new Exception("test exception "+ Random.value);
        }
        
#endif
        
    }
}