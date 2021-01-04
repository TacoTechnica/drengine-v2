using System;
using System.Reflection;
using GameEngine;
using Gtk;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public abstract class AbstractTextFieldWidget<T> : FieldWidget<T>
    {
        protected override T Data
        {
            get => FromString(_text.Buffer.Text);
            set
            {
                _modifiedFlag = true;
                _text.Buffer.Text = DataToString(value);
            }
        }

        private TextView _text;
        private bool _modifiedFlag = false;

        private string _prevBuffer;

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
                if (!IsValidParse(_text.Buffer.Text))
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

        protected abstract T FromString(string value);
        protected abstract string DataToString(T value);
        protected abstract bool IsValidParse(string value);
    }
}
