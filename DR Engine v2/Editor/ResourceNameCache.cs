using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DREngine.Game.Scene;
using GameEngine.Game.Resources;

namespace DREngine.Editor
{
    public class ResourceNameCache
    {
        private readonly Dictionary<Type, List<string>> _resourcePathsByType = new Dictionary<Type, List<string>>();

        public void Clear()
        {
            _resourcePathsByType.Clear();
        }

        public void AddToCache(string path)
        {
            var extension = new FileInfo(path).Extension;
            if (extension.StartsWith(".")) extension = extension.Substring(1);

            var type = GetType(path, extension);
            if (type == null) return;

            if (!_resourcePathsByType.ContainsKey(type)) _resourcePathsByType.Add(type, new List<string>());
            _resourcePathsByType[type].Add(path);
        }

        public IEnumerable<string> GetPathsOfType(Type type)
        {
            if (!_resourcePathsByType.ContainsKey(type)) return new List<string>();
            return _resourcePathsByType[type];
        }

        public IEnumerable<string> GetPathsMatchingSearch<T>(string search)
        {
            var paths = GetPathsOfType(typeof(T));
            return paths.Where(path => PathMatchesSearch(path, search));
        }

        public IEnumerable<string> GetPathsMatchingSearch(Type t, string search)
        {
            var paths = GetPathsOfType(t);
            return paths.Where(path => PathMatchesSearch(path, search));
        }

        private static bool PathMatchesSearch(string path, string search)
        {
            // You can add more search features here!
            return path.ToLower().Contains(search.ToLower());
        }

        private static Type GetType(string path, string extension)
        {
            switch (extension)
            {
                case "png":
                    return typeof(Sprite);
                case "wav":
                    return typeof(AudioClip);
                case "scene":
                    return typeof(DRScene);
                case "ttf":
                    return typeof(Font);
                default:
                    return null;
            }
        }
    }
}