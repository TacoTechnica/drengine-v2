using System;
using System.Threading;
using System.Threading.Tasks;
using DREngine.Game;
using GameEngine;
using Gtk;
using Microsoft.Xna.Framework;
using Action = System.Action;

namespace DREngine.Editor
{
    public class DRProjectRunner
    {

        private readonly GameConnection _connection = new GameConnection();

        public Action OnRun;
        public Action OnStop;
        public Action<string> OnCrash;

        public bool Running => _connection.Running;

        public DRProjectRunner()
        {
            _connection.OnExit += OnConnectionExit;
        }

        private void OnConnectionExit()
        {
            OnStop.Invoke();
        }

        public void RunProject(string projectPath)
        {
            // Run ourselves.
            string gamePath = Environment.GetCommandLineArgs()[0];
            // If we're running a dll, use the executable instead.
            if (gamePath.EndsWith(".dll"))
            {
                gamePath = gamePath.Substring(0, gamePath.Length - ".dll".Length);// + ".exe";
            }
            if (_connection.StartGameProcessAndConnect(gamePath, projectPath))
            {
                OnRun?.Invoke();
            }
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
