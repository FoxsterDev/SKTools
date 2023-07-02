using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKTools.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Utility = SKTools.Editor.Utility;

namespace SKTools.Editor.Windows.CrashReporter
{
    public partial class CrashReporter
    {
        private Surrogate<IGUIContainer, Assets> _targetGui;
        private List<CrashReporterConfig> _configs;
        private static CrashReporter _instance;
        private string _assetsDirectory;
        private CrashReporterLogs _logs;
        private Vector2 _scrollPosition = Vector2.zero;
        private string _result;
        
        private static CrashReporter GetCrashReporter()
        {
            return _instance ?? (_instance = new CrashReporter());
        }

        private CrashReporter()
        {
            _assetsDirectory = Utility.GetPathRelativeToCurrentDirectory("Editor Resources");
        }

        private CrashReporterLogs Logs
        {
            get
            {
                if (_logs == null)
                {
                    _logs = new CrashReporterLogs(Configs[0].FileNameLogs);
                    _logs.Load(_assetsDirectory);
                    _logs.Config = Configs[0];
                }

                return _logs;
            }
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
                        //Debug.Log("added:="+ AssetDatabase.GUIDToAssetPath(guid));
                    }
                }
                
                _configs.Sort((a,b)=> b.Version - a.Version);
                return _configs;
            }
        }

        private void Application_LogMessageReceived(string condition, string stacktrace, LogType type)
        {
            if (Configs == null || Configs.Count < 1 || Configs[0].DontShowAgain) return;
            
            var config = Configs[0];
            if (type == config.Type && stacktrace.Contains(config.KeysInStackTrace))
            {
                var line = new CrashReporterLogs.Line
                    {Condition = condition, Stacktrace = stacktrace, Type = type, Count = 1};
                    
                var l = Logs.Lines.Find(_l => _l.Equals(line));
                if (l != null)
                {
                    l.Count += 1;
                }
                else
                {
                    while (Logs.Lines.Count >= config.MaxCount)
                    {
                        Logs.Lines.RemoveAt(Logs.Lines.Count - 1);
                    }
                    Logs.Lines.Insert(0, line);
                }
                    
                Logs.Save(_assetsDirectory);
                
                _result = Logs.Lines.Aggregate(config.AdditionalInfoToSend+"\n",
                    (longest, next) => longest + next.ToString());
               _scrollPosition = Vector2.zero;
                Show();
            }
        }

        private void DrawCrashReporterGui(IGUIContainer window)
        {
            const float buttonWidth = 200;
            const float buttonHeight = 64;

            var position = window.Position;

            GUI.Label(new Rect(5, 5, position.width, 28), "Something bad happened", _targetGui.Assets.LabelStyle);

            var rect = new Rect(5, 40, position.width - 10, position.height - buttonHeight - 40 - 10);
            //GUI.Box(rect, _targetGui.Assets.BoxContent);
            _scrollPosition = GUI.BeginScrollView(rect, _scrollPosition, new Rect(0, 0, rect.width, 1000), false, true);

            GUI.Label(new Rect(0, 0, rect.width, 1000), _result, _targetGui.Assets.LabelErrorStyle);
            GUI.EndScrollView();
            
            if (GUI.Button(new Rect(position.width / 2 - buttonWidth, position.height - buttonHeight, buttonWidth, buttonHeight),
                "Don't Show Again",
                _targetGui.Assets.ButtonStyle))
            {
                Logs.Config.DontShowAgain = true;
                Logs.Save(_assetsDirectory);
                window.Close();
                return;
            }

            if (GUI.Button(new Rect(position.width / 2 + 5, position.height - buttonHeight, buttonWidth, buttonHeight),
                "Send Report",
                _targetGui.Assets.ButtonStyle))
            {
                Email();
                Logs.Lines.Clear();
                Logs.Save(_assetsDirectory);
                window.Close();
                return;
            }
            
            window.Repaint();
        }

        private void Email()
        {
            var builder = new StringBuilder();
            builder.Append("mailto:" + Logs.Config.EmailToSend);
            builder.Append("?subject=" + EscapeURL(Logs.Config.AdditionalInfoToSend));
            builder.Append("&body=" + EscapeURL(_result));

            Application.OpenURL(builder.ToString());
        }

        private string EscapeURL(string url)
        {
            return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
        }
    }
}