using UnityEngine;

namespace SKTools.ScriptsExtensions
{
    /// <summary>
    ///     This script helps to stylize strings in Unity 3.x and above.
    /// </summary>
    public static class StyledTextEx
    {
        public static Style ToStylize(this string text)
        {
            return new Style(text);
        }

        public static string ToStylize(this string text, Color color)
        {
            return new Style(text, color).ToString();
        }

        public static string ToStylize(this string text, Color color, int size)
        {
            return new Style(text, color, size).ToString();
        }

        public static string ToStylize(this string text, Color color, FontStyle fontStyle)
        {
            return new Style(text, color, 0, fontStyle).ToString();
        }

        public static string ToStylize(this string text, Style style)
        {
            return new Style(text, style).ToString();
        }

        public struct Style
        {
            private string _text;
            private readonly Color _color;
            private readonly int _size;
            private readonly FontStyle _fontStyle;

            public Style(string text)
            {
                _text = text;
                _color = Color.white;
                _size = 0;
                _fontStyle = FontStyle.Normal;
            }

            public Style(string text, Style style)
            {
                _text = text;
                _color = style._color;
                _size = style._size;
                _fontStyle = style._fontStyle;

                Apply();
            }

            public Style(string text, Color color, int size = 0, FontStyle fontStyle = FontStyle.Normal)
            {
                _text = text;
                _color = color;
                _size = size;
                _fontStyle = fontStyle;

                Apply();
            }

            private Style Apply()
            {
                switch (_fontStyle)
                {
                    case FontStyle.Italic:
                    {
                        Italics();
                        break;
                    }
                    case FontStyle.Bold:
                    {
                        Bold();
                        break;
                    }
                    case FontStyle.BoldAndItalic:
                    {
                        Italics();
                        Bold();
                        break;
                    }
                }

                Sized(_size);
                Colored(_color);

                return this;
            }

            public Style Colored(Color color)
            {
                _text = string.Format("<color=#{0}>{1}</color>", ColorToHex(color), _text);
                return this;
            }

            public Style Sized(int size)
            {
                if (size > 0) _text = string.Format("<size={0}>{1}</size>", size, _text);
                return this;
            }

            public Style Bold()
            {
                _text = string.Format("<b>{0}</b>", _text);
                return this;
            }

            public Style Italics()
            {
                _text = string.Format("<i>{0}</i>", _text);
                return this;
            }

            private string ColorToHex(Color32 color)
            {
                return string.Concat(color.r.ToString("X2"), color.g.ToString("X2"), color.b.ToString("X2"));
            }

            public override string ToString()
            {
                return _text;
            }
        }
    }
}