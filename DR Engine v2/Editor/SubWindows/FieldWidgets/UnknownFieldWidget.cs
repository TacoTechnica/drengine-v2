using System.Reflection;
using Gtk;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class UnknownFieldWidget : FieldWidget<object>
    {
        // Empty
        protected override object Data { get => null; set {} }

        private string _errorMessage;

        public UnknownFieldWidget(string errorMessage)
        {
            _errorMessage = errorMessage;
        }
        
        protected override void Initialize(FieldInfo field, HBox content)
        {
            Label unknown = new Label(_errorMessage);
            content.Add(unknown);
            unknown.Show();
        }
    }
}