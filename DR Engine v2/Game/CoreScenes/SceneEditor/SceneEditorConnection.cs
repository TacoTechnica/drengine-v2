using System;
using System.Collections.Generic;
using GameEngine;
using Newtonsoft.Json;

namespace DREngine.Game.CoreScenes.SceneEditor
{
    public class SceneEditorConnection
    {
        private EditorConnection _connection;

        public Action<Type> OnNewObject;
        public Action<int> OnDeleteObject;
        public Action<int> OnSelectObject;
        public Action<int, string, object> OnModifiedObject;

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

        // TODO: Transform Changed

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
                        OnNewObject.Invoke(type);
                        break;
                    case "DELETE":
                    {
                        int index = int.Parse(parts[1]);
                        OnDeleteObject.Invoke(index);
                        break;
                    }
                    case "SELECT":
                    {
                        int index = int.Parse(parts[1]);
                        OnSelectObject.Invoke(index);
                        break;
                    }
                    case "MODIFIED":
                    {
                        int index = int.Parse(parts[1]);
                        string fieldName = parts[2];
                        object newValue = JsonConvert.DeserializeObject<object>(parts[3],
                            new JsonSerializerSettings
                                {TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented});
                        OnModifiedObject.Invoke(index, fieldName, newValue);
                        break;
                    }
                    case "SAVE":
                        OnSaveRequested.Invoke();
                        // We assume that whatever was invoked succeeded.
                        _connection.SendMessageBlocked("SAVE_SUCCESS");
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid Command: \"{command}\"");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"RECEIVED INVALID MESSAGE: \"{message}\". Error: {e.ToString()}");
            }
        }
    }
}