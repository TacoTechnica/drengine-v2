using Gtk;

namespace DREngine.Editor.Components
{
    public class NewFolderDialog : NewItemDialog
    {
        public NewFolderDialog(DREditor editor, Window parent, ProjectPath parentDirectory, string title = "New Folder")
            : base(editor, parent, parentDirectory, title)
        {
        }

        protected override string ItemName => "Folder";
    }
}