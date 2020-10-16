
using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DREngine
{
    public class ProjectData
    {
        #region Data

        /// <summary>
        /// Actual Serialized Project Data
        /// </summary>
        public string Name;
        public Dictionary<string, string> Rooms = new Dictionary<string, string>();
        public Dictionary<string, string> Characters = new Dictionary<string, string>();
        public Dictionary<string, string> Sprites = new Dictionary<string, string>();
        public Dictionary<string, string> Sfx = new Dictionary<string, string>();
        public Dictionary<string, string> Va = new Dictionary<string, string>();
        public Dictionary<string, string> Bgm = new Dictionary<string, string>();
        public Dictionary<string, string> Dialogue = new Dictionary<string, string>();

        /// <summary>
        /// Directories we will read to/create when modifying our project.
        /// </summary>
        public const string RoomDir = "rooms";
        public const string CharactersDir = "characters";
        public const string SpritesDir = "sprites";
        public const string SfxDir = "sfx";
        public const string VaDir = "va";
        public const string BgmDir = "bgm";
        public const string DialogueDir = "dialogue";

        #endregion

        private const string CommentHeader = "This file is auto generated. IT WILL BE OVERWRITTEN! Expect all changes to be lost.";

        public static void ReadFromFile(string fpath, out ProjectData data)
        {
            try
            {
                IDeserializer deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .IgnoreUnmatchedProperties()
                    .Build();
                // We might be given a relative directory, so try that first.
                if (!File.Exists(fpath) && !Directory.Exists(fpath))
                {
                    fpath = Program.RootDirectory + "/" + fpath;
                    Debug.Log($"RELATIVE PATH: {fpath}");
                }
                // We might be given the project directory, so try to find the project file within it.
                if (Directory.Exists(fpath))
                {
                    fpath += "/project.yaml";
                }
                Debug.Log($"OPENING: {fpath}");
                using StreamReader reader = File.OpenText(fpath);
                Debug.Log($"OPENED!");
                data = (ProjectData) deserializer.Deserialize(reader, typeof(ProjectData));
            }
            catch (Exception e)
            {
                data = null;
                throw e;
            }
        }

        public static void SaveToFile(string fpath, ProjectData data)
        {
            var serializer = new SerializerBuilder().Build();
            string yaml = serializer.Serialize(data);
            using StreamWriter writer = File.CreateText(fpath);
            writer.WriteLine($"\n#{CommentHeader}\n"); // Add comment to inform user that editing this is a bad idea
            serializer.Serialize(writer, data);
        }
    }
}
