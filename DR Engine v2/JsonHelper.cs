﻿using DREngine.Game;
using GameEngine;
using GameEngine.Game;
using GameEngine.Util;
using Newtonsoft.Json;

namespace DREngine
{
    public class JsonHelper
    {
        public static void SaveToJson<T>(T obj, string filepath)
        {
            IO.WriteTextFile(filepath, JsonConvert.SerializeObject(obj, new JsonSerializerSettings(){TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented}));
        }
        public static T LoadFromJson<T>(DRGame game, string filepath)
        {
            string text = IO.ReadTextFile(filepath);
            IDependentOnDRGame.CurrentGame = game;
            return JsonConvert.DeserializeObject<T>(text, new JsonSerializerSettings(){TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented});
        }

        /*
        public static void SaveToProjectJson<T>(DRGame game, T obj, Path filepath)
        {
            SaveToJson(obj, game.GameProjectData.GetFullProjectPath(filepath));
        }

        public static T LoadFromProjectJson<T>(DRGame game, Path filepath)
        {
            return LoadFromJson<T>(game, game.GameProjectData.GetFullProjectPath(filepath));
        }
        */

    }
}
