using GameEngine;
using Gdk;
using Gtk;
using Pango;
using WrapMode = Gtk.WrapMode;

namespace DREngine.Editor.Components
{
    public class EditorLog : ScrolledWindow
    {
        private readonly TextTag _debugTag;
        private readonly TextTag _errorTag;

        private bool _forceScrollToBottomOnce;

        private readonly TextTag _regularTag;

        private readonly TextView _text;
        private readonly TextTag _warningTag;

        public EditorLog()
        {
            _text = new TextView();

            // Tags
            _regularTag = NewTag("log", "#113311", "#FFFFFF");
            _debugTag = NewTag("debug", "#113311", "#CCAADD", Weight.Bold);
            _warningTag = NewTag("warning", "#113311", "#FFCC00", Weight.Bold);
            _errorTag = NewTag("error", "#113311", "#FF1133", Weight.Bold);

            _text.Editable = false;
            _text.WrapMode = WrapMode.Char;
            _text.Monospace = true;

            _text.PopulatePopup += TextOnPopulatePopup;

            Add(_text);
            _text.Show();

            _forceScrollToBottomOnce = true;
        }

        private void TextOnPopulatePopup(object o, PopulatePopupArgs args)
        {
            if (args.Popup is Menu m)
            {
                var item = new MenuItem();
                item.Label = "Clear";
                item.ButtonPressEvent += (o1, eventArgs) =>
                {
                    if (eventArgs.Event.Type == EventType.ButtonPress && eventArgs.Event.Button == 1) Clear();
                };
                m.Add(item);
                item.Show();
            }
            else
            {
                Debug.LogError("Failed to add clear option to popup. Your error log won't be clearable now sadly.");
            }
        }

        private TextTag NewTag(string name, string background, string foreground,
            Weight weight = Weight.Normal)
        {
            var tag = new TextTag(name);
            tag.Background = background;
            tag.Foreground = foreground;
            tag.Weight = weight;
            _text.Buffer.TagTable.Add(tag);
            return tag;
        }

        public void Clear()
        {
            _text.Buffer.Text = "";
        }

        public void Print(string text)
        {
            PrintWithTag(text + "\n", _regularTag);
        }

        public void PrintWarning(string text)
        {
            PrintWithTag(text + "\n", _warningTag);
        }

        public void PrintDebug(string text)
        {
            PrintWithTag(text + "\n", _debugTag);
        }

        public void PrintError(string text)
        {
            PrintWithTag(text + "\n", _errorTag);
        }

        private void PrintWithTag(string text, TextTag tag)
        {
            Application.Invoke(delegate
            {
                float threshold = 1000;
                //Debug.LogSilent($"{this.Vadjustment.Value} ?> {this.Vadjustment.Upper - threshold}");
                var stick = Vadjustment.Value > Vadjustment.Upper - threshold || _forceScrollToBottomOnce;
                _forceScrollToBottomOnce = false;
                var start = _text.Buffer.EndIter;
                _text.Buffer.InsertWithTags(ref start, text, tag);
                if (stick) Vadjustment.Value = Vadjustment.Upper;
            });
        }
    }
}