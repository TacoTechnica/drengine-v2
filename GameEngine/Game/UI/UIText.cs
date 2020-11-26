using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Game.UI
{
    public class UIText : UIComponent
    {
        public string Text = "";
        public SpriteFont Font = null;
        public Color Color;
        public bool WordWrap = true;

        public TextHAlignMode TextHAlign = TextHAlignMode.Left;
        public TextVAlignMode TextVAlign = TextVAlignMode.Top;
        //public WrapVerticalMode VerticalWrapMode = WrapVerticalMode.None;

        private Rect _cachedTargetRect = null;

        /// The top left corner of the text
        public Vector2 TextMin { get; private set; }

        public float CachedDrawnTextHeight { get; private set; }

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

        public UIText(GamePlus game, SpriteFont font, string text, Color textColor, UIComponent parent = null) : base(game, parent)
        {
            Text = text;
            Font = font;
            Color = textColor;
        }
        public UIText(GamePlus game, SpriteFont font, string text = "", UIComponent parent = null) : this(game, font, text, Color.Black, parent) {}

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            //screen.DrawRectOutline(targetRect, Color.Lavender);

            if (Font == null) return;
            if (Text == null) return;

            _cachedTargetRect = targetRect;
            screen.SpriteBatchBegin();

            string text = Text;
            if (WordWrap)
            {
                text = WrapText(Font, Text, targetRect.Width, targetRect.Height);
            }

            TextMin = Vector2.Zero;

            if (TextHAlign != TextHAlignMode.Left || TextVAlign != TextVAlignMode.Top)
            {
                string[] lines = text.Split('\n');
                Vector2 totalSize = Font.MeasureString(text);
                // Update text position
                float hSize = lines.Length == 0 ? 0 : Font.MeasureString(lines[0]).X;
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
                    Vector2 size = Font.MeasureString(line);
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
                    screen.SpriteBatch.DrawString(Font, line, targetRect.Min + offset, Color);
                    h += size.Y;
                }

                CachedDrawnTextHeight = h;
            }
            else
            {
                try
                {
                    CachedDrawnTextHeight = Font.MeasureString(text).Y;
                }
                catch (NullReferenceException)
                {
                    CachedDrawnTextHeight = 0;
                }

                screen.SpriteBatch.DrawString(Font, text, targetRect.Min, Color);
            }

            screen.SpriteBatchEnd();
        }

        private static string WrapText(SpriteFont font, string text, float maxLineWidth, float maxLineHeight=-1)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = font.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = font.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    if (size.X > maxLineWidth)
                    {
                        if (word.Length > 1)
                        {
                            if (sb.ToString() == "")
                            {
                                sb.Append(WrapText(font, word.Insert(word.Length / 2, " ") + " ", maxLineWidth));
                            }
                            else
                            {
                                sb.Append("\n" + WrapText(font, word.Insert(word.Length / 2, " ") + " ", maxLineWidth));
                            }
                        }
                        else
                        {
                            sb.Append(word + "\n");
                        }
                    }
                    else
                    {
                        sb.Append("\n" + word + " ");
                        lineWidth = size.X + spaceWidth;
                    }
                }
            }

            return sb.ToString();
        }

        public Vector2 GetSize(string text)
        {
            if (_cachedTargetRect == null)
            {
                return Font.MeasureString(text);
            }
            return Font.MeasureString(WrapText(Font, text, _cachedTargetRect.Width, _cachedTargetRect.Height));
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

        public override string ToString()
        {
            const int max = 40;
            string text = (Text.Length > max) ? Text.Substring(0, max) + "..." : Text;
            return base.ToString() + $" \"{text}\"";
        }
    }
}
