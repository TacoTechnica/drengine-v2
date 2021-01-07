using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using GameEngine.Game.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace GameEngine.Game.UI
{
    public class UIText : UIComponent
    {
        public enum TextHAlignMode
        {
            Left,
            Center,
            Right
        }

        public enum TextVAlignMode
        {
            Top,
            Middle,
            Bottom
        }

        public enum WrapVerticalMode
        {
            None,
            Elipses,
            Cutoff
        }

        private static readonly HashSet<char> DelimSet = new HashSet<char>(new[] {' ', '\t', '\n', '\0'});
        //public WrapVerticalMode VerticalWrapMode = WrapVerticalMode.None;

        private Rect _cachedTargetRect;

        private bool _richText;

        // Rich Text realtime state
        private bool _richTextBold = false;
        private Color _richTextColor;
        private bool _richTextItalics = false;
        private string _richTextNormalText = "";
        private readonly Dictionary<int, RichTextTag> _tagMapIgnoreNewlines = new Dictionary<int, RichTextTag>();

        private string _text = "";
        public Color Color;
        public Font Font;

        public TextHAlignMode TextHAlign = TextHAlignMode.Left;
        public TextVAlignMode TextVAlign = TextVAlignMode.Top;
        public bool WordWrap = true;

        public UIText(GamePlus game, Font font, string text, Color textColor, UIComponent parent = null) : base(game,
            parent)
        {
            Text = text;
            Font = font;
            Color = textColor;
        }

        public UIText(GamePlus game, Font font, string text = "", UIComponent parent = null) : this(game, font, text,
            Color.Black, parent)
        {
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                if (RichText) ParseRichText();
            }
        }

        public bool RichText
        {
            get => _richText;
            set
            {
                if (!_richText) ParseRichText();
                _richText = true;
            }
        }

        /// The top left corner of the text
        public Vector2 TextMin { get; private set; }

        public float CachedDrawnTextHeight { get; private set; }

        private SpriteFont SFont => Font.SpriteFont;

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            //screen.DrawRectOutline(targetRect, Color.Lavender);

            if (Font == null || Font.SpriteFont == null) return;
            if (Text == null) return;

            if (RichText)
                // Init Rich Text
                _richTextColor = Color;

            _cachedTargetRect = targetRect;
            screen.SpriteBatchBegin();

            var text = RichText ? _richTextNormalText : Text;
            if (WordWrap) text = WrapText(SFont, text, targetRect.Width, targetRect.Height);

            TextMin = Vector2.Zero;

            if (TextHAlign != TextHAlignMode.Left || TextVAlign != TextVAlignMode.Top)
            {
                var lines = text.Split('\n');
                var totalSize = SFont.MeasureString(text);
                // Update text position
                var hSize = lines.Length == 0 ? 0 : SFont.MeasureString(lines[0]).X;
                switch (TextHAlign)
                {
                    case TextHAlignMode.Center:
                        TextMin = new Vector2(targetRect.Width / 2 - hSize / 2f, TextMin.Y);
                        break;
                    case TextHAlignMode.Right:
                        TextMin = new Vector2(targetRect.Width - hSize, TextMin.Y);
                        break;
                }

                float h = 0;
                var indexCounter = 0;
                foreach (var line in lines)
                {
                    var offset = Vector2.Zero + Vector2.UnitY * h;
                    var size = SFont.MeasureString(line);
                    switch (TextHAlign)
                    {
                        case TextHAlignMode.Left:
                            break;
                        case TextHAlignMode.Center:
                            offset.X += targetRect.Width / 2 - size.X / 2f;
                            break;
                        case TextHAlignMode.Right:
                            offset.X += targetRect.Width - size.X;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    switch (TextVAlign)
                    {
                        case TextVAlignMode.Top:
                            break;
                        case TextVAlignMode.Middle:
                            offset.Y += targetRect.Height / 2 + h - totalSize.Y / 2;
                            break;
                        case TextVAlignMode.Bottom:
                            offset.Y += targetRect.Height + h - totalSize.Y;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    DrawString(screen, SFont, line, targetRect.Min + offset, indexCounter);
                    indexCounter += line.Length;
                    h += size.Y;
                }

                CachedDrawnTextHeight = h;
            }
            else
            {
                try
                {
                    CachedDrawnTextHeight = SFont.MeasureString(text).Y;
                }
                catch (NullReferenceException)
                {
                    CachedDrawnTextHeight = 0;
                }

                DrawString(screen, SFont, text, targetRect.Min);
            }

            screen.SpriteBatchEnd();
        }

        private string WrapText(SpriteFont font, string text, float maxLineWidth, float maxLineHeight)
        {
            var lastDelim = 0;
            var lastDelimIsStart = true;
            var i = 0;
            var originalStringCounter = 0;
            float widthAccum = 0;

            var result = new StringBuilder(text.Length);

            text += '\0';

            foreach (var c in text)
            {
                ++originalStringCounter;

                widthAccum += font.MeasureString(c.ToString()).X;
                result.Append(c);

                var delim = DelimSet.Contains(c);
                var over = widthAccum > maxLineWidth;

                if (delim)
                {
                    int startB = lastDelim,
                        lengthB = i - lastDelim;

                    //bool isNull = c == '\0';
                    if (over)
                    {
                        // Split!
                        if (lastDelimIsStart)
                        {
                            // Force split
                            int start = lastDelim,
                                length = i - lastDelim;
                            var sub = result.ToString().Substring(start, length);
                            //Debug.Log($"START SUB: {sub}");
                            float lastWidth;
                            // Newline should not be included at the start of the string.
                            var subToReplace = (lastDelim != 0 ? "\n" : "") +
                                               GetWordSplitted(font, sub, maxLineWidth, out lastWidth);
                            result.Remove(start, length);
                            result.Insert(start, subToReplace);
                            i += subToReplace.Length - length + 1;
                            widthAccum = lastWidth;

                            lastDelim = i - 1;
                            lastDelimIsStart = true;
                            continue;
                        }
                        else
                        {
                            // Nice split
                            result.Insert(lastDelim + 1, '\n');
                            var sub = result.ToString().Substring(lastDelim + 2, i - lastDelim);
                            //Debug.Log($"nice SUB: {sub}");
                            widthAccum = font.MeasureString(sub).X;
                            i += 2;
                            lastDelim = i - 1;
                            lastDelimIsStart = true;
                            continue;
                        }
                    }

                    if (c == '\n')
                    {
                        widthAccum = 0;
                        lastDelimIsStart = true;
                        lastDelim = i;
                        ++i;
                        continue;
                    }

                    lastDelimIsStart = false;
                    lastDelim = i;
                }

                ++i;
            }


            // Remove null at the end.
            result.Remove(result.Length - 1, 1);

            return result.ToString();
        }

        private static string GetWordSplitted(SpriteFont font, string text, float maxWidth, out float lastTextWidth)
        {
            float widthAccum = 0;
            var result = new StringBuilder(text.Length);
            foreach (var c in text)
            {
                var cwidth = font.MeasureString(c.ToString()).X;
                if (widthAccum + cwidth > maxWidth)
                {
                    result.Append('\n');
                    widthAccum = 0;
                }

                widthAccum += cwidth;
                result.Append(c);
            }

            lastTextWidth = widthAccum;

            return result.ToString();
        }

        private void ParseRichText()
        {
            var noParse = false;

            _richTextColor = Color;

            var result = new StringBuilder(_text.Length);

            var index = 0;
            var newlineCount = 0;

            foreach (var segment in Regex.Split(_text, @"(<[^>]*>)"))
            {
                var tag = segment.StartsWith("<") && segment.EndsWith(">");
                if (tag)
                {
                    var inner = segment.Substring(1, segment.Length - 2);
                    var open = !inner.StartsWith("/");
                    if (!open) inner = inner.Substring(1);

                    RichTextTag newTag = null;
                    //Debug.Log($"OUTER: \"{segment}\" INNER: \"{inner}\"");
                    switch (inner)
                    {
                        case "b":
                            newTag = new BoldTag(open);
                            break;
                        case "color":
                            newTag = new ColorTag();
                            break;
                        default:
                            // Check for custom color
                            if (inner.StartsWith("#")) newTag = new ColorTag(inner);
                            break;
                    }

                    // We have a new valid tag
                    if (newTag != null)
                    {
                        _tagMapIgnoreNewlines[index - newlineCount] = newTag;
                    }
                    else
                    {
                        // Invalid, regular string.
                        result.Append(segment);
                        index += segment.Length;
                        newlineCount += segment.Split('\n').Length - 1;
                    }
                }
                else
                {
                    result.Append(segment);
                    index += segment.Length;
                    newlineCount += segment.Split('\n').Length - 1;
                }
            }

            _richTextNormalText = result.ToString();
        }

        private void DrawString(UIScreen screen, SpriteFont font, string text, Vector2 pos, int indexOffset = 0)
        {
            if (RichText)
            {
                var start = Vector2.Zero;
                // TODO: richTextFont
                var i = 0;
                var newLines = 0;
                foreach (var c in text)
                {
                    var tagIndex = i - newLines;
                    var isTag = _tagMapIgnoreNewlines.ContainsKey(tagIndex);
                    if (isTag)
                    {
                        var tag = _tagMapIgnoreNewlines[tagIndex];
                        tag.Activate(this);
                    }

                    switch (c)
                    {
                        case '\n':
                            start.Y += font.LineSpacing;
                            start.X = 0;
                            newLines += 1;
                            break;
                        case '\r':
                            break;
                        default:
                            screen.SpriteBatch.DrawString(font, c.ToString(), pos + start, _richTextColor);
                            start.X += font.MeasureString(c.ToString()).X;
                            break;
                    }

                    ++i;
                }
            }
            else
            {
                screen.SpriteBatch.DrawString(font, text, pos, Color);
            }
        }

        public Vector2 GetSize(string text)
        {
            if (_cachedTargetRect == null) return SFont.MeasureString(text);
            return SFont.MeasureString(WrapText(SFont, text, _cachedTargetRect.Width, _cachedTargetRect.Height));
        }

        public UIText WithoutWordWrap()
        {
            WordWrap = false;
            return this;
        }

        public UIText WithHAlign(TextHAlignMode mode)
        {
            TextHAlign = mode;
            return this;
        }

        public UIText WithVAlign(TextVAlignMode mode)
        {
            TextVAlign = mode;
            return this;
        }

        public UIText WithRichText()
        {
            RichText = true;
            return this;
        }

        public override string ToString()
        {
            const int max = 40;
            var text = Text.Length > max ? "\"" + Text.Substring(0, max) + "\"..." : "\"" + Text + "\"";
            // Don't print newlines in our output.
            text = text.Replace("\n", "\\n");
            return base.ToString() + text;
        }

        abstract class RichTextTag
        {
            private readonly bool _open;

            public RichTextTag(bool open)
            {
                _open = open;
            }

            public void Activate(UIText parent)
            {
                Activate(parent, _open);
            }

            protected abstract void Activate(UIText parent, bool open);
        }

        private class BoldTag : RichTextTag
        {
            public BoldTag(bool open) : base(open)
            {
            }

            protected override void Activate(UIText parent, bool open)
            {
                if (open)
                    parent._richTextColor = Color.Green;
                else
                    parent._richTextColor = parent.Color;
            }
        }

        private class ColorTag : RichTextTag
        {
            private readonly Color _color;

            public ColorTag(Color color) : base(true)
            {
                _color = color;
            }

            public ColorTag(string hex) : base(true)
            {
                var c = ColorTranslator.FromHtml(hex);
                _color = new Color(c.R, c.G, c.B, c.A);
            }

            public ColorTag() : base(false)
            {
            }

            protected override void Activate(UIText parent, bool open)
            {
                if (open)
                    parent._richTextColor = _color;
                else
                    parent._richTextColor = parent.Color;
            }
        }
    }
}