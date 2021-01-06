using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using Debug = GameEngine.Debug;

namespace DREngine.Editor
{
    /// <summary>
    ///     Editor side connection to the game
    /// </summary>
    public class GameConnection : DRConnection
    {
        private Process _gameProcess;

        public Action OnExit;

        public GameConnection() : base(
            new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable),
            new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
        {
            OnMessage += OnGameMessage;
        }

        private AnonymousPipeServerStream InputPipe => (AnonymousPipeServerStream) _inputPipe;
        private AnonymousPipeServerStream OutputPipe => (AnonymousPipeServerStream) _outputPipe;

        public bool Running => _gameProcess != null;

        public bool StartGameProcessAndConnect(string gameExecPath, string project)
        {
            if (Running) return false;
            Debug.LogDebug($"Running game at {gameExecPath}");

            var pinfo = new ProcessStartInfo();
            pinfo.UseShellExecute = false;
            pinfo.CreateNoWindow = false;
            pinfo.WindowStyle = ProcessWindowStyle.Normal;
            pinfo.FileName = gameExecPath;
            pinfo.Arguments =
                $"--game=\"{project}\" --readpipe=\"{OutputPipe.GetClientHandleAsString()}\" --writepipe=\"{InputPipe.GetClientHandleAsString()}\"";

            _gameProcess = Process.Start(pinfo);
            // ReSharper disable once PossibleNullReferenceException
            _gameProcess.EnableRaisingEvents = true;
            _gameProcess.Disposed += GameProcessOnExited;
            _gameProcess.Exited += GameProcessOnExited;

            Debug.LogDebug("Started game.");

            // Below is ok, we will continuously be waiting.

            // If DisposeLocalCopyOfClientHandle is not called, the anonymous pipe will not receive
            // notice when the child/client process disposes its pipe stream.
            //InputPipe.DisposeLocalCopyOfClientHandle();
            //OutputPipe.DisposeLocalCopyOfClientHandle();

            SendPing();

            Debug.LogDebug("Sent ping to game. Now waiting for responses!");

            BeginReceiving();
            return true;
        }

        private void GameProcessOnExited(object? sender, EventArgs e)
        {
            OnGameClose();
        }

        public bool CloseGame()
        {
            if (!Running) return false;

            Debug.LogDebug("Closing Game...");

            _gameProcess.Kill();
            _gameProcess.Close();
            OnGameClose();

            Debug.LogDebug("Game Closed Successfully!");
            return true;
        }

        private void OnGameClose()
        {
            Debug.LogDebug("Game Process Closed.");
            _gameProcess.Exited -= GameProcessOnExited;
            _gameProcess = null;
            OnExit?.Invoke();
        }

        private void OnGameMessage(string message)
        {
            var firstSpace = message.IndexOf(' ');
            if (firstSpace == -1)
                ParseCommand(message, "");
            else
                ParseCommand(message.Substring(0, firstSpace), message.Substring(firstSpace + 1));
        }

        private void ParseCommand(string command, string data)
        {
            switch (command)
            {
                case NetworkHelper.LOG_COMMAND:
                    Debug.Log(GetPrint(data));
                    break;
                case NetworkHelper.DEBUG_COMMAND:
                    Debug.LogDebug(GetPrint(data));
                    break;
                case NetworkHelper.WARNING_COMMAND:
                    Debug.LogWarning(GetPrint(data));
                    break;
                case NetworkHelper.ERROR_COMMAND:
                    Debug.LogError(GetPrint(data));
                    break;
                default:
                    Debug.LogWarning($"Invalid client command: {command}");
                    break;
            }
        }

        private static string GetPrint(string data)
        {
            return $"[DRGame] {data}";
        }
    }
}