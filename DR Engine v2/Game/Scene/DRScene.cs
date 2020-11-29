using System;
using System.Collections.Generic;
using DREngine.Game.VN;
using GameEngine;
using GameEngine.Game;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Path = GameEngine.Game.Path;

namespace DREngine.Game.Scene
{
    [Serializable]
    public class DRScene : BaseSceneLoader, IGameResource, IDependentOnDRGame
    {

        [JsonIgnore]
        public Path Path { get; set; }
        public List<ISceneObject> Objects { get; set; } = new List<ISceneObject>();

        private bool _testFlag;
        public DRGame Game { get; set; }

        public DRScene(DRGame game, string name, Path scenePath, bool testFlag) : base(game, name)
        {
            Game = game;
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
            new FreeCamera3D(Game, Vector3.Zero, Quaternion.Identity);
            TEST_DELETE_ME();
            var copy = JsonHelper.LoadFromJson<DRScene>(Game, Path);
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
            //IO.WriteTextFile(_scenePath, JsonConvert.SerializeObject(this, new JsonSerializerSettings(){TypeNameHandling = TypeNameHandling.Auto}));
            JsonHelper.SaveToJson(this, path);
            /* No custom conversion
            string jsonString = JsonSerializer.Serialize(this);
            IO.WriteTextFile(_scenePath, jsonString);
            */
            Debug.LogDebug($"Saved scene to {path}");
        }

        public void Load(GamePlus game)
        {
            // Nothing really. File reading happens later.
            Game = (DRGame)game;
        }

        public void Unload(GamePlus game)
        {
            // Nothing.
            Objects.Clear();
        }

        public void TEST_DELETE_ME()
        {
            VNScript script = new VNScript(Game);

            script.Commands = new List<VNCommand>(new VNCommand[]
            {
                new LabelCommand() {Label = "STARTO"},
                new PrintCommand() {Text = "poopity scoop"},
                new PrintCommand() {Text = "scoopity poop"},
                new LabelCommand() {Label = "ENDO"}
            });

            Game.VNRunner.State.CurrentScript = script;

            script.Save(new ProjectPath(Game, "TEST_SCRIPT.vn"));
        }
    }
}
