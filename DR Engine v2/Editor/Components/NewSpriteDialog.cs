using System.IO;
using DREngine.Editor.SubWindows.FieldWidgets;
using Gtk;

namespace DREngine.Editor.Components
{
    // TODO: Reduce code duplication with Font and Audio Clip.
    public class NewSpriteDialog : NewItemDialog
    {
        [EngineFileField("projects", "Choose Image to Copy", "*.png", "Image")]
        public string ImageToCopy = null;

        public NewSpriteDialog(DREditor editor, Window parent, ProjectPath parentDirectory, string title = "New Folder")
            : base(editor, parent, parentDirectory, title)
        {
        }

        protected override string ItemName => "Sprite";

        protected override void OnModified()
        {
            base.OnModified();
            if (!File.Exists(ImageToCopy))
                SetFailure($"Image file does not exist at path {ImageToCopy}");
            else if (!ImageToCopy.EndsWith(".png"))
                SetFailure("Only PNG images are supported right now. Sorry!");
            else if (!Failed())
                SetPostText(
                    $"Will copy sprite from\n\n{ImageToCopy}\n\nto\n\n{GetTargetDirectory().GetShortName()}\n\n" +
                    "NOTE: You may also copy your sprite(s) into the project directly and reload the project (File->Reload Project)!");
        }

        public override ProjectPath GetTargetDirectory()
        {
            var result = base.GetTargetDirectory();
            if (!result.ToString().EndsWith(".png")) return (ProjectPath) (result + ".png");
            return result;
        }
    }
}