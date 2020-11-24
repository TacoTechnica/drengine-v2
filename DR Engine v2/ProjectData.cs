
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GameEngine;
using Microsoft.Xna.Framework.Graphics;
using YamlDotNet.Core;
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

        public class ProjectResourceList
        {
            public Dictionary<string, string> Rooms = new Dictionary<string, string>();
            public Dictionary<string, string> Characters = new Dictionary<string, string>();
            public Dictionary<string, string> Sprites = new Dictionary<string, string>();
            public Dictionary<string, string> Sfx = new Dictionary<string, string>();
            public Dictionary<string, string> Va = new Dictionary<string, string>();
            public Dictionary<string, string> Bgm = new Dictionary<string, string>();
            public Dictionary<string, string> Dialogue = new Dictionary<string, string>();
        }

        //public ProjectResourceList Resources = new ProjectResourceList();

        public class OverrideableResourceList
        {
            public OverridableSpriteFont MenuFont = new OverridableSpriteFont("Fonts/SourceSansPro/SourceSansPro-Regular.ttf", 16);
            public OverridableSpriteFont DialogueFont = new OverridableSpriteFont("Fonts/SourceSansPro/SourceSansPro-Regular.ttf", 16);
            public OverridableSpriteFont TitleFont = new OverridableSpriteFont("Fonts/SourceSansPro/SourceSansPro-Bold.ttf", 24);
        }
        public OverrideableResourceList OverridableResources = new OverrideableResourceList();

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

        // TODO: Use GamePath instead of a string. That will eliminate the need to pre-process it.
        public static ProjectData ReadFromFile(GraphicsDevice g, string fpath)
        {
            try
            {
                IDeserializer deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .IgnoreUnmatchedProperties()
                    .Build();
                // We might be given a relative directory, so try that first.
                if (!File.Exists(fpath) && !Directory.Exists(fpath))
                {
                    fpath = Path.Combine(Environment.CurrentDirectory, fpath);
                    Debug.Log($"RELATIVE PATH: {fpath}");
                }
                // We might be given the project directory, so try to find the project file within it.
                if (Directory.Exists(fpath))
                {
                    fpath += "/project.yaml";
                }
                Debug.Log($"OPENING: {fpath}");
                string text = IO.ReadTextFile(fpath);
                //Debug.Log($"OUTPUT: {text}");
                ProjectData result = deserializer.Deserialize<ProjectData>(text);
                result.LoadDefaults(g);
                return result;
                /*
                using StreamReader reader = File.OpenText(fpath);
                Debug.Log($"OPENED!");
                return deserializer.Deserialize<ProjectData>(reader);
                */
            }
            catch (YamlException e)
            {
                throw e;
            }
        }

        public static void WriteToFile(string fpath, ProjectData data)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();
            using StreamWriter writer = File.CreateText(fpath);
            writer.WriteLine($"\n#{CommentHeader}\n"); // Add comment to inform user that editing this is a bad idea
            serializer.Serialize(writer, data);
        }

        public void LoadDefaults(GraphicsDevice g)
        {
            CallOnDeserializeOn(g, OverridableResources);
        }

        /// <summary>
        ///     Goes through every object that has a OnDeserialize method (from interface) and calls that method.
        /// </summary>
        private void CallOnDeserializeOn(GraphicsDevice g, object parent)
        {
            Debug.Log("OKI DOKI");
            foreach (var field in parent.GetType().GetFields())
            {
                object b = field.GetValue(parent);
                if (b is IOnDeserialized obj)
                {
                    obj.OnDeserialized(g);
                }
            }
        }
    }
}
