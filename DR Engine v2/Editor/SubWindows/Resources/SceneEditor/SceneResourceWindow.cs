using System;
using DREngine.Game.Scene;
using DREngine.ResourceLoading;
using GameEngine;
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
        }

        protected override void OnInitialize(Box container)
        {
            _list = new SceneObjectList(_editor);
            Add(_list);

            _list.NewItemAdded += type =>
            {
                Debug.Log("NEW ITEM: TODO");
                // TODO: Modify scene type (add)
                // TODO: Alert game of new item
            };
            _list.ItemSelected += i =>
            {
                Debug.Log("SELECTION: TODO");
                // TODO: Alert game of selection
            };

            _connection = new SceneEditorConnection(_editor);

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
            // Extra load must be done on scenes.
            resource.LoadScene();

            _list.Clear();

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

    }
}
