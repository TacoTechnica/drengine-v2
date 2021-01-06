using System;

namespace DREngine.Editor
{
    public class DRProjectRunner
    {
        private readonly GameConnection _connection = new GameConnection();
        public Action<string> OnCrash;

        public Action OnRun;
        public Action OnStop;

        public DRProjectRunner()
        {
            _connection.OnExit += OnConnectionExit;
        }

        public bool Running => _connection.Running;

        private void OnConnectionExit()
        {
            OnStop.Invoke();
        }

        public void RunProject(string projectPath)
        {
            // Run ourselves.
            var gamePath = Environment.GetCommandLineArgs()[0];
            // If we're running a dll, use the executable instead.
            if (gamePath.EndsWith(".dll"))
                gamePath = gamePath.Substring(0, gamePath.Length - ".dll".Length); // + ".exe";
            if (_connection.StartGameProcessAndConnect(gamePath, projectPath)) OnRun?.Invoke();
        }


        public void Stop()
        {
            try
            {
                _connection.CloseGame();
            }
            catch (InvalidOperationException e)
            {
                // Failed to close for some reason.
                OnCrash?.Invoke(e.Message);
            }
        }
    }
}