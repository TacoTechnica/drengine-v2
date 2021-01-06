using System.IO;
using DREngine.Editor.SubWindows.FieldWidgets;
using Gtk;

namespace DREngine.Editor.Components
{
    // TODO: Reduce Code duplication with sprite & audio clip.
    public class NewFontDialog : NewItemDialog
    {
        [EngineFileField("projects", "Choose Font to Copy", "*.ttf", "Font File")]
        public string FontToCopy = null;

        //private Button _systemFontButton;

        public NewFontDialog(DREditor editor, Window parent, ProjectPath parentDirectory, string title = "New Folder") :
            base(editor, parent, parentDirectory, title)
        {
            /*
            _systemFontButton = new Button("Load System Font");
            this.ContentArea.PackStart(_systemFontButton, false, false, 16);
            this.ContentArea.ReorderChild(_systemFontButton, 0);
            _systemFontButton.Show();

            _systemFontButton.Pressed += (sender, args) =>
            {

                using FontChooserDialog window = new FontChooserDialog("Font choose", this);
                ResponseType type = (ResponseType) window.Run();
                Debug.Log($"RESPONDED: {type}");
                
                Debug.Log($"{window.FontDesc?.ToFilename()} => {window.Font}, {window.FontFace?.Data?.Count}, {window.FontFamily?.Name}, {window.FontFamily?.Data?.Count}");

            };
            */
        }

        protected override string ItemName => "Font";

        protected override void OnModified()
        {
            base.OnModified();
            if (!File.Exists(FontToCopy))
                SetFailure($"Font file does not exist at path {FontToCopy}");
            else if (!FontToCopy.EndsWith(".ttf"))
                SetFailure("Only TTF Fonts are supported right now. Sorry!");
            else if (!Failed())
                SetPostText($"Will copy font from\n\n{FontToCopy}\n\nto\n\n{GetTargetDirectory().GetShortName()}\n\n" +
                            "NOTE: You may also copy your font(s) into the project directly and reload the project (File->Reload Project)!");
        }

        protected override bool CheckForFailuresPreSubmit()
        {
            if (string.IsNullOrEmpty(FontToCopy))
            {
                SetFailure("Font not set!");
                return false;
            }

            return base.CheckForFailuresPreSubmit();
        }

        public override ProjectPath GetTargetDirectory()
        {
            var result = base.GetTargetDirectory();
            if (!result.ToString().EndsWith(".ttf")) return (ProjectPath) (result + ".ttf");
            return result;
        }
    }
}