using System;
using UnityEditor;

namespace SKTools.Base.Editor.GuiElementsSystem
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
    }
}