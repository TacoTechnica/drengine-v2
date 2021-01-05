using System.IO;
using DREngine.Editor.SubWindows.FieldWidgets;
using GameEngine.Game;
using Gtk;
using Path = GameEngine.Game.Path;

namespace DREngine.Editor.Components
{
    public class NewProjectDialog : EasyDialog
    {
        //[EngineDirectoryField("projects", "Choose Project Parent Directory")]
        //public string ProjectParentDirectory = null;

        private DREditor _editor;

        public string ProjectTitle = "Example Project";
        public string Author = "Me";

        [EngineFileField("projects", "Choose Icon", "*.png", "Image")]
        public string IconPath = null;

        private FieldBox _fields;

        public NewProjectDialog(DREditor editor, Window parent, string title="New Project") : base(editor, parent, title)
        {
            _editor = editor;

            _post.Text =
                "Will create a new folder in the projects directory.\nYou can always change the project name later.\nIcon is optional.";
        }

        public Path GetTargetPath()
        {
            if (ProjectTitle != null)
            {
                return new EnginePath("projects/" + ProjectTitle);
            }

            return null;
        }

        protected override void OnModified()
        {
            if (ProjectTitle != null)
            {
                string targetDir = GetTargetPath();
                if (Directory.Exists(targetDir))
                {
                    SetFailure($"Directory\n\n\"{targetDir}\"\n\nis occupied! Please pick a different project name.\nYou can always change the project name later.");
                }
                else
                {
                    SetPostText($"Will create a new folder with directory\n\n\"{targetDir}\"\n\nYou can always change the project name later.\nIcon is optional.");
                }
            }
        }

        protected override bool CheckForFailuresPreSubmit()
        {
            if (ProjectTitle == null)
            {
                SetFailure("Project title must not be empty!");
                return false;
            }
            if (Author == null)
            {
                SetFailure("Author title must not be empty!");
                return false;
            }

            // Directory checking
            Path pathToMake = GetTargetPath();

            if (Directory.Exists(pathToMake))
            {
                SetFailure(
                    $"Directory \n\n{GetTargetPath()}\n\nalready exists. Please pick a different project name.\n" +
                    "You may pick a temporary name that isn't taken for now and change the project name later by opening project.json. " +
                    "Renaming the folder is OK too.");
                return false;
            }
            if (ProjectTitle.Contains('/'))
            {
                SetFailure($"Project title \"{ProjectTitle}\" cannot contain forward slashes. Sorry!");
                return false;
            }
            if (!Directory.GetParent(pathToMake).Exists)
            {
                SetFailure($"Project cannot be created because path {Directory.GetParent(pathToMake)} does not exist. This is probably a bug!");
                return false;
            }

            return true;
        }
    }
}
