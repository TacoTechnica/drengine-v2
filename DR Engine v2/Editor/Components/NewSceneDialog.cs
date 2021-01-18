using DREngine.ResourceLoading;
using Gtk;

namespace DREngine.Editor.Components
{
    public class NewSceneDialog : NewItemDialog
    {
        public NewSceneDialog(DREditor editor, Window parent, ProjectPath parentDirectory, string title = "New Scene") : base(editor, parent, parentDirectory, title)
        {
        }

        protected override string ItemName => "Scene";

        public override ProjectPath GetTargetDirectory()
        {
            var result = base.GetTargetDirectory();
            if (!result.ToString().EndsWith(".scene")) return (ProjectPath) (result + ".scene");
            return result;
        }
    }
}
