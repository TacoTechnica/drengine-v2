using System;
using System.Globalization;
using Gtk;
using Action = System.Action;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class FloatView : HBox
    {
        private float _prevValue;

        private TextView _text;

#pragma warning disable 649
        public Action Modified;
#pragma warning restore 649


        public FloatView(string label = null)
        {
            if (label != null)
            {
                Label pre = new Label(label);
                this.PackStart(pre, false, false, 16);
                pre.Show();
            }

            _text = new TextView();
            PackEnd(_text, true, true, 0);
            _text.Show();

            _text.Buffer.Changed += (sender, args) => { Modified.Invoke(); };
        }

        public float Value
        {
            get
            {
                try
                {
                    return float.Parse(_text.Buffer.Text);
                }
                catch (Exception)
                {
                    return _prevValue;
                }
            }
            set
            {
                _prevValue = value;
                _text.Buffer.Text = value.ToString(CultureInfo.InvariantCulture);
                //Modified?.Invoke();
            }
        }
    }
}