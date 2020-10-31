using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game.UI
{
    public class UIText : UiComponent
    {
        public string Text = "";
        public SpriteFont Font = null;
        public Color Color = Color.Black;
        public bool WordWrap = true;

        public TextHAlignMode TextHAlign = TextHAlignMode.Left;
        public TextVAlignMode TextVAlign = TextVAlignMode.Top;
        //public WrapVerticalMode VerticalWrapMode = WrapVerticalMode.None;

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

        public UIText(GamePlus game, UiComponent parent, SpriteFont font, string text) : base(game, parent)
        {
            Text = text;
            Font = font;
        }
        public UIText(GamePlus game, SpriteFont font, string text) : this(game, null, font, text) {}

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            screen.SpriteBatchBegin();

            string text = Text;
            if (WordWrap)
            {
                text = WrapText(Font, Text, targetRect.Width, targetRect.Height);
            }

            if (TextHAlign != TextHAlignMode.Left || TextVAlign != TextVAlignMode.Top)
            {
                Vector2 totalSize = Font.MeasureString(text);
                float h = 0;
                foreach (string line in text.Split('\n'))
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
            }
            else
            {
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
    }
}
