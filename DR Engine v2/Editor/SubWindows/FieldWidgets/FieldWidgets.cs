using System;
using System.Reflection;
using Gtk;

namespace DREngine.Editor.SubWindows.FieldWidgets
{

    /// <summary>
    /// Many times we'll want to have a DIRECT mapping from CLASS FIELD to WIDGET INTERFACE.
    /// These structures are here to make that as easy as possible.
    /// </summary>
    public interface IFieldWidget
    {
        /// <summary>
        /// Whenever the field is modified
        /// </summary>
        Action<object> Modified { get; set; }
        /// <summary>
        /// The value of a variable stored in the field
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Apply the locally stored value to our target objet
        /// </summary>
        void Apply();

        /// <summary>
        /// Loads a value from a target object
        /// </summary>
        /// <param name="target"></param>
        void Load(object target);

        void InitializeField(FieldInfo field);
    }

    public abstract class FieldWidget<T> : HBox, IFieldWidget
    {
        public Action<object> Modified { get; set; }

        private object _target = null;
        private FieldInfo _field;

        public bool AutoApply = false;

        public void Apply()
        {
            if (_target != null)
            {
                _field.SetValue(_target, Value);
            }
        }

        public void Load(object target)
        {
            _target = target;
            Value = _field.GetValue(target);
        }

        public void InitializeField(FieldInfo field)
        {
            _field = field;
            Label label = new Label(field.Name);
            HBox modifierContainer = new HBox();

            this.PackStart(label, false, false, 4);

            label.Show();
            Initialize(field, modifierContainer);
            this.PackEnd(modifierContainer, true, true, 4);
            modifierContainer.Show();
        }

        public object Value
        {
            get => Data;
            set => Data = (T) value;
        }

        protected new abstract T Data { get; set; }
        
        protected abstract void Initialize(FieldInfo field, HBox content);

        protected void OnModify()
        {
            Modified?.Invoke(Value);
            if (AutoApply)
            {
                Apply();
            }
        }
    }
}
