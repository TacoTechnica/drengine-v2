using System.Collections.Generic;
using GameEngine;
using GameEngine.Game;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace DREngine.Game.VN
{
    public class VNScript : IGameResource
    {
        [JsonIgnore]
        public Path Path { get; set; }

        public List<VNCommand> Commands;

        private DRGame _game;

        // Manual constructor
        public VNScript(DRGame game, Path path = null)
        {
            _game = game;
            Path = path;
            Commands = new List<VNCommand>();
            if (path != null) Load(game);
        }
        // Deserialize constructor
        public VNScript()
        {
            Commands = new List<VNCommand>();
        }
        public void Load(GamePlus game)
        {
            _game = (DRGame) game;

            var copy = JsonHelper.LoadFromJson<VNScript>(_game, Path);
            if (copy != null)
            {
                Commands.Clear();
                Commands.AddRange(copy.Commands);
                // Update each command
                int i = 0;
                foreach (VNCommand command in Commands)
                {
                    command.CommandIndex = i++;

                    if (command is LabelCommand label)
                    {
                        Debug.Log($"GOT LABEL! {label.Label}");
                    }
                }
            }
        }

        public void Save(Path path)
        {
            Path = path;
            JsonHelper.SaveToJson(this, path);
        }

        public void Unload(GamePlus game)
        {
            Commands.Clear();
        }
    }
}
