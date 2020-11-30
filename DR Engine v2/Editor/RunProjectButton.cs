using GameEngine;
using Gdk;
using Gtk;

namespace DREngine.Editor
{
    public class RunProjectButton : Button
    {
        public RunProjectButton()
        {
            SetIcon("Run Project", DREditor.Instance.Window.Icons.Play);
            DREditor.Instance.ProjectRunner.OnRun += OnGameRun;
            DREditor.Instance.ProjectRunner.OnStop += OnGameStop;
        }

        private void OnGameStop()
        {
            SetIcon("Run Project", DREditor.Instance.Window.Icons.Play);
        }

        private void OnGameRun()
        {
            SetIcon("Stop Project", DREditor.Instance.Window.Icons.Stop);
        }

        protected override void OnClicked()
        {
            base.OnClicked();
            if (DREditor.Instance.ProjectRunner.Running)
            {
                DREditor.Instance.ProjectRunner.Stop();
            }
            else
            {
                DREditor.Instance.RunCurrentProject();
            }
        }

        private void SetIcon(string name, Pixbuf icon)
        {
            TooltipText = name;
            if (icon == null)
            {
                Label = name;
            } else {
                Image = new Image(icon);
            }
        }
    }
}
