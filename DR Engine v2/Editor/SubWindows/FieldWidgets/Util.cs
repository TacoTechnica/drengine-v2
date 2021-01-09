using System;
using System.Globalization;
using Gtk;
using Action = System.Action;

namespace DREngine.Editor.SubWindows.FieldWidgets
{

    public class FloatSlider : HBox
    {
        private float _prevValue;

        private Scale _range;

#pragma warning disable 649
        public Action Modified;
#pragma warning restore 649

        public FloatSlider(float min, float max) : this(null, min, max) {}

        public FloatSlider(string label, float min, float max, float step=1)
        {
            if (label != null)
            {
                Label pre = new Label(label);
                this.PackStart(pre, false, false, 16);
                pre.Show();
            }

            _range = new Scale(Orientation.Horizontal, min, max, step);
            _range.WidthRequest = 100;

            PackEnd(_range, true, true, 0);
            _range.Show();

            _range.ValueChanged += (sender, args) => { Modified?.Invoke(); };
        }

        public float Value
        {
            get
            {
                try
                {
                    return (float)_range.Value;
                }
                catch (Exception)
                {
                    return _prevValue;
                }
            }
            set
            {
                _prevValue = value;
                _range.Value = value;
            }
        }

    }
    
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

            _text.Buffer.Changed += (sender, args) => { Modified?.Invoke(); };
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