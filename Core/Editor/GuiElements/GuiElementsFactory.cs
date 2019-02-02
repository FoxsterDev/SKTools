using System;
using UnityEditor;
using UnityEngine;

namespace SKTools.Core.Editor.GuiElementsSystem
{
    public static class GuiElementsFactory
    {
        public static Func<GuiListElements> CreateLayoutTemplateBarWithLeftAndRightAnchors =
            () => new GuiListElements(
                new GuiLayoutSpace(10),
                new GuiLayoutHorizontal(
                    new GuiListElements(
                        new GuiListElements("LeftAnchor", 10),
                        new GuiLayoutFlexibleSpace(),
                        new GuiListElements("RightAnchor", 10)
                    )));

        public static Func<string, Action, GuiLayoutButton> CreateLayoutButtonWithMiniLabelStyle =
            (name, act) => new GuiLayoutButton(name, EditorStyles.miniLabel, act);

        public static Func<string, Action<bool>, Func<bool>, float, GuiLayoutToggle> CreateLayoutToggleWithFixedWidth =
            (label, setvalue, getvalue, width) => new GuiLayoutToggle(label, setvalue, getvalue,
                GUILayout.MaxWidth(width), GUILayout.MinWidth(width));
    }
}