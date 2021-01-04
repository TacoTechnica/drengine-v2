using System;
using Gtk;

namespace DREngine.Editor
{
    public class Text : Label
    {
        public const int NORMAL = 0,
            WARNING = 1,
            ERROR = 2;

        private string _text;

        public Text(string text) : base(text)
        {
            _text = text;
            this.Wrap = true;
        }

        public void SetMode(int mode)
        {
            switch (mode)
            {
                case NORMAL:
                    ResetColor();
                    break;
                case WARNING:
                    SetStyle("#FFDD11", "italic");
                    break;
                case ERROR:
                    SetStyle("#FF2210", "bold", "ultrabold");
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Set text mode to {mode} which is out of range!");
            }
        }

        private void SetStyle(string color, string style = "normal", string weight="normal")
        {
            this.UseMarkup = true;
            this.Markup = $"<span foreground=\"{color}\" font_style=\"{style}\" font_weight=\"{weight}\">{Text}</span>";
            //output.Buffer.InsertWithTags(ref start, message, errTag);
        }

        private void ResetColor()
        {
            this.UseMarkup = false;
            //this.Text = _text;
        }
    }
}