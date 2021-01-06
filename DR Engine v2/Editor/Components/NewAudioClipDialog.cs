using System.IO;
using DREngine.Editor.SubWindows.FieldWidgets;
using Gtk;

namespace DREngine.Editor.Components
{
    // TODO: Reduce Code duplication with sprite & font
    public class NewAudioClipDialog : NewItemDialog
    {
        [EngineFileField("projects", "Choose Audio Clip to Copy", "*.wav", "Audio File")]
        public string AudioToCopy = null;

        public NewAudioClipDialog(DREditor editor, Window parent, ProjectPath parentDirectory, string title = "New Folder") : base(editor, parent, parentDirectory, title)
        {
        }

        protected override void OnModified()
        {
            base.OnModified();
            if (!File.Exists(AudioToCopy))
            {
                SetFailure($"Audio file does not exist at path {AudioToCopy}");
            } else if (!AudioToCopy.EndsWith(".wav"))
            {
                SetFailure("Only WAV Audio Files are supported right now. Sorry!");
            } else if (!Failed())
            {
                SetPostText($"Will copy audio file from\n\n{AudioToCopy}\n\nto\n\n{GetTargetDirectory().GetShortName()}\n\n" +
                            "NOTE: You may also copy your font(s) into the project directly and reload the project (File->Reload Project)!");
            }
        }

        public override ProjectPath GetTargetDirectory()
        {
            ProjectPath result = base.GetTargetDirectory();
            if (!result.ToString().EndsWith(".wav"))
            {
                return (ProjectPath)(result + ".wav");
            }
            return result;
        }

        protected override string ItemName => "Audio Clip";
    }
}