using System;
using System.IO.Pipes;
using GameEngine;

namespace DREngine.Game
{
    /// <summary>
    ///     Game side connection to the editor
    /// </summary>
    public class EditorConnection : DRConnection
    {
        public readonly bool Active;

        public EditorConnection(bool active, string editorPipeReadHandle, string editorPipeWriteHandle) : base(
            active ? new AnonymousPipeClientStream(PipeDirection.In, editorPipeReadHandle) : null,
            active ? new AnonymousPipeClientStream(PipeDirection.Out, editorPipeWriteHandle) : null)
        {
            Active = active;
            if (active)
            {
                BeginReceiving();

                // Debug hookups (send a command for every log)
                Debug.OnLogDebug += message => { SendCommand(NetworkHelper.DEBUG_COMMAND, message); };
                Debug.OnLogPrint += message => { SendCommand(NetworkHelper.LOG_COMMAND, message); };
                Debug.OnLogWarning += message => { SendCommand(NetworkHelper.WARNING_COMMAND, message); };
                Debug.OnLogError += (message, stacktrace) =>
                {
                    SendCommand(NetworkHelper.ERROR_COMMAND, message + " : " + stacktrace);
                };
            }
        }

        public void WaitOnEditorPingAsync(Action onComplete)
        {
            if (!Active) return;

            WaitForPing(onComplete);
        }

        private void SendCommand(string name, string data)
        {
            SendMessageBlocked($"{name} {data}");
        }
    }
}