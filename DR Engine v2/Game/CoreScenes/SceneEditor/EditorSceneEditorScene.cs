using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DREngine.Game.Scene;
using DREngine.ResourceLoading;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Objects;
using GameEngine.Game.Resources;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

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
            _connection.OnResourceModifiedObject += ModifyResourceField;
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

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private void ModifyObjectField(int index, string fieldName, string data)
        {

            ISceneObject sceneObject = null;
            try
            {
                sceneObject = _loadedScene.Objects[index];
                if (sceneObject == null) throw new InvalidOperationException("Null object");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Invalid object index: {index} {e}");
            }

            try
            {
                UniFieldInfo f = sceneObject.GetType().GetUniField(fieldName);

                object newValue = JsonConvert.DeserializeObject(data, f.FieldType,
                    new JsonSerializerSettings
                        {TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented});

                // TODO: Is this necessary? Streamline this field de-serialization process please.
                newValue = ExtraResourceHelper.ConvertObjectFromJson(newValue, f.FieldType);
                f.SetValue(sceneObject, newValue);
            }
            catch (Exception)
            {
                Debug.LogWarning("FAILED TO UPDATE FIELD.");
                throw;
            }
        }

        private void ModifyResourceField(int index, string fieldName, Path path)
        {
            ISceneObject sceneObject = null;
            try
            {
                sceneObject = _loadedScene.Objects[index];
                if (sceneObject == null) throw new InvalidOperationException("Null object");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Invalid object index: {index} {e}");
            }

            try
            {
                UniFieldInfo f = sceneObject?.GetType().GetUniField(fieldName);

                _game.ResourceLoader.FinishUsingResource((IGameResource)f?.GetValue(sceneObject));
                var newValue = _game.ResourceLoader.GetResource(path, f?.FieldType);

                f?.SetValue(sceneObject, newValue);
            }
            catch (Exception)
            {
                Debug.LogWarning("FAILED TO UPDATE FIELD.");
                throw;
            }
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

                _translator.Target = object3d;

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
                _translator.Target = null;
                Debug.Log($"Unable to select object as it is not a 3d object: {sceneObject}");
            }
        }

        private void OnTranslatorDrag(Vector3 delta)
        {
            if (_selected != null && _selected is GameObjectRender3D obj)
            {
                obj.Transform.Position += delta;
                _connection.SendTransformChanged(_loadedScene.Objects.IndexOf(_selected), obj.Transform);
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
