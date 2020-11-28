
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GameEngine;
using GameEngine.Game.Debugging;
using Microsoft.Xna.Framework.Graphics;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Path = GameEngine.Game.Path;

namespace DREngine
{
    public class ProjectData
    {
        #region Data

        public string Name;
        public string Author;

        //public ProjectResourceList Resources = new ProjectResourceList();

        public class OverrideableResourceList
        {
            public OverridablePath MenuFont = new OverridablePath("Fonts/SourceSansPro/SourceSansPro-Regular.ttf");
            public OverridablePath DialogueFont = new OverridablePath("Fonts/SourceSansPro/SourceSansPro-Regular.ttf");
            public OverridablePath TitleFont = new OverridablePath("Fonts/SourceSansPro/SourceSansPro-Bold.ttf");
        }
        public OverrideableResourceList OverridableResources = new OverrideableResourceList();

        /// <summary>
        /// Default directories of our resources.
        /// THESE ARE NOT ENFORCED! They only serve as suggestions.
        /// </summary>
        public const string ROOM_DEFAULT_DIR = "rooms";
        public const string CHARACTERS_DEFAULT_DIR = "characters";
        public const string SPRITES_DEFAULT_DIR = "sprites";
        public const string SFX_DEFAULT_DIR = "sfx";
        public const string VA_DEFAULT_DIR = "va";
        public const string BGM_DEFAULT_DIR = "bgm";
        public const string DIALOGUE_DEFAULT_DIR = "dialogue";

        #endregion

        private const string CommentHeader = "This file is auto generated. IT WILL BE OVERWRITTEN! Expect all changes to be lost.";

        private Path _fullProjectPath;

        /// <summary>
        /// Loads a project from a file.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="fpath"></param>
        /// <param name="fullLoad"> If false, it will NOT do any extra project loading. Use this for quick surface level project parsing. </param>
        /// <returns></returns>
        /// <exception cref="YamlException"></exception>
        public static ProjectData ReadFromFile(GraphicsDevice g, Path fpath, bool fullLoad = true)
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
                    fpath = System.IO.Path.Combine(Environment.CurrentDirectory, fpath);
                    //Debug.Log($"RELATIVE PATH: {fpath}");
                }
                // We might be given the project directory, so try to find the project file within it.
                if (Directory.Exists(fpath))
                {
                    fpath += "/project.yaml";
                }
                //Debug.Log($"OPENING: {fpath}");
                string text = IO.ReadTextFile(fpath);
                //Debug.Log($"OUTPUT: {text}");
                ProjectData result = deserializer.Deserialize<ProjectData>(text);
                result._fullProjectPath = fpath;
                if (fullLoad)
                {
                    result.LoadDefaults(g);
                }

                return result;
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
            //CallOnDeserializeOn(g, OverridableResources);
        }

        public string GetFullProjectPath(string relative = "")
        {
            string dir = System.IO.Path.GetDirectoryName(_fullProjectPath);
            return System.IO.Path.Join(dir, relative);
        }

        public string GetRelativeProjectPath(string fullPath)
        {
            string dir = GetFullProjectPath();
            string result = System.IO.Path.GetRelativePath(dir, fullPath);
            //Debug.Log($"{fullPath} from {_fullProjectPath} => {result}");
            if (result.StartsWith(".."))
            {
                throw new InvalidArgumentsException($"Full path {fullPath} does not reside within project path: {dir}.");
            }

            return result;
        }

        public string GetFullDefaultResourcePath(string relative = "")
        {
            string rootDir = System.IO.Path.GetDirectoryName(_fullProjectPath);
            return System.IO.Path.Join(rootDir, DefaultResourcePath.DEFAULT_RESOURCE_FOLDER, relative);
        }

        public string GetRelativeDefaultResourcePath(string fullPath)
        {
            string dir = GetFullDefaultResourcePath();
            string result = System.IO.Path.GetRelativePath(dir, fullPath);
            //Debug.Log($"{fullPath} from {_fullProjectPath} => {result}");
            if (result.StartsWith(".."))
            {
                throw new InvalidArgumentsException($"Full path {fullPath} does not reside within default resource path: {dir}.");
            }

            return result;
        }

        public bool IsDefaultResourcePath(string fullPath)
        {
            return fullPath.StartsWith(GetFullDefaultResourcePath());
        }

        /*
        /// <summary>
        ///     Goes through every object that has a OnDeserialize method (from interface) and calls that method.
        /// </summary>
        private void CallOnDeserializeOn(GraphicsDevice g, object parent)
        {
            Debug.Log("ProjectData On Deserialize");
            foreach (var field in parent.GetType().GetFields())
            {
                object b = field.GetValue(parent);
                if (b is IOnDeserialized obj)
                {
                    obj.OnDeserialized(g);
                }
            }
        }
        */
    }
}
