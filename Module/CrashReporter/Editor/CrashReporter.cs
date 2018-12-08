using SKTools.Base.Editor;
using UnityEditor;
using UnityEngine;

namespace SKTools.Module.CrashReporter
{
    public partial class CrashReporter
    {
        private Surrogate<IGUIContainer, Assets> _targetGui;
        private CrashReporterConfig _config;
        private string _condition, _stacktrace, _type;
        private static CrashReporter _instance;

        private static CrashReporter GetCrashReporter()
        {
            return _instance ?? (_instance = new CrashReporter());
        }

        private CrashReporterConfig Config
        {
            get { return _config ?? (_config = new CrashReporterConfig()); }
        }

        private void Application_LogMessageReceived(string condition, string stacktrace, LogType type)
        {
            if (type == LogType.Exception && stacktrace.Contains("SKTools"))
            {
                _condition = condition;
                _stacktrace = stacktrace;
                _type = type.ToString();
                 Show(null);
            }
        }
        
        private void DrawCrashReporterGui(IGUIContainer window)
        {
            var position = window.Position;

            GUI.Label(new Rect(5, 5, position.width, 28), "Something bad happened",_targetGui.Assets.LabelStyle);

            
            const float buttonWidth = 200;
            const float buttonHeight = 64;
            if (GUI.Button(new Rect(position.width / 2 - buttonWidth, position.height - buttonHeight, buttonWidth, buttonHeight),
                "Don't Show Again",
                _targetGui.Assets.ButtonStyle))
            {
                window.Close();
                return;
            }

            if (GUI.Button(new Rect(position.width / 2 + 5, position.height - buttonHeight, 156, buttonHeight),
                "Send Report",
                _targetGui.Assets.ButtonStyle))
            {
                //Email();
                window.Close();
                return;
            }
            
            window.Repaint();
            return;
            var height = position.height * 0.5f;
            var rectUpper = new Rect(0, 0, position.width, height);
            // GUI.color = Color.red;
            GUIStyle myTextAreaStyle = new GUIStyle(EditorStyles.miniLabel);
            // ...
            myTextAreaStyle.wordWrap = true;
            myTextAreaStyle.normal.textColor = Color.red;
            
            // value = EditorGUILayout.TextArea(value, myTextAreaStyle);
            var label = string.Format("Condition={0} LogType={1} StackTrace={2} ", _condition, _type, _stacktrace);
            EditorGUILayout.TextArea(label,myTextAreaStyle, GUILayout.MaxHeight(position.height/2),
                GUILayout.MinHeight(position.height/2), GUILayout.MaxWidth(position.width));
            /*GUI.Label(rectUpper,string.Format("Condition={0} LogType={1} StackTrace={2} ", _condition,_type, _stacktrace ),
                EditorStyles.miniBoldLabel);*/
            //GUI.color = Color.white;
            if (GUI.Button(new Rect((position.width - 128) / 2, position.height - 64, 128, 64), "Send Email",
                _targetGui.Assets.ButtonStyle))
            {
            }

            window.Repaint();
        }
        
    }
}