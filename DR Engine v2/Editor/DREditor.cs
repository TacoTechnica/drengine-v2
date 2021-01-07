using System;
using System.IO;
using DREngine.Editor.Components;
using DREngine.Editor.SubWindows;
using DREngine.ResourceLoading;
using GameEngine;
using GameEngine.Game.Audio;
using GameEngine.Game.Resources;
using Gdk;
using GLib;
using Gtk;
using Microsoft.Xna.Framework.Graphics;
using Application = Gtk.Application;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;

namespace DREngine.Editor
{
    public class DREditor : IDisposable
    {
        // TODO: Make this set to nothing or a custom/licensed theme.
        private const string STARTING_THEME = "themes/Material-Black-Lime/gtk-3.0/gtk.css";
        private bool _disposed;
        private AudioMixer _globalMixer;
        private string _projectPath;

        public ProjectData ProjectData;

        public DRProjectRunner ProjectRunner;

        public DREditor()
        {
            // Initialize our jank game. Run it one frame and grab the graphics device. Bullshit I tell you.
            var jankGame = new BackgroundJankGameRunner();
            jankGame.RunOneFrame();
            GraphicsDevice = jankGame.GraphicsDevice;
            // DO NOT DISPOSE
        }

        public DREditorMainWindow Window { get; private set; }

        public ResourceLoader ResourceLoader { get; private set; }
        public ResourceLoaderData ResourceLoaderData { get; private set; }

        public ResourceWindowManager ResourceWindowManager { get; private set; }

        public Icons Icons => Window.Icons;

        public AudioOutput AudioOutput { get; private set; }
        public AudioSource GlobalAudioSource { get; private set; }
        public GraphicsDevice GraphicsDevice { get; }

        public ResourceNameCache ResourceNameCache { get; private set; }

        public bool ProjectLoaded => ProjectData != null;

        ~DREditor()
        {
            Dispose(false);
        }

        public void Run()
        {
            Debug.LogDebug("Editor Run()");
            ExceptionManager.UnhandledException += OnHandleExceptionEvent;
            // Init app
            Application.Init();

            ProjectRunner = new DRProjectRunner();
            ResourceLoaderData = new ResourceLoaderData();
            ResourceLoader = new ResourceLoader(ResourceLoaderData);

            ResourceNameCache = new ResourceNameCache();

            Window = new DREditorMainWindow(this);
            Window.MakeWindow("DR Editor", 640, 480);
            Window.AddEvents((int) (EventMask.ButtonPressMask | EventMask.ButtonReleaseMask));
            Window.Show();
            Window.DeleteEvent += WindowOnDeleteEvent;

            Window.OnFileOpened += (path, fullPath) =>
            {
                // Ignore directories
                if (Directory.Exists(fullPath)) return;

                if (File.Exists(fullPath))
                    OpenProjectFile(path);
                else
                    Debug.LogWarning($"No file found at {fullPath} from project path {path}. This is a bug.");
            };

            // Kinda jank but like... if it works it works right?
            //GraphicsDevice = CreateGraphicsDevice();
            AudioOutput = new AudioOutput();
            _globalMixer = new AudioMixer(AudioOutput);

            GlobalAudioSource = new AudioSource(_globalMixer);

            ResourceLoaderData.Initialize(GraphicsDevice, AudioOutput);

            ResourceWindowManager = new ResourceWindowManager(this);


            if (!string.IsNullOrEmpty(STARTING_THEME)) Window.SetTheme(STARTING_THEME);
            Initialize();
            Application.Run();
        }

        public void RunCurrentProject()
        {
            if (ProjectData == null)
                Window.AlertProblem("No project is loaded, will not run anything.");
            else
                ProjectRunner.RunProject(_projectPath);
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
            Debug.LogDebug($"Loading project at {fullPath}");
            ProjectData = ProjectData.LoadFromFile(fullPath);
            if (ProjectData != null)
            {
                ResourceNameCache.Clear();
                _projectPath = fullPath;
                Window.LoadProject(ProjectData, fullPath, file => { ResourceNameCache.AddToCache(file); });
                Window.Title = $"DREditor {Program.Version}: {ProjectData.Name}";
            }
        }

        public void ReloadCurrentProject()
        {
            LoadProject(_projectPath);
        }

        private void Initialize()
        {
            EmptyProject();
        }

        private void OpenProjectFile(string projectPath)
        {
            ResourceWindowManager.OpenResource(new ProjectPath(this, projectPath));
        }

        private void OnHandleExceptionEvent(UnhandledExceptionArgs args)
        {
            args.ExitApplication = false;
            var e = (Exception) args.ExceptionObject;
            // Error handling.
            if (Window == null)
            {
                Debug.LogError("Window not created yet, will print error to STDOUT.");
                Debug.LogError(e.ToString());
            }
            else
            {
                Debug.LogError(e.ToString());

                var d = new Window(WindowType.Toplevel);
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
                var preText = new Text("<big>The program encountered an exception!</big>");
                preText.UseMarkup = true;
                preText.Show();
                b.Add(preText);
                var output = new TextView();
                output.Editable = false;
                output.Monospace = true;
                output.CursorVisible = false;
                var message = $"Message: {e.Message}\n\nException:\n{e}";
                var errTag = new TextTag("error");
                errTag.Foreground = "#FF3311";
                output.Buffer.TagTable.Add(errTag);
                var start = output.Buffer.EndIter;
                output.Buffer.InsertWithTags(ref start, message, errTag);
                output.Margin = 10;
                output.Show();
                b.Add(output);
                var postText =
                    new Text(
                        "If this is an issue, please copy+paste the text above and send it to the developer! Try to give as much additional info on how the error happened as possible."
                    );
                postText.Show();
                b.Add(postText);

                var ok = new Button();
                ok.Label = "Ok";
                ok.Show();
                ok.Pressed += (sender, eventArgs) => { d.Dispose(); };
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // Dispose managed resources.
                // ex: component.Dispose();
                Window.Dispose();
                Window.DeleteEvent -= WindowOnDeleteEvent;
                ExceptionManager.UnhandledException -= OnHandleExceptionEvent;
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