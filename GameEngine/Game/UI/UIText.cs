using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Game.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Game.UI
{
    public class UIText : UIComponent
    {
        public string Text = "";
        public Font Font = null;
        public Color Color;
        public bool WordWrap = true;

        public bool RichText = false;

        public TextHAlignMode TextHAlign = TextHAlignMode.Left;
        public TextVAlignMode TextVAlign = TextVAlignMode.Top;
        //public WrapVerticalMode VerticalWrapMode = WrapVerticalMode.None;

        private Rect _cachedTargetRect = null;

        /// The top left corner of the text
        public Vector2 TextMin { get; private set; }

        public float CachedDrawnTextHeight { get; private set; }

        // Rich Text realtime state
        private bool _richTextBold = false;
        private bool _richTextItalics = false;
        private Color _richTextColor;

        private SpriteFont SFont => Font.SpriteFont;

        public enum WrapVerticalMode
        {
            None,
            Elipses,
            Cutoff
        }

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

        public UIText(GamePlus game, Font font, string text, Color textColor, UIComponent parent = null) : base(game, parent)
        {
            Text = text;
            Font = font;
            Color = textColor;
        }
        public UIText(GamePlus game, Font font, string text = "", UIComponent parent = null) : this(game, font, text, Color.Black, parent) {}

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            //screen.DrawRectOutline(targetRect, Color.Lavender);

            if (Font == null) return;
            if (Text == null) return;

            if (RichText)
            {
                // Init Rich Text
                _richTextColor = Color;
            }

            _cachedTargetRect = targetRect;
            screen.SpriteBatchBegin();

            string text = Text;
            if (WordWrap)
            {
                text = WrapText(SFont, Text, targetRect.Width, targetRect.Height);
            }

            TextMin = Vector2.Zero;

            if (TextHAlign != TextHAlignMode.Left || TextVAlign != TextVAlignMode.Top)
            {
                string[] lines = text.Split('\n');
                Vector2 totalSize = SFont.MeasureString(text);
                // Update text position
                float hSize = lines.Length == 0 ? 0 : SFont.MeasureString(lines[0]).X;
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
                foreach (string line in lines)
                {
                    Vector2 offset = Vector2.Zero + Vector2.UnitY * h;
                    Vector2 size = SFont.MeasureString(line);
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
                            offset.Y += targetRect.Height/2 + h - totalSize.Y / 2;
                            break;
                        case TextVAlignMode.Bottom:
                            offset.Y += targetRect.Height + h - totalSize.Y;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    DrawString(screen, SFont, line, targetRect.Min + offset);
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

        static readonly HashSet<char> DelimSet = new HashSet<char>(new []{' ', '\t', '\n', '\0'});
        private static string WrapText(SpriteFont font, string text, float maxLineWidth, float maxLineHeight)
        {
            int lastDelim = 0;
            bool lastDelimIsStart = true;
            int i = 0;
            float widthAccum = 0;

            StringBuilder result = new StringBuilder(text.Length);

            text += '\0';

            Debug.Log("$$$$$$$$$$$$$$$$");

            foreach (char c in text)
            {
                widthAccum += font.MeasureString(c.ToString()).X;
                result.Append(c);

                bool delim = DelimSet.Contains(c);
                bool over = widthAccum > maxLineWidth;

                if (delim)
                {
                    int startB = lastDelim,
                        lengthB = i - lastDelim;
                    string subB = result.ToString().Substring(startB, lengthB);
                    //Debug.Log($"SUB: {subB} : {over}");

                    //bool isNull = c == '\0';
                    if (over)
                    {
                        // Split!
                        if (lastDelimIsStart)
                        {
                            // Force split
                            int start = lastDelim,
                                length = i - lastDelim;
                            string sub = result.ToString().Substring(start, length);
                            //Debug.Log($"START SUB: {sub}");
                            float lastWidth;
                            // Newline should not be included at the start of the string.
                            string subToReplace = (lastDelim != 0? "\n" : "") + GetWordSplitted(font, sub, maxLineWidth, out lastWidth);
                            result.Remove(start, length);
                            result.Insert(start, subToReplace);
                            i += (subToReplace.Length - length) + 1;
                            widthAccum = lastWidth;

                            lastDelim = i - 1;
                            lastDelimIsStart = true;
                            continue;
                        }
                        else
                        {
                            // Nice split
                            result.Insert(lastDelim + 1, '\n');
                            string sub = result.ToString().Substring(lastDelim + 2, i - lastDelim);
                            //Debug.Log($"nice SUB: {sub}");
                            widthAccum = font.MeasureString(sub).X;
                            i += 2;
                            lastDelim = i - 1;
                            lastDelimIsStart = true;
                            continue;
                        }
                    }
                    else
                    {
                        if (c == '\n')
                        {
                            widthAccum = 0;
                            lastDelimIsStart = true;
                            lastDelim = i;
                            ++i;
                            continue;
                        }
                        else
                        {
                            lastDelimIsStart = false;
                        }
                    }
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
            StringBuilder result = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                float cwidth = font.MeasureString(c.ToString()).X;
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

        private void DrawString(UIScreen screen, SpriteFont font, string text, Vector2 pos)
        {
            if (RichText)
            {
                // TODO: Parse RichText. Store rich text properties into this items state.
            } else {
                screen.SpriteBatch.DrawString(font, text, pos, Color);
            }
        }

        public Vector2 GetSize(string text)
        {
            if (_cachedTargetRect == null)
            {
                return SFont.MeasureString(text);
            }
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
            string text = (Text.Length > max) ? Text.Substring(0, max) + "..." : Text;
            return base.ToString() + $" \"{text}\"";
        }
    }
}
