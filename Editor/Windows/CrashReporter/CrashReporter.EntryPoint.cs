﻿using System;
using SKTools.Editor;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SKTools.Editor.Windows.CrashReporter
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
            if (Configs == null || Configs.Count < 1)
                return;
            
            var container = CustomEditorWindow<Configuration>.GetWindow(createIfNotExist);
            if (container == null) return;

            var assets = new Assets(_assetsDirectory);
            Utility.DiagnosticRun(assets.Load);

            _targetGui = new Surrogate<IGUIContainer, Assets>(container, assets);
            _targetGui.Container.DrawGuiCallback = DrawCrashReporterGui;

            if (createIfNotExist)
            {
                _targetGui.Container.Show();
            }
        }

        private static void Show()
        {
            GetCrashReporter().SetUpWindow(true);
        }

#if FOXSTER_DEV_MODE

        [MenuItem("SKTools/DevMode/CrashReporter Throw Exception")]
        private static void ShowWindowMenuItem()
        {
            throw new Exception("test exception 0.23456"+Random.value);
        }

        [MenuItem("SKTools/DevMode/CrashReporter Create Config")]
        private static void CreateConfig()
        {
            var config = new CrashReporterConfig();
            config.Type = LogType.Exception;
            config.AdditionalInfoToSend = "All logs relative to SKTools";
            config.MaxCount = 20;
            config.EmailToSend = "s.khalandachev@gmail.com";
            config.FileNameLogs = "SKToolsExceptionsLogs.json";
            config.KeysInStackTrace = "SKTools";
            config.Save(GetCrashReporter()._assetsDirectory);
        }

#endif
    }
}