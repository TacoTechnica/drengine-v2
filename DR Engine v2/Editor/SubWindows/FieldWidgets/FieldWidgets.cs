using System;
using System.Reflection;
using System.Text.RegularExpressions;
using GameEngine;
using Gtk;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    /// <summary>
    ///     Many times we'll want to have a DIRECT mapping from CLASS FIELD to WIDGET INTERFACE.
    ///     These structures are here to make that as easy as possible.
    /// </summary>
    public interface IFieldWidget
    {
        /// <summary>
        ///     Whenever the field is modified
        /// </summary>
        Action<object> Modified { get; set; }

        /// <summary>
        ///     The value of a variable stored in the field
        /// </summary>
        object Value { get; set; }

        /// <summary>
        ///     Apply the locally stored value to our target objet
        /// </summary>
        void Apply();

        /// <summary>
        ///     Loads a value from a target object
        /// </summary>
        /// <param name="target"></param>
        void Load(object target);

        void InitializeField(MemberInfo field);
    }

    public abstract class FieldWidget<T> : HBox, IFieldWidget
    {
        // Split into capital spaces.
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Regex SpaceRegex = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

        private MemberInfo _field;

        private object _target;

        public bool AutoApply = false;

        protected new abstract T Data { get; set; }
        public Action<object> Modified { get; set; }

        public void Apply()
        {
            if (_target != null)
            {
                if (_field is FieldInfo finfo)
                {
                    finfo.SetValue(_target, Value);
                } else if (_field is PropertyInfo pinfo)
                {
                    pinfo.SetValue(_target, Value);
                }
            }
            else
            {
                //Debug.Log($"Field {_field.Name} of object {typeof(T)} is null.");
            }
        }

        public void Load(object target)
        {
            _target = target;
            if (_field is FieldInfo finfo)
            {
                Value = finfo.GetValue(_target);
            } else if (_field is PropertyInfo pinfo)
            {
                Value = pinfo.GetValue(_target);
            }
        }

        public void InitializeField(MemberInfo field)
        {
            _field = field;
            var label = new Label(GetFieldName(field));
            var modifierContainer = new HBox();

            PackStart(label, false, false, 4);

            label.Show();
            Initialize(field, modifierContainer);
            PackEnd(modifierContainer, true, true, 4);
            modifierContainer.Show();
        }

        public object Value
        {
            get => Data;
            set => Data = (T) value;
        }

        protected object GetFieldParent()
        {
            return _target;
        }

        protected abstract void Initialize(MemberInfo field, HBox content);

        protected void OnModify()
        {
            Modified?.Invoke(Value);
            if (AutoApply) Apply();
        }

        private static string GetFieldName(MemberInfo field)
        {
            return SpaceRegex.Replace(field.Name, " ");
        }
    }
}