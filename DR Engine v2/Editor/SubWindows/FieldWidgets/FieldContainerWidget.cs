using System.Reflection;
using Gtk;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    /// <summary>
    ///     If we have an inner class that we want to widget-tize, use this. It creates a sub fieldbox.
    ///     Allows for some crazy nice recursion.
    /// </summary>
    public class FieldContainerWidget : FieldWidget<object>
    {
        private readonly DREditor _editor;

        private FieldBox _subBox;


        public FieldContainerWidget(DREditor editor)
        {
            _editor = editor;
        }

        // Represents the container
        protected override object Data
        {
            get => _subBox.Target;
            set => _subBox.LoadTarget(value);
        }

        protected override void Initialize(FieldInfo field, HBox content)
        {
            var name = field.Name;

            // Name can depend on attribute
            var a = field.GetCustomAttribute<FieldContainerAttribute>();
            if (a != null && a.OverrideTitle != null) name = a.OverrideTitle;

            var top = new Label(name);
            _subBox = new FieldBox(_editor, field.FieldType);
            _subBox.Modified += OnModify;

            content.PackStart(top, false, false, 4);
            content.PackEnd(_subBox, true, true, 4);
            top.Show();
            _subBox.Show();
        }
    }
}