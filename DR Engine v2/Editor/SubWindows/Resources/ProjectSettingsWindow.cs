using System;
using DREngine.Editor.Components;
using DREngine.Editor.SubWindows.FieldWidgets;
using DREngine.ResourceLoading;
using GameEngine.Game;
using Gtk;

namespace DREngine.Editor.SubWindows.Resources
{
    public class ProjectSettingsWindow : SavableWindow
    {
        private readonly DREditor _editor;

        private FieldBox _fields;

        private Text _message;

        public ProjectSettingsWindow(DREditor editor, ProjectPath resPath) : base(editor, resPath)
        {
            _editor = editor;
        }

        protected override void OnInitialize(Box container)
        {
            _message = new Text("");
            _fields = new FieldBox(_editor, typeof(ProjectData))  {AutoApply = true};
            _fields.Modified += (name, obj) =>
            {
                MarkDirty();
            };


            container.PackStart(_message, false, false, 4);
            container.PackStart(_fields, true, true, 4);

            _message.Show();
            _fields.Show();
        }

        protected override void OnOpen(Path path, Box container)
        {
            _message.Text = "PROJECT SETTINGS";

            var data = _editor.ProjectData;
            if (data == null) throw new InvalidOperationException("No project data currently loaded by the editor.");

            _fields.LoadTarget(data);
        }

        protected override void OnSave(Path path)
        {
            _fields.SaveFields();
            // Also save to disk
            var data = (ProjectData) _fields.Target;
            ProjectData.WriteToFile(path, data);
        }

        protected override void OnLoadError(bool fileExists, Exception exception)
        {
            _message.Text = exception.ToString();
        }

        protected override void OnClose()
        {
            // Reload project if we modified/failed to modify so it reflects the file state.
            _editor.ProjectData = ProjectData.LoadFromFile(CurrentPath);
        }
    }
}