using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using DREngine.Game;
using GameEngine;
using GameEngine.Game;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using Path = GameEngine.Game.Path;

namespace DREngine
{

    class DefaultResourcePath : Path
    {

        public const string DEFAULT_RESOURCE_FOLDER = "default_resources";

        public DefaultResourcePath(string path) : base(path) { }
        public override string ToString()
        {
            return $"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)}/{DEFAULT_RESOURCE_FOLDER}/{RelativePath}";
        }
    }

    public class OverridablePath
    {
        public string DefaultResourcePath { get; private set; }
        public string OverrideProjectPath = null;
        public bool Overrided => OverrideProjectPath != null;

        public OverridablePath(string defaultResourcePath)
        {
            DefaultResourcePath = defaultResourcePath;
        }

        public string GetFullPath(DRGame game)
        {
            if (Overrided)
            {
                return new ProjectPath(game, OverrideProjectPath);
            }
            return new DefaultResourcePath(DefaultResourcePath);
        }
    }
}
