using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GameEngine.Util;
using Newtonsoft.Json;

namespace GameEngine.Game.Resources
{
    /// <summary>
    /// This class is extremely useful for reference during development.
    ///
    /// It saves (serializes) + loads (de-serializes) fields from a Resource.
    ///
    /// This serializing and deserializing of SPECIFIC fields is very useful in general.
    /// </summary>
    public class ExtraResourceHelper
    {
        public static void SaveExtraData(object target, string path)
        {
            var toSave = new Dictionary<string, object>();

            foreach (var field in target.GetType().GetFields())
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

        public static void LoadExtraData(object target, Path path)
        {
            var extraPath = ToExtra(path);
            if (!File.Exists(extraPath)) return; // No loading needed.
            var text = IOHelper.ReadTextFile(extraPath);

            var toLoad = JsonConvert.DeserializeObject<Dictionary<string, object>>(text,
                new JsonSerializerSettings
                    {TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented});

            if (toLoad == null)
            {
                throw new InvalidOperationException($"Extra data loaded is null at {path}. Make sure the data here is valid!");
            }

            foreach (var name in toLoad.Keys)
            {
                var targetField = target.GetType().GetField(name);
                if (targetField == null)
                {
                    Debug.LogWarning(
                        $"[Extra Resource] Field {name} in class {target.GetType().Name} can't be found! Will skip.");
                    continue;
                }

                var value = ConvertObjectFromJson(toLoad[name], targetField.FieldType);
                if (value == null) continue;

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

        public static object ConvertObjectFromJson(object straightForwardObject, Type targetType)
        {
            var autoType = straightForwardObject.GetType();

            if (!IsType(targetType, autoType))
            {
                // Fields don't match, will have to re-parse.
                if (autoType != typeof(string) && targetType != typeof(float) && targetType != typeof(int))
                    // Enum
                    if (!IsType(autoType, typeof(long)) && IsType(targetType, typeof(Enum)))
                        Debug.LogWarning(
                            $"[Extra Resource] Weird mismatch on object {straightForwardObject}: Parsed {autoType} but we need {targetType}");

                //continue;

                // Parse string as single json thing.
                var valueJsonString = "\"" + straightForwardObject + "\"";


                object value = JsonConvert.DeserializeObject(valueJsonString, targetType);
                if (value == null)
                {
                    Debug.LogWarning(
                        $"[Extra Resource] Parse was null?? Parsed from {valueJsonString} to type {targetType}");
                }

                return value;
            }

            return straightForwardObject;
        }
    }
}