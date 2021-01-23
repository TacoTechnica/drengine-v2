using System;
using DREngine.Game.Scene;
using DREngine.ResourceLoading;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Objects;
using GameEngine.Game.Resources;
using Gtk;

namespace DREngine.Editor.SubWindows.Resources.SceneEditor
{
    public class SceneResourceWindow : ResourceWindow<DRScene>
    {
        private SceneEditorConnection _connection;
        private SceneObjectList _list;
        private SceneObjectFields _fields;

        private DREditor _editor;

        private int _currentSelected = -1;

        private Window _fieldWindow;

        private bool _invokingFlag;

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
                SelectField(selectedIndex);
            };
            _connection.OnTransformModified += SetTransform;
        }

        protected override void OnInitialize(Box container)
        {
            _list = new SceneObjectList(_editor);
            Add(_list);

            _fields = new SceneObjectFields(_editor);
            _fieldWindow = new Window(WindowType.Toplevel);
            _fieldWindow.Add(_fields);
            _fields.Show();
            _fieldWindow.Show();
            /*
            fieldWindow.Destroyed += (sender, args) =>
            {
                this.Close();
                this.Dispose();
            };
            */

            _list.NewObjectAdded += type =>
            {
                ISceneObject newObject = (ISceneObject)Activator.CreateInstance(type);;

                if (newObject == null) throw new InvalidOperationException($"Type {type} failed to create an ISceneObject!");

                CurrentResource.Objects.Add(newObject);

                _connection.SendNewObject(type);
                MarkDirty();
            };
            _list.ObjectSelected += objectIndex =>
            {
                _currentSelected = objectIndex;
                _connection.SendSelectObject(objectIndex);
                SelectField(objectIndex);
            };

            _fields.FieldModified += (name, obj) =>
            {
                //Debug.Log($"MODIFIED FIELD: {name}");
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
                    // Special case: We are modifying the object's name.
                    if (name == "Name")
                    {
                        if (obj == null || obj == "")
                        {
                            _list.ResetObjectName(_currentSelected);
                        }
                        else
                        {
                            _list.RenameObject(_currentSelected, obj.ToString());
                        }
                    }
                    _connection.SendPropertyModified(_currentSelected, name, obj);
                }
                MarkDirty();
            };

            // Close when we close the editor.
            _connection.OnStop += () =>
            {
                this.Close();
                this.Dispose();
                _fieldWindow.Close();
                _fieldWindow.Dispose();
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

        private void SelectField(int objectIndex)
        {
            if (_invokingFlag) return;
            _invokingFlag = true;
            Application.Invoke((o, e) =>
            {
                ISceneObject currentObject = CurrentResource.Objects[objectIndex];
                if (currentObject == null) throw new InvalidOperationException("Tried selecting a null object, what?");
                _fields.LoadObject(currentObject);
                _fieldWindow.Title = $"Currently Editing: {currentObject.Name ?? "(unnamed)"}";
                _invokingFlag = false;
            });
        }
        private void SetTransform(int objectIndex, Transform3D transform)
        {
            ISceneObject currentObject = CurrentResource.Objects[objectIndex];
            if (currentObject is GameObjectRender3D object3d)
            {
                object3d.Transform = transform;
                MarkDirty();
            }
            else
            {
                Debug.LogWarning($"(tried applying transform) Scene Object {currentObject} is not a 3D object! It is a {currentObject.GetType()}");
            }
            SelectField(objectIndex);
        }
    }
}
