using System;
using DREngine.Game.Scene;
using DREngine.ResourceLoading;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Resources;
using Gtk;
using Debug = System.Diagnostics.Debug;

namespace DREngine.Editor.SubWindows.Resources.SceneEditor
{
    public class SceneResourceWindow : ResourceWindow<DRScene>
    {
        private SceneEditorConnection _connection;
        private SceneObjectList _list;
        private SceneObjectFields _fields;

        private DREditor _editor;

        private int _currentSelected = -1;

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
                _currentSelected = selectedIndex;
                _list.ForceSelect(selectedIndex);
            };
        }

        protected override void OnInitialize(Box container)
        {
            _list = new SceneObjectList(_editor);
            Add(_list);

            _fields = new SceneObjectFields(_editor);
            Window fieldWindow = new Window(WindowType.Toplevel);
            fieldWindow.Add(_fields);
            _fields.Show();
            fieldWindow.Show();
            /*
            fieldWindow.Destroyed += (sender, args) =>
            {
                this.Close();
                this.Dispose();
            };
            */

            _list.NewObjectAdded += type =>
            {
                _connection.SendNewObject(type);
                MarkDirty();
            };
            _list.ObjectSelected += objectIndex =>
            {
                _currentSelected = objectIndex;
                _connection.SendSelectObject(objectIndex);
                ISceneObject currentObject = CurrentResource.Objects[objectIndex]; 
                _fields.LoadObject(currentObject);
                fieldWindow.Title = $"Currently Editing: {currentObject.Name ?? "(unnamed)"}";
            };

            _fields.FieldModified += (name, obj) =>
            {
                // All resources are modified separately since we're in a scene.
                if (obj is IGameResource resource)
                {
                    if (resource.Path is ProjectPath)
                    {
                        _connection.SendPropertyModifiedResource(_currentSelected, name, (ProjectPath) resource.Path);
                    }
                    else
                    {
                        throw new InvalidOperationException("Resource path is not a ProjectPath. If it's a Default Path, this should be patched right away!.");
                    }
                }
                else
                {
                    _connection.SendPropertyModified(_currentSelected, name, obj);
                }
            };

            // Close when we close the editor.
            _connection.OnStop += () =>
            {
                this.Close();
                this.Dispose();
                fieldWindow.Close();
                fieldWindow.Dispose();
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
