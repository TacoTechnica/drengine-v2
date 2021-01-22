using System.Reflection;
using DREngine.ResourceLoading;
using Gtk;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class UnknownFieldWidget : FieldWidget<object>
    {
        private readonly string _errorMessage;

        private object _data;

        public UnknownFieldWidget(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        // Empty
        protected override object Data
        {
            get => _data;
            set => _data = value;
        }

        protected override void Initialize(UniFieldInfo field, HBox content)
        {
            var unknown = new Label(_errorMessage);
            content.Add(unknown);
            unknown.Show();
        }
    }
}