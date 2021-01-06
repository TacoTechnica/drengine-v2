
using System;
using System.IO;
using DREngine.Editor.SubWindows.FieldWidgets;
using GameEngine;
using Gtk;


namespace DREngine.Editor.Components
{
    public abstract class NewItemDialog : EasyDialog
    {
        // ReSharper disable once UnassignedField.Global
        public new string Name;

        private ProjectPath _parentDirectory;
        private StringTextFieldWidget _input;
        private Text _label;

        //public Path NewFolderPath => FailureString == null? (_folderParent + _input.Value) : null;

        public NewItemDialog(DREditor editor, Window parent, ProjectPath parentDirectory, string title="New Folder") : base(editor, parent, title)
        {
            _parentDirectory = parentDirectory;
        }

        protected override void OnModified()
        {
            string displayPath = GetTargetDirectory().GetShortName();
            if (Directory.Exists(GetTargetDirectory()) || File.Exists(GetTargetDirectory()) )
            {
                SetFailure($"File already exists here: {displayPath}");
            }
            else if (Name == null || Name.Trim() == "")
            {
                SetFailure($"{ItemName} Name can't be empty!");
            } 
            else
            {
                SetPostText($"New {ItemName}: {displayPath}");
            }
        }

        protected override bool CheckForFailuresPreSubmit()
        {

            ProjectPath path = GetTargetDirectory();
            if (!Directory.GetParent(path).Exists)
            {
                SetFailure($"Parent Directory doesn't exist at {Directory.GetParent(path).FullName}!");
                return false;
            }

            if (Directory.Exists(path))
            {
                SetFailure($"{ItemName} already exists at {path}.");
                return false;
            }

            return true;
        }

        public virtual ProjectPath GetTargetDirectory()
        {
            return (ProjectPath)(_parentDirectory + "/" + Name);
        }

        protected abstract string ItemName { get; }
    }
}