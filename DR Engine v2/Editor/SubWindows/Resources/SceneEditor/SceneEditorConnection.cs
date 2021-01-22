
using System;
using DREngine.ResourceLoading;
using Newtonsoft.Json;
using Debug = GameEngine.Debug;

namespace DREngine.Editor.SubWindows.Resources.SceneEditor
{
    public class SceneEditorConnection
    {
        private DREditor _editor;
        private DRProjectRunner _connection;

        public Action OnSaved;
        public Action<int> OnSelected;

        public Action OnStop
        {
            get => _connection.OnStop;
            set => _connection.OnStop = value;
        }

        public bool Running => _connection.Running;

        public SceneEditorConnection(DREditor editor)
        {
            _editor = editor;
            _connection = new DRProjectRunner();
            
            _connection.Connection.OnMessage += OnMessage;
        }

        private void OnMessage(string message)
        {
            try
            {
                string[] parts = message.Split(' ');
                if (parts.Length == 0) throw new InvalidOperationException("Received Message is empty!");
                string command = parts[0];
                switch (command)
                {
                    case "SAVED":
                        OnSaved.Invoke();
                        break;
                    case "SELECTED":
                    {
                        int selected = int.Parse(parts[1]);
                        OnSelected?.Invoke(selected);
                        break;
                    }
                    case "SAVE_SUCCESS":
                    case "LOG":
                    case "DEBUG":
                    case "WARNING":
                    case "ERROR":
                        // Ignore these.
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid command: \"{command}\""); 
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Invalid message sent from Scene Editor to Editor: \"{message}\": {e}");
            }
        }

        public void Open(string scene)
        {
            // For now, just cancel + reopen
            _connection.Stop();
            _connection.RunProject(_editor.ProjectData.GetFullProjectPath(), $"--sceneedit=\"{scene}\"");
        }

        public void Stop()
        {
            _connection.Stop();
        }

        public void SendNewObject(Type type)
        {
            _connection.Connection.SendMessageBlocked($"NEW {type}");
        }

        public void SendSelectObject(int objectIndex)
        {
            _connection.Connection.SendMessageBlocked($"SELECT {objectIndex}");
        }

        public void SendDeleteObject(int objectIndex)
        {
            _connection.Connection.SendMessageBlocked($"DELETE {objectIndex}");
        }

        public void SendPropertyModified(int objectIndex, string propertyName, object value)
        {
            var text = JsonConvert.SerializeObject(value,
                new JsonSerializerSettings
                    {TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented});
            _connection.Connection.SendMessageBlocked($"MODIFIED {objectIndex} {propertyName} {text}");
        }

        public void SendPropertyModifiedResource(int objectIndex, string propertyName, ProjectPath path)
        {
            _connection.Connection.SendMessageBlocked($"MODIFIED_RESOURCE {objectIndex} {propertyName} {path.GetShortName()}");
        }

        public void SendSave()
        {
            _connection.Connection.SendMessageBlocked("SAVE");
        }

        public bool WaitForSave(double timeoutSeconds)
        {
            _connection.Connection.OnMessage += OnMessage;
            bool saved = false;
            void OnMessage(string obj)
            {
                if (obj == "SAVE_SUCCESS")
                {
                    saved = true;
                }
            }

            var start = DateTime.Now;

            while (!saved)
            {
                // Fail
                if ((DateTime.Now - start).TotalSeconds > timeoutSeconds) return false;
            }

            return true;
        }

    }
}
