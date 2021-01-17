using System;
using DREngine.Game.Scene;
using DREngine.ResourceLoading;
using GameEngine;
using GameEngine.Game;
using Gtk;

namespace DREngine.Editor.SubWindows.Resources.SceneEditor
{
    public class SceneResourceWindow : ResourceWindow<DRScene>
    {
        private SceneEditorConnection _connection;
        private SceneObjectList _list;

        private DREditor _editor;

        public SceneResourceWindow(DREditor editor, ProjectPath resPath) : base(editor, resPath)
        {
            _editor = editor;
            _connection = new SceneEditorConnection(editor);

            _connection.OnSaved += () =>
            {
                MarkDirty(false);
            };
            _connection.OnSelected += selectedIndex =>
            {
                _list.ForceSelect(selectedIndex);
            };
        }

        protected override void OnInitialize(Box container)
        {
            _list = new SceneObjectList(_editor);
            Add(_list);

            _list.NewObjectAdded += type =>
            {
                _connection.SendNewObject(type);
                MarkDirty();
            };
            _list.ObjectSelected += objectIndex =>
            {
                _connection.SendSelectObject(objectIndex);
            };

            // Close when we close the editor.
            _connection.OnStop += () =>
            {
                this.Close();
                this.Dispose();
            };

            RequestMinSize(100, 400);
        }

        protected override void OnOpen(DRScene resource, Box container)
        {
            // Extra load must be run on scenes.
            resource.LoadScene();

            _list.LoadItems(resource.Objects);

            // Tell our DR Game Window to load the scene.
            _connection.Open(resource.Path);
        }

        protected override void OnLoadError(bool fileExists, Exception exception)
        {
            throw exception;
        }

        protected override void OnClose()
        {
            // Close all other windows
            if (_connection.Running)
            {
                _connection.Stop();
            }
        }

        // The DR scene window actually holds on to the scene.
        protected override void OnSave(Path path)
        {
            _connection.SendSave();
            if (!_connection.WaitForSave(5.0))
            {
                MarkDirty();
                throw new InvalidOperationException($"Failed to save scene to path {path}, probably a networking issue!");
            }
        }
    }
}
