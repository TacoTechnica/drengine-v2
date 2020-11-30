using System;
using System.Threading;
using System.Threading.Tasks;
using DREngine.Game;
using GameEngine;
using Microsoft.Xna.Framework;

namespace DREngine.Editor
{
    public class DRProjectRunner
    {

        private DRGame CurrentGame = null;

        public Action OnRun;
        public Action OnStop;
        public Action<Exception> OnCrash;

        private Task _runner;

        private bool _closeFlag = false;

        public bool Running => (CurrentGame != null); //  && (_runner == null || _runner.IsCompleted)

        public void RunProject(string projectPath)
        {
            if (!Running)
            {
                CurrentGame = new DRGame(projectPath);

                CurrentGame.OnPostUpdate += OnPostUpdate;

                OnRun?.Invoke();
                try
                {
                    CurrentGame.Run();
                }
                catch (Exception e)
                {
                    //CurrentGame = null;
                    OnCrash?.Invoke(e);
                    Debug.LogError(e.ToString());
                }
                CurrentGame.OnPostUpdate -= OnPostUpdate;
                CurrentGame.Dispose();
                CurrentGame = null;
                OnStop?.Invoke();

                _closeFlag = false;

                /*
                _runner = Task.Run(() =>
                {
                    try
                    {
                        CurrentGame.Run();
                    }
                    catch (Exception e)
                    {
                        //CurrentGame = null;
                        Debug.LogError(e.ToString());
                    }
                });
                */
                /*
                OnRun.Invoke();
                using (CurrentGame = new DRGame(projectPath))
                {
                    CurrentGame.Run();
                }
                CurrentGame.Dispose();
                CurrentGame = null;
                OnStop.Invoke();
                */
            }

        }

        private void OnPostUpdate()
        {
            if (!_closeFlag)
            {
                Gtk.Application.RunIteration(false);
            }
        }

        public void Stop()
        {
            _closeFlag = true;
            CurrentGame?.Exit();
        }
    }
}
