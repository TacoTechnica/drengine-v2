using System.IO;
using DREngine.Editor.SubWindows.FieldWidgets;
using GameEngine;
using Gtk;
using Path = GameEngine.Game.Path;

namespace DREngine.Editor.Components
{
    public class NewFolderDialog : EasyDialog
    {
        public string FolderName;

        private ProjectPath _folderParent;
        private StringTextFieldWidget _input;
        private Text _label;

        //public Path NewFolderPath => FailureString == null? (_folderParent + _input.Value) : null;

        public NewFolderDialog(DREditor editor, Window parent, ProjectPath folderParent, string title="New Folder") : base(editor, parent, title)
        {
            _folderParent = folderParent;
        }

        protected override void OnModified()
        {
            string displayPath = GetTargetDirectory().GetShortName();
            if (Directory.Exists(GetTargetDirectory()))
            {
                SetFailure($"Directory already exists here: {displayPath}");
            }
            else
            {
                SetPostText($"New Directory: {displayPath}");
            }
        }

        protected override bool CheckForFailuresPreSubmit()
        {
            ProjectPath path = GetTargetDirectory();
            if (!Directory.GetParent(path).Exists)
            {
                SetFailure($"Directory parent doesn't exist at {Directory.GetParent(path).FullName}!");
                return false;
            }

            if (Directory.Exists(path))
            {
                SetFailure($"Directory already exists at {path}.");
                return false;
            }

            return true;
        }

        public ProjectPath GetTargetDirectory()
        {
            return (ProjectPath)(_folderParent + "/" + FolderName);
        }
    }
}
