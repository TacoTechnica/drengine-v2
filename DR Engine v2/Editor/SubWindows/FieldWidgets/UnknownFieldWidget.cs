using System.Reflection;
using Gtk;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class UnknownFieldWidget : FieldWidget<object>
    {
        private readonly string _errorMessage;

        public UnknownFieldWidget(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        // Empty
        protected override object Data
        {
            get => null;
            set { }
        }

        protected override void Initialize(MemberInfo field, HBox content)
        {
            var unknown = new Label(_errorMessage);
            content.Add(unknown);
            unknown.Show();
        }
    }
}