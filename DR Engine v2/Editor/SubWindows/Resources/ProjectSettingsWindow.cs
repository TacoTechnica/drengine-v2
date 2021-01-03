using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DREngine.Editor.SubWindows.FieldWidgets;
using GameEngine;
using Gtk;

namespace DREngine.Editor.SubWindows.Resources
{
    public class ProjectSettingsWindow : SavableWindow
    {
        private DREditor _editor;

        private FieldBox _fields;

        private Label _message;

        public ProjectSettingsWindow(DREditor editor, ProjectPath resPath) : base(editor, resPath)
        {
            _editor = editor;
        }

        protected override void OnInitialize(Box container)
        {
            _message = new Label();
            _fields = new FieldBox(typeof(ProjectData));
            _fields.Modified += () =>
            {
                MarkDirty();
            };
            
            container.PackStart(_message, false, false, 4);
            container.PackStart(_fields, true, true, 4);
            
            _message.Show();
            _fields.Show();
        }

        protected override void OnOpen(string path, Box container)
        {
            _message.Text = "PROJECT SETTINGS";

            ProjectData data = _editor.ProjectData;
            if (data == null)
            {
                throw new InvalidOperationException("No project data currently loaded by the editor.");
            }

             _fields.LoadTarget(data);
        }

        protected override void OnSave(string path)
        {
            _fields.SaveFields();
            // Also save to disk
            ProjectData data = (ProjectData) _fields.Target;
            ProjectData.WriteToFile(path, data);
        }

        protected override void OnLoadError(bool fileExists, Exception exception)
        {
            _message.Text = exception.ToString();
        }

        protected override void OnClose()
        {
            // Nothing
        }
    }
}
