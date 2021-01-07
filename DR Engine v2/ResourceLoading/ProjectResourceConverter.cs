using System;
using System.IO;
using DREngine.Game;
using GameEngine.Game.Debugging;
using GameEngine.Game.Resources;
using Newtonsoft.Json;

namespace DREngine.ResourceLoading
{
    /// <summary>
    ///     Append this to ALL serializable fields that use a project resource.
    /// </summary>
    internal class ProjectResourceConverter : JsonConverter
    {
        public const string RESOURCE_PATH_PREFIX = "DRPROJECT://";
        public const string DEFAULT_RESOURCE_PATH_PREFIX = "DEFAULT://";
        public const string INVALID_PATH_SIGNIFIER = "###INVALID###://";
        public const string NULL_PATH_SIGNIFIER = "(empty)";
        private static DRGame _currentGame;

        public override bool CanWrite { get; } = true;
        public override bool CanRead { get; } = true;

        public static void OnInitGame(DRGame game)
        {
            _currentGame = game;
        }

        public override void WriteJson(JsonWriter writer, object o, JsonSerializer serializer)
        {
            var value = (IGameResource) o;
            //Debug.Log($"!!!!!!!!!!!!!!GOT RESOURCE: {value.Path}");
            string storedPath;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // Technically value is always not null, but juuuust in case.....
            if (value == null || value.Path == "")
                storedPath = NULL_PATH_SIGNIFIER;
            else
                try
                {
                    if (_currentGame.GameProjectData.IsDefaultResourcePath(value.Path))
                        storedPath =
                            $"{DEFAULT_RESOURCE_PATH_PREFIX}{_currentGame.GameProjectData.GetRelativeDefaultResourcePath(value.Path)}";
                    else
                        // Project path
                        storedPath =
                            $"{RESOURCE_PATH_PREFIX}{_currentGame.GameProjectData.GetRelativeProjectPath(value.Path)}";
                }
                catch (InvalidArgumentsException)
                {
                    // Get path relative to program path, so we have something to work with.
                    var dir = Program.RootDirectory;
                    var relativeProgramPath = Path.GetRelativePath(dir, value.Path);
                    storedPath = $"{INVALID_PATH_SIGNIFIER}{relativeProgramPath}";
                }

            //Debug.Log($"!!!!!!!!!!!!!!WROTE {projectPath}");
            writer.WriteValue(storedPath);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object value,
            JsonSerializer jsonSerializer)
        {
            var data = (string) reader.Value;
            //Debug.Log($"!!!!!!!!!!!!!!READING RESOURCE: {data}");

            if (data == NULL_PATH_SIGNIFIER || data == null)
                // Intentionally empty path
                return null;

            if (data.StartsWith(INVALID_PATH_SIGNIFIER))
                // Bad path
                return null;

            // Convert stored path to full path
            string fullPath;
            if (data.StartsWith(RESOURCE_PATH_PREFIX))
            {
                var projectRelativePath = data.Substring(RESOURCE_PATH_PREFIX.Length);
                fullPath = new ProjectPath(_currentGame, projectRelativePath);
            }
            else if (data.StartsWith(DEFAULT_RESOURCE_PATH_PREFIX))
            {
                var defaultRelativePath = data.Substring(DEFAULT_RESOURCE_PATH_PREFIX.Length);
                fullPath = new DefaultResourcePath(defaultRelativePath);
            }
            else
            {
                return null;
            }

            //Debug.Log($"FULL PATH: {data} => {fullPath}, TYPE: {objectType}");
            return _currentGame.ResourceLoader.GetResource(fullPath, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IGameResource).IsAssignableFrom(objectType);
        }
    }
}