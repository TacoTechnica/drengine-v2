using System.Reflection;
using DREngine.ResourceLoading;
using GameEngine;
using Gtk;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class FieldWidgetAuto<T> : FieldWidget<T>
    {
        private DREditor _editor;

        private FieldBox _fields;
        protected override T Data
        {
            get => (T)_fields.Target;
            set => _fields.LoadTarget(value);
        }

        public FieldWidgetAuto(DREditor editor)
        {
            _editor = editor;
        }

        protected override void Initialize(UniFieldInfo field, HBox content)
        {
            _fields = new FieldBox(_editor, typeof(T))  {AutoApply = true};
            content.PackStart(_fields, true, true, 16);
            _fields.Modified += (name, obj) =>
            {
                //Debug.Log($"MODIFIED: {name} = {obj}");
                //Debug.Log($"RESULT: {Data}");
                OnModify();
            };
            _fields.Show();
        }
    }
}