using System;
using DREngine.Game.Scene;
using DREngine.ResourceLoading;
using Gtk;

namespace DREngine.Editor.SubWindows.Resources.SceneEditor
{
    public class SceneResourceWindow : ResourceWindow<DRScene>
    {
        private SceneEditorConnection _connection;

        public SceneResourceWindow(DREditor editor, ProjectPath resPath) : base(editor, resPath)
        {
            _connection = new SceneEditorConnection(editor);

            // Close when we close the editor.
            _connection.OnStop += () =>
            {
                this.Close();
                this.Dispose();
            };
        }

        protected override void OnInitialize(Box container)
        {
            // TODO: Open left "Object List" window 
        }

        protected override void OnOpen(DRScene resource, Box container)
        {
            // TODO: Reset editor windows
            
            // Tell our DR Game Window to load the scene.
            _connection.Open(resource.Path);
        }

        protected override void OnLoadError(bool fileExists, Exception exception)
        {
            throw exception;
        }

        protected override void OnClose()
        {
            // TODO: Close all windows
            if (_connection.Running)
            {
                _connection.Stop();
            }
        }

    }
}
