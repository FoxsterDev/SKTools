using SKTools.Core.Editor;
using UnityEngine;

namespace SKTools.Module.CrashReporter
{
    internal sealed class Assets : AssetsContainer
    {
        private GUIStyle _labelStyle, _labelErrorStyle, _buttonStyle;
        private GUIContent _boxContent;
        
        public GUIContent BoxContent
        {
            get
            {
                if (_boxContent == null)
                {
                    _boxContent = new GUIContent("Pay attention!", null, "I'll be highly appreciate,\nif you send me this exception");
                }

                return _boxContent;
            }
        }
        
        public GUIStyle LabelErrorStyle
        {
            get
            {
                if (_labelErrorStyle == null)
                {
                    _labelErrorStyle = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.UpperCenter,
                        fontStyle = FontStyle.Bold,
                        fontSize = 16,
                        wordWrap = true,
                    
                    };
                    _labelErrorStyle.normal.textColor = Color.red;
                }

                return _labelErrorStyle;
            }
        }
        
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