using System;
using System.Collections.Generic;
using DREngine.Game.VN;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Objects;
using GameEngine.Game.Resources;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Path = GameEngine.Game.Path;

namespace DREngine.Game.Scene
{
    [Serializable]
    public class DRScene : BaseSceneLoader, IGameResource
    {

        [JsonIgnore]
        public Path Path { get; set; }
        public List<ISceneObject> Objects { get; set; } = new List<ISceneObject>();

        private bool _testFlag;
        private DRGame _game;

        public DRScene(DRGame game, string name, Path scenePath, bool testFlag) : base(game, name)
        {
            _game = game;
            Path = scenePath;
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
            //TEST_DELETE_ME();
            var copy = JsonHelper.LoadFromJson<DRScene>(_game, Path);
            if (copy != null)
            {
                Objects.Clear();
                Objects.AddRange(copy.Objects);
            }
            else
            {
                Debug.LogError($"Invalid JSON file to read scene from: {Path}. Please check to make sure the json at this path is a valid scene!");
            }
        }

        public void Save(Path path)
        {
            /*
            using (StreamWriter writer = File.CreateText(_scenePath))
            {
                JsonTextWriter jwriter = new JsonTextWriter(writer);
                new SceneObjectJsonConverter((DRGame)_game).WriteJson(iwriter, );
            }
            */
            //SceneObjectJsonConverter.InitConversion((DRGame)_game);
            //IOHelper.WriteTextFile(_scenePath, JsonConvert.SerializeObject(this, new JsonSerializerSettings(){TypeNameHandling = TypeNameHandling.Auto}));
            JsonHelper.SaveToJson(this, path);
            /* No custom conversion
            string jsonString = JsonSerializer.Serialize(this);
            IOHelper.WriteTextFile(_scenePath, jsonString);
            */
            Debug.LogDebug($"Saved scene to {path}");
        }

        public void Load(ResourceLoaderData data)
        {
            // Nothing really. File reading happens later.
            //Game = (DRGame)game;
        }

        public void Unload()
        {
            // Nothing.
            Objects.Clear();
        }

        public void TEST_DELETE_ME()
        {
            VNScript script = new VNScript(_game);

            script.Commands = new List<VNCommand>(new VNCommand[]
            {
                new LabelCommand() {Label = "STARTO"},
                new PrintCommand() {Text = "poopity scoop"},
                new PrintCommand() {Text = "scoopity poop"},
                new LabelCommand() {Label = "ENDO"}
            });

            //_game.VNRunner.State.CurrentScript = script;

            script.Save(new ProjectPath(_game, "TEST_SCRIPT.vn"));
        }
    }
}
