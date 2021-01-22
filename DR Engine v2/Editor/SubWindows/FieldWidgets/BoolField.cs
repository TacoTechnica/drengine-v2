using System.Reflection;
using Gtk;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class BoolField : FieldWidget<bool>
    {
        private CheckMenuItem _check;

        protected override bool Data
        {
            get => _check.Active;
            set => _check.Active = value;
        }

        protected override void Initialize(MemberInfo field, HBox content)
        {
            _check = new CheckMenuItem();
            content.PackStart(_check, true, true, 16);
            _check.Show();

            _check.Toggled += (sender, args) => { OnModify(); };
        }
    }
}