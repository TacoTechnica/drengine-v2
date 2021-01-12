
using System;

namespace DREngine.Editor.SubWindows.Resources.SceneEditor
{
    public class SceneEditorConnection
    {
        private DREditor _editor;
        private DRProjectRunner _connection;

        public Action OnStop
        {
            get => _connection.OnStop;
            set => _connection.OnStop = value;
        }

        public bool Running => _connection.Running;

        public SceneEditorConnection(DREditor editor)
        {
            _editor = editor;
            _connection = new DRProjectRunner();
        }

        public void Open(string scene)
        {
            // For now, just cancel + reopen
            _connection.Stop();
            _connection.RunProject(_editor.ProjectData.GetFullProjectPath(), $"--sceneedit=\"{scene}\"");
        }

        public void Stop()
        {
            _connection.Stop();
        }
    }
}
