using System;

namespace DREngine.Editor
{
    public class DRProjectRunner
    {
        public readonly GameConnection Connection = new GameConnection();
        public Action<string> OnCrash;

        public Action OnRun;
        public Action OnStop;

        public DRProjectRunner()
        {
            Connection.OnExit += OnConnectionExit;
        }

        public bool Running => Connection.Running;

        private void OnConnectionExit()
        {
            OnStop?.Invoke();
        }

        public void RunProject(string projectPath, string extraArgs = "")
        {
            // Run ourselves.
            var gamePath = Environment.GetCommandLineArgs()[0];
            // If we're running a dll, use the executable instead.
            if (gamePath.EndsWith(".dll"))
                gamePath = gamePath.Substring(0, gamePath.Length - ".dll".Length); // + ".exe";
            if (Connection.StartGameProcessAndConnect(gamePath, projectPath, extraArgs)) OnRun?.Invoke();
        }


        public void Stop()
        {
            try
            {
                Connection.CloseGame();
            }
            catch (InvalidOperationException e)
            {
                // Failed to close for some reason.
                OnCrash?.Invoke(e.Message);
            }
        }
    }
}