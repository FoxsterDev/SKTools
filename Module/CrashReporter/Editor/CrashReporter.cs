using System.Collections.Generic;
using SKTools.Base.Editor;
using UnityEditor;
using UnityEngine;

namespace SKTools.Module.CrashReporter
{
    public partial class CrashReporter
    {
        private Surrogate<IGUIContainer, Assets> _targetGui;
        //private CrashReportConfig _config;
        //private string _condition, _stacktrace, _type;
        private List<CrashReporterConfig> _configs;
        private static CrashReporter _instance;
        private string _assetsDirectory;
        private CrashReporterLogs _logs;
        
        private static CrashReporter GetCrashReporter()
        {
            return _instance ?? (_instance = new CrashReporter());
        }

        private CrashReporter()
        {
            _assetsDirectory = Utility.GetPathRelativeToCurrentDirectory("Editor Resources");
        }
        
        private List<CrashReporterConfig> Configs
        {
            get
            {
                if (_configs != null)
                    return _configs;
                
                _configs = new List<CrashReporterConfig>();
                
                var assets = AssetDatabase.FindAssets("t:textasset CrashReporterConfig");
                foreach (var guid in assets)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (assetPath.Contains(".json"))
                    {
                        var json = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath).text;
                        var config  = new CrashReporterConfig(json);
                        _configs.Add(config);
                        Debug.Log("added:="+ AssetDatabase.GUIDToAssetPath(guid));
                    }
                }
                
                _configs.Sort((a,b)=> b.Version - a.Version);
                return _configs;
            }
        }

        private void Application_LogMessageReceived(string condition, string stacktrace, LogType type)
        {
            if (Configs != null)
            {
                var config = Configs[0];
                if (type == config.Type && stacktrace.Contains(config.KeysInStackTrace))
                {
                    var line = new CrashReporterLogs.Line
                        {Condition = condition, Stacktrace = stacktrace, type = type, Count = 1};
                    if (_logs == null)
                    {
                        _logs = new CrashReporterLogs(config.FileNameLogs);
                        _logs.Config = config;
                        _logs.Load(_assetsDirectory);
                    }

                    var l = _logs.Lines.Find(_l => _l.Equals(line));
                    if (l != null)
                    {
                        l.Count += 1;
                    }
                    else
                    {
                        _logs.Lines.Add(line);
                    }
                    
                    _logs.Save(_assetsDirectory);
                }
            }
        }
        
        private void DrawCrashReporterGui(IGUIContainer window)
        {
            const float buttonWidth = 200;
            const float buttonHeight = 64;

            var position = window.Position;

            GUI.Label(new Rect(5, 5, position.width, 28), "Something bad happened", _targetGui.Assets.LabelStyle);

            var rect = new Rect(5, 40, position.width - 10, position.height - buttonHeight - 40 - 10);
            GUI.Box(rect, _targetGui.Assets.BoxContent);
            var label = "";//string.Format("Condition={0}\nStackTrace={1} ", _condition, _stacktrace);
            GUI.Label(rect,label, _targetGui.Assets.LabelErrorStyle);

            if (GUI.Button(new Rect(position.width / 2 - buttonWidth, position.height - buttonHeight, buttonWidth, buttonHeight),
                "Don't Show Again",
                _targetGui.Assets.ButtonStyle))
            {
                window.Close();
                return;
            }

            if (GUI.Button(new Rect(position.width / 2 + 5, position.height - buttonHeight, buttonWidth, buttonHeight),
                "Send Report",
                _targetGui.Assets.ButtonStyle))
            {
                Email();
                window.Close();
                return;
            }
            
            window.Repaint();
        }

        private void Email()
        {
            
        }
    }
}