// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using GameEngine;
using GameEngine.Game;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Path = GameEngine.Game.Path;

namespace DREngine.Game.Scene
{
    [Serializable]
    public class DRScene : BaseSceneLoader
    {
        public List<ISceneObject> Objects { get; set; } = new List<ISceneObject>();

        private bool _testFlag;

        private Path _scenePath;

        private DRGame _game;

        public DRScene(DRGame game, string name, Path scenePath, bool testFlag) : base(game, name)
        {
            _game = game;
            _scenePath = scenePath;
            _testFlag = testFlag;
        }

        // Temporary constructor for when we are deserialized into a temporary object.
        public DRScene() : base(null, "")
        {
            Debug.LogDebug("Commencing Scene Deserialization...");
        }

        public override void LoadScene()
        {
            new FreeCamera3D(_game, Vector3.Zero, Quaternion.Identity);
            if (_testFlag)
            {
                Debug.Log("MAKING");

                ProjectPath sprPath = new ProjectPath(_game, "icon.png");
                Sprite spr = new Sprite(_game, sprPath);

                // Save directly
                Objects.Add(new Cube(_game,new Vector3(4, 2, 1), spr, new Vector3(1, 2, -3), Quaternion.Identity));
                Objects.Add(new Cube(_game,new Vector3(2, 1, 2), spr, new Vector3(4, 4, -3), Quaternion.Identity));
                Objects.Add(new Billboard(_game, new Vector3(6, 9, 0), Quaternion.Identity));
                Objects.Add(new Billboard(_game, new Vector3(1, 7, 3), Quaternion.Identity));
                Save();
            }
            else
            {
                Debug.Log("LOADING & SAVING");
                string text = IO.ReadTextFile(_scenePath);
                // Load & then save directly
                //SceneObjectJsonConverter.InitConversion((DRGame)_game);
                ISceneObject.CurrentGame = _game;
                var copy = JsonConvert.DeserializeObject<DRScene>(text, new JsonSerializerSettings(){TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented});
                if (copy != null)
                {
                    Objects.Clear();
                    Objects.AddRange(copy.Objects);
                    Save();
                }
                else
                {
                    Debug.LogError($"Invalid JSON file to read scene from: {_scenePath}. Please check to make sure the json at this path is a valid scene!");
                }
            }
        }

        private void Save()
        {

            /*
            using (StreamWriter writer = File.CreateText(_scenePath))
            {
                JsonTextWriter jwriter = new JsonTextWriter(writer);
                new SceneObjectJsonConverter((DRGame)_game).WriteJson(iwriter, );
            }
            */
            //SceneObjectJsonConverter.InitConversion((DRGame)_game);
            //IO.WriteTextFile(_scenePath, JsonConvert.SerializeObject(this, new JsonSerializerSettings(){TypeNameHandling = TypeNameHandling.Auto}));
            ISceneObject.CurrentGame = _game;
            IO.WriteTextFile(_scenePath, JsonConvert.SerializeObject(this, new JsonSerializerSettings(){TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented}));
            /* No custom conversion
            string jsonString = JsonSerializer.Serialize(this);
            IO.WriteTextFile(_scenePath, jsonString);
            */
            Debug.LogDebug($"Saved scene to {_scenePath}");
        }
    }
}
