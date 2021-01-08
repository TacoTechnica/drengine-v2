using System.Collections.Generic;
using DREngine.ResourceLoading;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Resources;
using Newtonsoft.Json;

namespace DREngine.Game.VN
{
    public class VNScript : IGameResource
    {
        private DRGame _game;

        public List<VNCommand> Commands;

        [JsonIgnore] public int CommandCount => Commands.Count;

        // Manual constructor
        public VNScript(DRGame game, Path path = null)
        {
            _game = game;
            Path = path;
            Commands = new List<VNCommand>();
            if (path != null) Load(game.ResourceLoaderData);
        }

        // Deserialize constructor
        public VNScript()
        {
            Commands = new List<VNCommand>();
        }

        [JsonIgnore] public Path Path { get; set; }

        public void Load(ResourceLoaderData data)
        {
            var copy = JsonHelper.LoadFromJson<VNScript>(null, Path);
            if (copy != null)
            {
                Commands.Clear();
                Commands.AddRange(copy.Commands);
                // Update each command
                var i = 0;
                foreach (var command in Commands)
                {
                    command.CommandIndex = i++;

                    if (command is LabelCommand label) Debug.Log($"GOT LABEL! {label.Label}");
                }
            }
        }

        public VNCommand Get(int number)
        {
            return Commands[number];
        }

        public void Save(Path path)
        {
            Path = path;
            JsonHelper.SaveToJson(this, path);
        }

        public void Unload()
        {
            Commands.Clear();
        }
    }
}