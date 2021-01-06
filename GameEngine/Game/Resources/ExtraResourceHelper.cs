using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GameEngine.Util;
using Newtonsoft.Json;

namespace GameEngine.Game.Resources
{
    public class ExtraResourceHelper
    {
        public static void SaveExtraData<T>(T target, string path)
        {
            var toSave = new Dictionary<string, object>();

            foreach (var field in typeof(T).GetFields())
            {
                var fieldIsExtra = field.GetCustomAttribute<ExtraDataAttribute>() != null;
                if (fieldIsExtra) toSave[field.Name] = field.GetValue(target);
            }

            if (toSave.Count != 0)
            {
                var text = JsonConvert.SerializeObject(toSave,
                    new JsonSerializerSettings
                        {TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented});
                IOHelper.WriteTextFile(ToExtra(path), text);
            }
        }

        public static void LoadExtraData<T>(T target, Path path)
        {
            var extraPath = ToExtra(path);
            if (!File.Exists(extraPath)) return; // No loading needed.
            var text = IOHelper.ReadTextFile(extraPath);

            var toLoad = JsonConvert.DeserializeObject<Dictionary<string, object>>(text,
                new JsonSerializerSettings
                    {TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented});

            foreach (var name in toLoad.Keys)
            {
                var targetField = typeof(T).GetField(name);
                if (targetField == null)
                {
                    Debug.LogWarning(
                        $"[Extra Resource] Field {name} in class {typeof(T).Name} can't be found! Will skip.");
                    continue;
                }

                var value = toLoad[name];
                var autoType = value.GetType();
                var realType = targetField.FieldType;

                if (!IsType(realType, autoType))
                {
                    // Fields don't match, will have to re-parse.
                    if (autoType != typeof(string) && realType != typeof(float) && realType != typeof(int))
                        // Enum
                        if (!IsType(autoType, typeof(long)) && IsType(realType, typeof(Enum)))
                            Debug.LogWarning(
                                $"[Extra Resource] Weird mismatch on field {name}: Parsed {autoType} (<< not string!!) but we need {realType}");

                    //continue;

                    // Parse string as single json thing.
                    var valueJsonString = "\"" + value + "\"";


                    value = JsonConvert.DeserializeObject(valueJsonString, realType);
                    if (value == null)
                    {
                        Debug.LogWarning(
                            $"[Extra Resource] Parse was null?? Parsed from {valueJsonString} to type {realType}");
                        continue;
                    }
                }

                targetField.SetValue(target, value);
            }
        }

        private static Path ToExtra(Path path)
        {
            return path + ".extra";
        }

        private static bool IsType(Type typeToCheck, Type type)
        {
            // type is the parent here
            return type.IsAssignableFrom(typeToCheck);
        }
    }
}