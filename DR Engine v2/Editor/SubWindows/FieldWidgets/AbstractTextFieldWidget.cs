using System.Reflection;
using Gtk;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public abstract class AbstractTextFieldWidget<T> : FieldWidget<T>
    {
        private bool _modifiedFlag;

        private string _prevBuffer;

        private TextView _text;
        public bool AllowNewlines = false;

        protected override T Data
        {
            get => FromString(_text.Buffer.Text);
            set
            {
                _modifiedFlag = true;
                _text.Buffer.Text = DataToString(value);
            }
        }

        protected override void Initialize(FieldInfo field, HBox content)
        {
            _text = new TextView();
            _prevBuffer = _text.Buffer.Text;
            _text.Buffer.Changed += (sender, args) =>
            {
                //Debug.Log($"FIELD SET {field.Name} = {_text.Buffer.Text}");
                if (_modifiedFlag)
                {
                    _prevBuffer = _text.Buffer.Text;
                    _modifiedFlag = false;
                    return;
                }

                // If we have invalid string, revert.
                if (!IsValidParseInternal(_text.Buffer.Text))
                {
                    _modifiedFlag = true;
                    _text.Buffer.Text = _prevBuffer;
                }
                else
                {
                    _prevBuffer = _text.Buffer.Text;
                    OnModify();
                }
            };
            content.PackEnd(_text, true, true, 4);
            _text.Show();
        }

        private bool IsValidParseInternal(string value)
        {
            if (!AllowNewlines)
                if (value.Contains('\n'))
                    return false;
            return IsValidParse(value);
        }

        protected abstract T FromString(string value);
        protected abstract string DataToString(T value);
        protected abstract bool IsValidParse(string value);
    }
}