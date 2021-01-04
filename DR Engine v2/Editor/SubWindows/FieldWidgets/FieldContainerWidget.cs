using System.Reflection;
using Gtk;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    /// <summary>
    /// If we have an inner class that we want to widget-tize, use this. It creates a sub fieldbox.
    ///
    /// Allows for some crazy nice recursion.
    /// </summary>
    public class FieldContainerWidget : FieldWidget<object>
    {
        private DREditor _editor;

        private FieldBox _subBox;

        private object _data;

        // Represents the container
        protected override object Data
        {
            get
            {
                return _subBox.Target;
            }
            set
            {
                _subBox.LoadTarget(value);
            }
        }


        public FieldContainerWidget(DREditor editor)
        {
            _editor = editor;
        }

        protected override void Initialize(FieldInfo field, HBox content)
        {
            string name = field.Name;

            // Name can depend on attribute
            FieldContainerAttribute a = field.GetCustomAttribute<FieldContainerAttribute>();
            if (a != null && a.OverrideTitle != null)
            {
                name = a.OverrideTitle;
            }

            Label top = new Label(name);
            _subBox = new FieldBox(_editor, field.FieldType);
            _subBox.Modified += OnModify;

            content.PackStart(top, false, false, 4);
            content.PackEnd(_subBox, true, true, 4);
            top.Show();
            _subBox.Show();
        }
    }
}