using System;
using System.Collections.Generic;
using System.Text;
using DREngine.ResourceLoading;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Objects;
using GameEngine.Game.Resources;
using Newtonsoft.Json;

namespace DREngine.Game.CoreScenes.SceneEditor
{
    public class SceneEditorConnection
    {
        private EditorConnection _connection;

        public Action<Type> OnNewObject;
        public Action<int> OnDeleteObject;
        public Action<int> OnSelectObject;
        public Action<int, string, string> OnModifiedObject;
        public Action<int, string, Path> OnResourceModifiedObject;

        public Action OnSaveRequested;

        public SceneEditorConnection(EditorConnection connection)
        {
            _connection = connection;

            _connection.OnMessage += OnEditorMessage;
        }

        public void SendSaved()
        {
            _connection.SendMessageBlocked("SAVED");
        }

        public void SendSelected(int index)
        {
            _connection.SendMessageBlocked($"SELECTED {index}");
        }

        public void SendTransformChanged(int index, Transform3D transform)
        {
            var text = JsonConvert.SerializeObject(transform,
                new JsonSerializerSettings
                    {TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented});
            _connection.SendMessageBlocked($"TRANSFORM {index} {text}");
        }

        private void OnEditorMessage(string message)
        {
            try
            {
                string[] parts = message.Split(" ");
                if (parts.Length == 0) Debug.LogWarning("Empty message received...");
                string command = parts[0];
                switch (command)
                {
                    case "NEW":
                        Type type = JsonConvert.DeserializeObject<Type>("\"" + parts[1] + ", DR Engine\"",
                            new JsonSerializerSettings
                                {TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented});
                        OnNewObject?.Invoke(type);
                        break;
                    case "DELETE":
                    {
                        int index = int.Parse(parts[1]);
                        OnDeleteObject?.Invoke(index);
                        break;
                    }
                    case "SELECT":
                    {
                        int index = int.Parse(parts[1]);
                        OnSelectObject?.Invoke(index);
                        break;
                    }
                    case "MODIFIED":
                    {
                        int index = int.Parse(parts[1]);
                        string fieldName = parts[2];

                        string objectData = JoinRemainder(parts, 3);

                        OnModifiedObject?.Invoke(index, fieldName, objectData);
                        break;
                    }
                    case "SAVE":
                        OnSaveRequested.Invoke();
                        // We assume that whatever was invoked succeeded.
                        _connection.SendMessageBlocked("SAVE_SUCCESS");
                        break;
                    case "MODIFIED_RESOURCE":
                    {
                        int index = int.Parse(parts[1]);
                        string fieldName = parts[2];
                        string shortPath = JoinRemainder(parts, 3);;

                        Path resultPath = ProjectResourceConverter.ShortNameToPath(shortPath);

                        OnResourceModifiedObject?.Invoke(index, fieldName, resultPath);
                        break;
                    }
                    default:
                        throw new InvalidOperationException($"Invalid Command: \"{command}\"");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"RECEIVED INVALID MESSAGE: \"{message}\". Error: {e.ToString()}");
            }
        }

        public static string JoinRemainder(string[] parts, int startIndex)
        {
            StringBuilder result = new StringBuilder();
            for (int i = startIndex; i < parts.Length; ++i)
            {
                result.Append(parts[i]);
                if (i != parts.Length - 1)
                {
                    result.Append(" ");
                }
            }

            return result.ToString();
        }
    }
}