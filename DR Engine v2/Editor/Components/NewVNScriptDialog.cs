using System.IO;
using DREngine.Editor.SubWindows.FieldWidgets;
using DREngine.ResourceLoading;
using Gtk;

namespace DREngine.Editor.Components
{
    public class NewVNScriptDialog : NewItemDialog
    {

        public NewVNScriptDialog(DREditor editor, Window parent, ProjectPath parentDirectory, string title = "New VN Script") :
            base(editor, parent, parentDirectory, title)
        {
        }

        protected override string ItemName => "VN Script";


        public override ProjectPath GetTargetDirectory()
        {
            var result = base.GetTargetDirectory();
            if (!result.ToString().EndsWith(".vn")) return (ProjectPath) (result + ".vn");
            return result;
        }
    }
}