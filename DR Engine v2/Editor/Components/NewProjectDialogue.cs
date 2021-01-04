using System.IO;
using DREngine.Editor.SubWindows.FieldWidgets;
using GameEngine.Game;
using Gtk;
using Path = GameEngine.Game.Path;

namespace DREngine.Editor
{
    public class NewProjectDialogue : Dialog
    {
        //[EngineDirectoryField("projects", "Choose Project Parent Directory")]
        //public string ProjectParentDirectory = null;

        private DREditor _editor;

        public string ProjectTitle = "Example Project";
        public string Author = "Me";

        [EngineFileField("projects", "Choose Icon", "*.png", "Image")]
        public string IconPath = null;

        private FieldBox _fields;

        public NewProjectDialogue(DREditor editor, Window parent, string title="New Project") : base(title, parent, DialogFlags.DestroyWithParent, "Create", ResponseType.Accept, "Cancel", ResponseType.Cancel)
        {
            _editor = editor;

            _fields = new FieldBox(editor, typeof(NewProjectDialogue), true);
            ContentArea.PackStart(_fields, true, true, 16);

            Text post = new Text("Will create a new folder in the projects directory.\nYou can always change the project name later.\nIcon is optional.");

            _fields.Modified += () =>
            {
                if (ProjectTitle != null)
                {
                    string targetDir = GetTargetPath();
                    if (Directory.Exists(targetDir))
                    {
                        post.Text = $"Directory \n\n\"{targetDir}\"\n\n is occupied! Please pick a different project name.\nYou can always change the project name later.";
                        post.SetMode(Text.WARNING);
                    }
                    else
                    {
                        post.Text =
                            $"Will create a new folder with directory \n\n\"{targetDir}\"\n\nYou can always change the project name later.\nIcon is optional.";
                        post.SetMode(Text.NORMAL);
                    }
                }
            };

            ContentArea.PackEnd(post, false, false, 16);

            _fields.Show();
            post.Show();
            _fields.LoadTarget(this);
        }

        public Path GetTargetPath()
        {
            if (ProjectTitle != null)
            {
                return new EnginePath("projects/" + ProjectTitle);
            }

            return null;
        }
    }
}
