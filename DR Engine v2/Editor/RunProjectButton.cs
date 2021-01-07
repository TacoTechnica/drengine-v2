using Gdk;
using Gtk;

namespace DREngine.Editor
{
    public class RunProjectButton : Button
    {
        private DREditor _editor;

        public RunProjectButton(DREditor editor)
        {
            _editor = editor;
            SetIcon("Run Project", editor.Window.Icons.Play);
            editor.ProjectRunner.OnRun += OnGameRun;
            editor.ProjectRunner.OnStop += OnGameStop;
        }

        private void OnGameStop()
        {
            SetIcon("Run Project", _editor.Window.Icons.Play);
        }

        private void OnGameRun()
        {
            SetIcon("Stop Project", _editor.Window.Icons.Stop);
        }

        protected override void OnClicked()
        {
            base.OnClicked();
            if (!_editor.ProjectLoaded)
            {
                _editor.Window.AlertProblem("Project not loaded.");
                return;
            }

            if (_editor.ProjectRunner.Running)
                _editor.ProjectRunner.Stop();
            else
                _editor.RunCurrentProject();
        }

        private void SetIcon(string name, Pixbuf icon)
        {
            TooltipText = name;
            if (icon == null)
                Label = name;
            else
                Image = new Image(icon);
        }
    }
}