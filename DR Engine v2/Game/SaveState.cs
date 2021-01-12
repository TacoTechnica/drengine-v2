using System;
using DREngine.Game.VN;
using DREngine.ResourceLoading;
using GameEngine;
using GameEngine.Game;
using Newtonsoft.Json;

namespace DREngine.Game
{
    public class SaveState : IDependentOnResourceData
    {
        private readonly DRGame _game;


        public string ProjectName;
        public DateTime TimeSaved;
        public VNState VNState
        {
            get => _game.VNRunner.State;
            set => _game.VNRunner.State = value;
        }


        public SaveState(DRGame game)
        {
            _game = game;
        }

        public SaveState() : this(IDependentOnResourceData.CurrentGame)
        {
        }


        public void Save(Path file)
        {
            Debug.LogDebug($"SAVING GAME to {file}");
            TimeSaved = DateTime.Now;
            ProjectName = _game.GameData.Name;

            JsonHelper.SaveToJson(this, file);
        }

        public bool Load(Path file)
        {
            Debug.LogDebug($"LOADING GAME from {file}");
            var copy = JsonHelper.LoadFromJson<SaveState>(_game, file);

            var currentProject = _game.GameData.Name;
            if (copy.ProjectName != currentProject)
            {
                var noProject = currentProject == "";
                if (noProject)
                    Debug.LogError($"Tried to load save file for project \"{copy.ProjectName}\" without loading a " +
                                   $"project first. Please load project \"{copy.ProjectName}\" before trying to load this save file.");
                else
                    Debug.LogError("Tried to load a save file from a different project!\n" +
                                   $"You currently have project \"{currentProject}\" " +
                                   $"opened, while the save is from \"{copy.ProjectName}\".\nIf this isn't expected behaviour " +
                                   "(like if the project was renamed), please fix this by " +
                                   $"opening the the save file at {file} in a text editor and changing" +
                                   "the \"ProjectName\" paramter " +
                                   $"to be equal to \"{currentProject}\"."
                    );

                return false;
            }

            // TODO: Make this cleaner/automatic.
            ProjectName = copy.ProjectName;
            TimeSaved = copy.TimeSaved;
            VNState = copy.VNState;
            Debug.LogDebug($"LOADING GAME from {file} Success!");
            return true;
        }
    }
}