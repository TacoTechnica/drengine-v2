using System;
using DREngine.Game;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Debugging;
using Newtonsoft.Json;

namespace DREngine
{
    /// <summary>
    /// Append this to ALL serializable fields that use a project resource.
    /// </summary>
    internal class ProjectResourceConverter : JsonConverter
    {
        private static DRGame _currentGame = null;

        public const string RESOURCE_PATH_PREFIX = "DRPROJECT://";
        public const string DEFAULT_RESOURCE_PATH_PREFIX = "DEFAULT://";
        public const string INVALID_PATH_SIGNIFIER = "###INVALID###";
        public const string NULL_PATH_SIGNIFIER = "(empty)";

        public static void OnInitGame(DRGame game)
        {
            _currentGame = game;
        }

        public override bool CanWrite { get; } = true;
        public override bool CanRead { get; } = true;

        public override void WriteJson(JsonWriter writer, object? o, JsonSerializer serializer)
        {
            IGameResource value = (IGameResource) o;
            //Debug.Log($"!!!!!!!!!!!!!!GOT RESOURCE: {value.Path}");
            string storedPath;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // Technically value is always not null, but juuuust in case.....
            if (value == null || value.Path == "")
            {
                storedPath = NULL_PATH_SIGNIFIER;
            }
            else
            {
                try
                {
                    if (_currentGame.GameProjectData.IsDefaultResourcePath(value.Path))
                    {
                        storedPath =
                            $"{DEFAULT_RESOURCE_PATH_PREFIX}{_currentGame.GameProjectData.GetRelativeDefaultResourcePath(value.Path)}";
                    }
                    else
                    {
                        // Project path
                        storedPath =
                            $"{RESOURCE_PATH_PREFIX}{_currentGame.GameProjectData.GetRelativeProjectPath(value.Path)}";
                    }
                }
                catch (InvalidArgumentsException e)
                {
                    storedPath = INVALID_PATH_SIGNIFIER;
                }
            }

            //Debug.Log($"!!!!!!!!!!!!!!WROTE {projectPath}");
            writer.WriteValue(storedPath);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? value, JsonSerializer jsonSerializer)
        {
            string data = (string)reader.Value;
            //Debug.Log($"!!!!!!!!!!!!!!READING RESOURCE: {data}");

            if (data == NULL_PATH_SIGNIFIER)
            {
                // Intentionally empty path
                return null;
            }

            if (data == INVALID_PATH_SIGNIFIER)
            {
                // Bad path
                return null;
            }

            // Convert stored path to full path
            string fullPath;
            if (data.StartsWith(RESOURCE_PATH_PREFIX))
            {
                string projectRelativePath = data.Substring(RESOURCE_PATH_PREFIX.Length);
                fullPath = new ProjectPath(_currentGame, projectRelativePath);
            } else if (data.StartsWith(DEFAULT_RESOURCE_PATH_PREFIX))
            {
                string defaultRelativePath = data.Substring(DEFAULT_RESOURCE_PATH_PREFIX.Length);
                fullPath = new DefaultResourcePath(defaultRelativePath);
            }
            else
            {
                return null;
            }
            //Debug.Log($"FULL PATH: {data} => {fullPath}, TYPE: {objectType}");
            return _currentGame.ProjectResources.GetResource(fullPath, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IGameResource).IsAssignableFrom(objectType);
        }
    }
}
