using System;
using SKTools.Base.Editor;
using UnityEditor;
using UnityEngine;

namespace SKTools.Module.CrashReporter
{
    public partial class CrashReporter
    {
        private Surrogate<IGUIContainer, Assets> _targetGui;
        private CrashReporterConfig _config;
        private static string _condition, _stacktrace, _type;


        private static CrashReporter _instance;

        private static CrashReporter GetCrashReporter()
        {
            return _instance ?? (_instance = new CrashReporter());
        }

        public static void Show(CrashReporterConfig config)
        {
            var instance = GetCrashReporter();
            instance._config = config;
            instance.SetUpWindow(true);
        }

        private CrashReporterConfig Config
        {
            get { return _config ?? (_config = new CrashReporterConfig().Load()); }
        }

        [InitializeOnLoadMethod]
        private static void CrashReporter_CheckReload()
        {
            Application.logMessageReceived += Application_LogMessageReceived;
            GetCrashReporter().SetUpWindow(false);
         
        }

        private static void Application_LogMessageReceived(string condition, string stacktrace, LogType type)
        {
            if (type == LogType.Exception && stacktrace.Contains("SKTools"))
            {
                _condition = condition;
                _stacktrace = stacktrace;
                _type = type.ToString();
                Show(null);
            }
        }

        private void SetUpWindow(bool createIfNotExist)
        {
            var container = CustomEditorWindow<Window>.GetWindow(createIfNotExist);
            if (container == null) return;

            //_feedbackMessage = Config.FeedbackMessage;
            //_starRects = new Rect[Config.MaxStar];

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

        private void DrawCrashReporterGui(IGUIContainer window)
        {
            var position = window.Position;

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
            
            window.Repaint();
        }
        
#if FOXSTER_DEV_MODE
        
        [MenuItem("SKTools/CrashReporter Throw Exception")]
        private static void ShowWindowMenuItem()
        {
            throw new Exception("test exception");
        }
        
#endif
    }
}