using System.IO;
using DREngine.Editor.SubWindows.FieldWidgets;
using GameEngine;
using Gtk;
using Path = GameEngine.Game.Path;

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
