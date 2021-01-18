using System;

using DREngine.Game.Scene;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Objects;
using Microsoft.Xna.Framework;

namespace DREngine.Game.CoreScenes.SceneEditor
{
    public class EditorSceneEditorScene : BaseSceneLoader
    {
        private const string SCENE_NAME = "__SCENE_EDITOR_SCREEN__";

        private SceneEditorConnection _connection;

        private DRGame _game;

        private DRScene _loadedScene;

        private Path _scenePath;

        private SceneEditorCamera _camera;

        private TransformTranslator _translator;

        private ISceneObject _selected;

        public EditorSceneEditorScene(DRGame game, Path sceneToLoad) : base(game, SCENE_NAME)
        {
            _game = game;
            _scenePath = sceneToLoad;
            _connection = new SceneEditorConnection(game.EditorConnection);

            _connection.OnNewObject += AddNewObject;
            _connection.OnDeleteObject += DeleteObject;
            _connection.OnModifiedObject += ModifyObjectField;
            _connection.OnSelectObject += OnEditorSelectObject;
            _connection.OnSaveRequested += OnSaveRequested;
        }

        private void OnSaveRequested()
        {
            _loadedScene.Save(_scenePath);
        }

        private void OnEditorSelectObject(int obj)
        {
            try
            {
                ISceneObject sceneObject = _loadedScene.Objects[obj];
                SelectObject(sceneObject, true);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Invalid object index: {obj} {e}");
            }
        }

        private void AddNewObject(Type obj)
        {
            IDependentOnResourceData.CurrentGame = _game;

            object instance = Activator.CreateInstance(obj);

            if (instance is ISceneObject sceneObject)
            {
                _loadedScene.Objects.Add(sceneObject);
                SelectObject(sceneObject, true);
            }
            else
            {
                Debug.LogWarning($"Non Scene Object Type created: {obj}");
            }
        }

        private void DeleteObject(int obj)
        {
            throw new NotImplementedException();
        }

        private void ModifyObjectField(int arg1, string arg2, object arg3)
        {
            throw new NotImplementedException();
        }

        private void LoadSceneFile(Path path)
        {
            _scenePath = path;
            Debug.Log("TEMP DEBUG: Loaded scene file without actually placing its items.");
            _loadedScene = _game.ResourceLoader.GetResource<DRScene>(path);
            _loadedScene.Load(_game.ResourceLoaderData);
            // Load the scene into the current game. This is a bit jank but it works I think.
            _loadedScene.LoadSceneRaw(_game);
        }
    
        private void SelectObject(ISceneObject sceneObject, bool look, bool sendInfoToEditor = false)
        {
            _translator.SetActive(true);

            _selected = sceneObject;
            if (sceneObject is GameObjectRender3D object3d)
            {
                if (look)
                {
                    _camera.LookAt(sceneObject.FocusCenter, sceneObject.FocusDistance);
                }

                _translator.Transform.Position = object3d.Transform.Position;

                if (sendInfoToEditor)
                {
                    int index = _loadedScene.Objects.IndexOf(sceneObject);
                    if (index == -1)
                    {
                        Debug.LogWarning("Could not find index for 3D Editor selected object! This is an issue.");
                    }
                    else
                    {
                        _connection.SendSelected(index);
                    }
                }
            }
            else
            {
                Debug.Log($"Unable to select object as it is not a 3d object: {sceneObject}");
            }
        }

        private void OnTranslatorDrag(Vector3 delta)
        {
            if (_selected != null && _selected is GameObjectRender3D obj)
            {
                obj.Transform.Position += delta;
                _translator.Transform.Position = obj.Transform.Position;
            }
        }

        public override void LoadScene()
        {
            LoadSceneFile(_scenePath);

            if (_camera == null || _translator == null)
            {
                new SceneEditorGrid(_game);
                // Add a freecam to look around the scene. This will change to a Room Editor Camera with extra features later.
                _camera = new SceneEditorCamera(_game, Vector3.Zero, Quaternion.Identity);
                _translator = new TransformTranslator(_game, Vector3.Zero);

                _translator.SetActive(false);

                // When we select a collider, select it.
                _camera.ColliderSelected += collider =>
                {
                    // Prioritize translator selection over collider selection.
                    if (_translator.Selected) return;
                    if (collider.GameObject is ISceneObject sceneObject)
                    {
                        SelectObject(sceneObject, false, true);
                    }
                };

                _translator.XArrow.Dragged += delta =>
                {
                    OnTranslatorDrag(delta * Vector3.UnitX);
                };
                _translator.YArrow.Dragged += delta =>
                {
                    OnTranslatorDrag(delta * Vector3.UnitY);
                };
                _translator.ZArrow.Dragged += delta =>
                {
                    OnTranslatorDrag(delta * Vector3.UnitZ);
                };
            }
        }
    }
}
