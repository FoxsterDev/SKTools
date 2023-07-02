using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SKTools.Core.Editor.GuiElementsSystem
{
    public class GuiLayoutSearchBar : IGuiElement
    {
        private static GUIStyle _toolbarSearchFieldStyle, _toolbarSearchFieldCancelButtonStyle;

        public string Id { get; private set; }
        public string Text { get; private set; }
        public bool HasFocusControl { get; private set; }

        private GUILayoutOption[] _options;

        public GuiLayoutSearchBar(string id = "SearchTextField", params GUILayoutOption[] options)
        {
            Id = id;
            _options = options;
            var editorStyles = (EditorStyles) typeof(EditorStyles)
                .GetField("s_Current", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

            _toolbarSearchFieldStyle = new GUIStyle((GUIStyle) typeof(EditorStyles)
                .GetField("m_ToolbarSearchField", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(editorStyles))
            {
                stretchWidth = true,
                stretchHeight = true
            };

            _toolbarSearchFieldCancelButtonStyle = new GUIStyle((GUIStyle) typeof(EditorStyles)
                .GetField("m_ToolbarSearchFieldCancelButton", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(editorStyles));
        }

        public void Draw()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUI.SetNextControlName(Id);
            Text = GUILayout.TextField(Text, _toolbarSearchFieldStyle, _options);

            if (GUILayout.Button(string.Empty, _toolbarSearchFieldCancelButtonStyle))
            {
                Text = string.Empty;
            }

            if (GUI.GetNameOfFocusedControl() != Id && HasFocusControl)
            {
                GUI.FocusControl(Id);
            }

            GUILayout.EndHorizontal();
        }

        public string Draw(string text, bool focusControl)
        {
            HasFocusControl = focusControl;
            Text = text;
            Draw();
            return Text;
        }

        public T GetChild<T>(string id) where T : IGuiElement
        {
            if (Id == id) return (T) (object) this;
            return default(T);
        }
    }
}