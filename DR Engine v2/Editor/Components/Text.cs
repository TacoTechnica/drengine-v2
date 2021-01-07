using System;
using Gtk;

namespace DREngine.Editor.Components
{
    public class Text : Label
    {
        public enum LabelType
        {
            Normal,
            Warning,
            Error
        }

        public Text(string text) : base(text)
        {
            Wrap = true;
        }

        public void SetMode(LabelType mode)
        {
            switch (mode)
            {
                case LabelType.Normal:
                    ResetColor();
                    break;
                case LabelType.Warning:
                    SetStyle("#FFDD11");
                    break;
                case LabelType.Error:
                    SetStyle("#FF2210", "bold", "ultrabold");
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Set text mode to {mode} which is out of range!");
            }
        }

        private void SetStyle(string color, string style = "normal", string weight = "normal")
        {
            UseMarkup = true;
            Markup = $"<span foreground=\"{color}\" font_style=\"{style}\" font_weight=\"{weight}\">{Text}</span>";
            //output.Buffer.InsertWithTags(ref start, message, errTag);
        }

        private void ResetColor()
        {
            UseMarkup = false;
            //this.Text = _text;
        }
    }
}