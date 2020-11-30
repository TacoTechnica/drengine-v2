using System;
using GameEngine;
using GameEngine.Game;
using Gtk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Editor
{
    public class DREditor : IDisposable
    {
        private bool _disposed = false;
        public DREditorMainWindow Window { get; private set; } = null;

        // TODO: Make this set to nothing or a custom/licensed theme.
        private const string STARTING_THEME = "themes/Material-Black-Lime/gtk-3.0/gtk.css";

        public ProjectData ProjectData;
        private string _projectPath;

        public DRProjectRunner ProjectRunner;

        // Poo poo singleton
        public static DREditor Instance = null;

        public DREditor()
        {
            // Poo poo singleton
            if (Instance != null) Debug.LogError("Editor already initialized! Will see problems.");
            Instance = this;

            ProjectRunner = new DRProjectRunner();
        }
        ~DREditor() {
            Dispose(false);
        }

        public void Run()
        {
            Debug.LogDebug("Editor Run()");
            GLib.ExceptionManager.UnhandledException += OnHandleExceptionEvent;
            // Init app
            Application.Init();

            Window = new DREditorMainWindow();
            Window.MakeWindow("DR Editor", 640, 480);
            Window.AddEvents((int) (Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask));
            Window.Show();
            Window.DeleteEvent += WindowOnDeleteEvent;


            if (!string.IsNullOrEmpty(STARTING_THEME))
            {
                Window.SetTheme(STARTING_THEME);
            }
            Initialize();
            Application.Run();
        }

        public void RunCurrentProject()
        {
            if (ProjectData == null)
            {
                Window.AlertProblem("No project is loaded, will not run anything.");
            }
            else
            {
                ProjectRunner.RunProject(_projectPath);
            }
        }

        public void EmptyProject()
        {
            Debug.LogDebug("Loading empty project");
            ProjectData = null;
            _projectPath = "";
            Window.EmptyProject();
        }

        public void LoadProject(string fullPath)
        {
            ProjectData = ProjectData.LoadFromFile(fullPath);
            if (ProjectData != null)
            {
                _projectPath = fullPath;
                Window.LoadProject(ProjectData, fullPath);
                Window.Title = $"DREditor {Program.Version}: {ProjectData.Name}";
            }
        }

        private void Initialize() {

            EmptyProject();
            LoadProject(new EnginePath("projects/test_project"));

            SubWindow test = new SubWindow(this, "Test window");
            test.ShowAll();
        }

        private void OnHandleExceptionEvent(GLib.UnhandledExceptionArgs args)
        {
            args.ExitApplication = false;
            Exception e = (Exception) args.ExceptionObject;
            // Error handling.
            if (Window == null)
            {
                Debug.LogError("Window not created yet, will print error to STDOUT.");
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
            }
            else
            {
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);

                Window d = new Window(WindowType.Toplevel);
                d.WindowPosition = WindowPosition.Center;
                //d.Parent = _window;
                d.Decorated = true;
                d.Deletable = true;
                d.Title = "Exception Encountered";
                Box b = new VBox();
                //b.Expand = false;
                //b.Fill = true;
                //b.PackType = PackType.Start;
                //b.Padding = 10;
                /*
                MessageDialog d = new MessageDialog(_window, DialogFlags.Modal, MessageType.Error, ButtonsType.None,
                    true,
                    $"<big>:( Program ran into an Exception!</big>"//\n\n <b>Message:</b> \n\n<tt>{e.Message}</tt>\n\n <b>Stack:</b> \n\n<tt>{e.StackTrace}</tt>\n\nTough luck buddy, Please contact dev if this is a problem!"
                );
                */
                Label preText = new Label("<big>The program encountered an exception!</big>");
                preText.UseMarkup = true;
                preText.Show();
                b.Add(preText);
                TextView output = new TextView();
                output.Editable = false;
                output.Monospace = true;
                output.CursorVisible = false;
                string message = $"Message: {e.Message}\n\nStacktrace:\n{e.StackTrace}";
                TextTag errTag = new TextTag("error");
                errTag.Foreground = "#FF3311";
                output.Buffer.TagTable.Add(errTag);
                TextIter start = output.Buffer.EndIter;
                output.Buffer.InsertWithTags(ref start, message, errTag);
                output.Margin = 10;
                output.Show();
                b.Add(output);
                Label postText =
                    new Label(
                        "If this is an issue, please copy+paste the text above and send it to the developer! Try to give as much additional info on how the error happened as possible."
                    );
                postText.Show();
                b.Add(postText);

                Button ok = new Button();
                ok.Label = "Ok";
                ok.Show();
                ok.Pressed += (sender, eventArgs) =>
                {
                    d.Dispose();
                };
                b.Add(ok);

                b.Show();
                d.Add(b);

                d.Show();
            }
        }

        private void WindowOnDeleteEvent(object o, DeleteEventArgs args)
        {
            Dispose();
        }

#region Disposable

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing) {
            if (_disposed) return;
            if(disposing)
            {
                // Dispose managed resources.
                // ex: component.Dispose();
                Window.Dispose();
                Window.DeleteEvent -= WindowOnDeleteEvent;
                GLib.ExceptionManager.UnhandledException -= OnHandleExceptionEvent;
                Application.Quit();
            }

            // TODO: Call the appropriate methods to clean up
            // unmanaged resources here.
            // If disposing is false,
            // only the following code is executed.
            //CloseHandle(handle);
            //handle = IntPtr.Zero;

            // Note disposing has been done.
            _disposed = true;
        }
#endregion
    }
}
