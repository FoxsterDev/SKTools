using SKTools.Base.Editor;
using UnityEngine;

namespace SKTools.Module.CrashReporter
{
    internal sealed class Assets : AssetsContainer
    {
        private GUIStyle _labelStyle, _buttonStyle;
        
        public GUIStyle LabelStyle
        {
            get
            {
                if (_labelStyle == null)
                {
                    _labelStyle = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontStyle = FontStyle.Bold,
                        fontSize = 32
                    };
                }

                return _labelStyle;
            }
        }

        public GUIStyle ButtonStyle
        {
            get
            {
                if (_buttonStyle == null)
                {
                    _buttonStyle = new GUIStyle(GUI.skin.button)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontStyle = FontStyle.Bold,
                        fontSize = 20,
                    };
                }

                return _buttonStyle;
            }
        }
        
        public Assets(string assetsDirectory) : base(assetsDirectory)
        {
        }
    }
}