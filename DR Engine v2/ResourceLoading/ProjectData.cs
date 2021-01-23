using System;
using System.IO;
using DREngine.Editor.SubWindows.FieldWidgets;
using DREngine.Game.Resources;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Debugging;
using GameEngine.Game.Resources;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Path = GameEngine.Game.Path;

namespace DREngine.ResourceLoading
{
    /// <summary>
    ///     Represents a project data file. (project.json)
    /// </summary>
    public class ProjectData
    {
        private const string PROJECT_DATA_FILE_NAME = "project.json";

        private const string COMMENT_HEADER =
            "This file is auto generated. IT WILL BE OVERWRITTEN! Expect all changes to be lost.";

        private string _fullProjectPath;

        /// <summary>
        ///     Loads a project from a file.
        /// </summary>
        /// <param name="fpath"></param>
        /// <param name="fullLoad">
        ///     If false, it will NOT do any extra project loading. Use this for quick surface level project
        ///     parsing.
        /// </param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        public static ProjectData LoadFromFile(Path fpath, bool fullLoad = true)
        {
            // We might be given a relative directory, so try that first.
            if (!File.Exists(fpath) && !Directory.Exists(fpath))
                fpath = System.IO.Path.Combine(Environment.CurrentDirectory, fpath);
            //Debug.Log($"RELATIVE PATH: {fpath}");
            // We might be given the project directory, so try to find the project file within it.
            if (Directory.Exists(fpath)) fpath += "/" + PROJECT_DATA_FILE_NAME;
            // throws JsonException
            var result = JsonHelper.LoadFromJson<ProjectData>(null, fpath);
            result._fullProjectPath = fpath;

            return result;
        }

        public static void WriteToFile(string fpath, ProjectData data)
        {
            JsonHelper.SaveToJson(data, fpath);
            //using StreamWriter writer = File.CreateText(fpath);
            //writer.WriteLine($"\n#{CommentHeader}\n"); // Add comment to inform user that editing this is a bad idea
        }

        public string GetFullProjectPath(Path path = null)
        {
            return GetFullProjectPath(_fullProjectPath, path);
        }

        public static string GetFullProjectPath(string fullProjectPath, Path path = null)
        {
            var dir = System.IO.Path.GetDirectoryName(fullProjectPath);
            //Debug.Log($"TEMP: {fullProjectPath} : {dir} -> {path}");
            string relativePath;
            if (path is ProjectPath pp)
            {
                relativePath = pp.RelativePath;
            }
            else if (path is EnginePath ep)
            {
                relativePath = ep.RelativePath;
                throw new NotImplementedException();
            }
            else
            {
                relativePath = path;
            }

            return System.IO.Path.Join(dir, relativePath);
        }

        public string GetRelativeProjectPath(string fullPath)
        {
            var dir = GetFullProjectPath();
            var result = System.IO.Path.GetRelativePath(dir, fullPath);
            //Debug.Log($"{fullPath} from {_fullProjectPath} => {result}");
            if (result.StartsWith(".."))
                throw new InvalidArgumentsException(
                    $"Full path {fullPath} does not reside within project path: {dir}.");

            return result;
        }

        public string GetFullDefaultResourcePath(string relative = "")
        {
            return new DefaultResourcePath(relative);
        }

        public string GetRelativeDefaultResourcePath(string fullPath)
        {
            var dir = GetFullDefaultResourcePath();
            var result = System.IO.Path.GetRelativePath(dir, fullPath);
            //Debug.Log($"{fullPath} from {_fullProjectPath} => {result}");
            if (result.StartsWith(".."))
                throw new InvalidArgumentsException(
                    $"Full path {fullPath} does not reside within default resource path: {dir}.");

            return result;
        }

        public bool IsDefaultResourcePath(string fullPath)
        {
            return fullPath.StartsWith(GetFullDefaultResourcePath());
        }

        #region Data

        public string Name;
        public string Author;

        //public ProjectResourceList Resources = new ProjectResourceList();

        public class OverrideableResourceList
        {
            [ResourceType(typeof(Font))]
            public OverridablePath DialogFont = new OverridablePath("Fonts/SourceSansPro/SourceSansPro-Regular.ttf");

            [ResourceType(typeof(Font))]
            public OverridablePath MenuFont = new OverridablePath("Fonts/SourceSansPro/SourceSansPro-Regular.ttf");

            [ResourceType(typeof(Font))]
            public OverridablePath TitleFont = new OverridablePath("Fonts/SourceSansPro/SourceSansPro-Bold.ttf");

            [ResourceType(typeof(DRSprite))]
            public OverridablePath DialogueBackground = new OverridablePath("Sprites/UI/DialogBox.png");

            [ResourceType(typeof(DRSprite))]
            public OverridablePath DialogueNameplate = new OverridablePath("Sprites/UI/DialogBoxNameplate.png");
            
            [ResourceType(typeof(DRSprite))]
            public OverridablePath DefaultSurfaceTexture = new OverridablePath("Sprites/Texture/DefaultSurface.png");
        }

        public class SettingsList
        {
            public Color DialogueTextColor = Color.White;
        }

        [FieldContainer] public OverrideableResourceList OverridableResources = new OverrideableResourceList();
        [FieldContainer] public SettingsList Settings = new SettingsList();
        /// <summary>
        ///     Default directories of our resources.
        ///     THESE ARE NOT ENFORCED! They only serve as suggestions.
        /// </summary>
        public const string ROOM_DEFAULT_DIR = "rooms";

        public const string CHARACTERS_DEFAULT_DIR = "characters";
        public const string SPRITES_DEFAULT_DIR = "sprites";
        public const string SFX_DEFAULT_DIR = "sfx";
        public const string VA_DEFAULT_DIR = "va";
        public const string BGM_DEFAULT_DIR = "bgm";
        public const string DIALOGUE_DEFAULT_DIR = "dialogue";

        #endregion

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


    [AttributeUsage(AttributeTargets.Field)]
    internal class ResourceTypeAttribute : Attribute
    {
        public Type Type;

        public ResourceTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}