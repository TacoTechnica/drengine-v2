using System;
using DREngine.Game.VN;
using GameEngine;
using GameEngine.Game;
using Newtonsoft.Json;

namespace DREngine.Game
{
    public class SaveState : IDependentOnDRGame
    {
        public string ProjectName;
        public DateTime TimeSaved;

        public SaveState(DRGame game)
        {
            Game = game;
        }

        public SaveState() : this(IDependentOnDRGame.CurrentGame)
        {
        }

        public VNState VNState
        {
            get => Game.VNRunner.State;
            set => Game.VNRunner.State = value;
        }

        [JsonIgnore] public DRGame Game { get; set; }

        public void Save(Path file)
        {
            Debug.LogDebug($"SAVING GAME to {file}");
            TimeSaved = DateTime.Now;
            ProjectName = Game.GameProjectData.Name;

            JsonHelper.SaveToJson(this, file);
        }

        public bool Load(Path file)
        {
            Debug.LogDebug($"LOADING GAME from {file}");
            var copy = JsonHelper.LoadFromJson<SaveState>(Game, file);

            var currentProject = Game.GameProjectData.Name;
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

            // TODO: Make this cleaner.
            ProjectName = copy.ProjectName;
            TimeSaved = copy.TimeSaved;
            VNState = copy.VNState;
            Debug.LogDebug($"LOADING GAME from {file} Success!");
            return true;
        }
    }
}